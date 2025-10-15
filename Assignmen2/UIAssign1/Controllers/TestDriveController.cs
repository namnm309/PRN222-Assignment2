using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.Services;
using BusinessLayer.ViewModels;
using System.Linq;

namespace PresentationLayer.Controllers
{
    public class TestDriveController : BaseDashboardController
    {
        private readonly ITestDriveService _service;
        private readonly IEVMReportService _evmService;
        private readonly IMappingService _mappingService;
        
        public TestDriveController(ITestDriveService service, IEVMReportService evmService, IMappingService mappingService)
        {
            _service = service;
            _evmService = evmService;
            _mappingService = mappingService;
        }

        // Danh sách lịch hẹn cho Dealer Staff/Manager
        [HttpGet]
        public async Task<IActionResult> Index(Guid? dealerId = null, string? status = null)
        {
            ViewBag.Dealers = await _evmService.GetAllDealersAsync();
            ViewBag.SelectedDealerId = dealerId;
            ViewBag.SelectedStatus = status;

            // Xác định dealerIdFilter dựa trên role
            Guid? dealerIdFilter = null;
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            
            if (userRole == "DealerManager" || userRole == "DealerStaff")
            {
                if (!string.IsNullOrEmpty(dealerIdString) && Guid.TryParse(dealerIdString, out var dealerIdParsed))
                {
                    dealerIdFilter = dealerIdParsed; // Dealer chỉ thấy lịch hẹn của mình
                }
                else
                {
                    TempData["Error"] = "Tài khoản chưa được gán đại lý. Vui lòng liên hệ Admin.";
                    return View(new List<TestDriveViewModel>());
                }
            }
            else
            {
                // Admin/EVM có thể xem tất cả lịch hẹn hoặc filter theo dealerId parameter
                dealerIdFilter = dealerId;
            }

            var (ok, err, testDrives) = await _service.GetAllAsync(dealerIdFilter, status);
            if (!ok)
            {
                TempData["Error"] = err;
                return View(new List<TestDriveViewModel>());
            }

            // Map entities to ViewModels
            var testDriveViewModels = _mappingService.MapToTestDriveViewModels(testDrives);
            return View(testDriveViewModels);
        }

        // Lịch hẹn của Customer
        [HttpGet]
        public async Task<IActionResult> MyTestDrives(Guid customerId)
        {
            var (ok, err, testDrives) = await _service.GetByCustomerAsync(customerId);
            if (!ok)
            {
                TempData["Error"] = err;
                return View(new List<TestDriveViewModel>());
            }
            
            // Map entities to ViewModels
            var testDriveViewModels = _mappingService.MapToTestDriveViewModels(testDrives);
            return View(testDriveViewModels);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Create(Guid productId, Guid? dealerId = null)
        {
            // Load danh sách dealers để khách hàng chọn
            var dealers = await _evmService.GetAllDealersAsync();
            if (!dealers.Any())
            {
                TempData["Error"] = "Hiện tại chưa có đại lý nào có sẵn. Vui lòng liên hệ trực tiếp.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Dealers = dealers;
            
            return View(new TestDriveViewModel { ProductId = productId, DealerId = dealerId ?? Guid.Empty });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TestDriveViewModel vm)
        {
            if (vm.ScheduledDate == default)
            {
                // Nếu view không truyền lên (ẩn), đặt mặc định sau 2 giờ kể từ hiện tại (UTC)
                vm.ScheduledDate = DateTime.UtcNow.AddHours(2);
            }

            // Validate DealerId
            if (vm.DealerId == Guid.Empty)
            {
                ModelState.AddModelError("DealerId", "Vui lòng chọn đại lý");
                ViewBag.Dealers = await _evmService.GetAllDealersAsync();
                return View(vm);
            }

            var (ok, err, td) = await _service.CreatePublicAsync(
                vm.CustomerName, 
                vm.CustomerPhone, 
                vm.CustomerEmail, 
                vm.Notes,
                vm.ProductId, 
                vm.DealerId, 
                vm.ScheduledDate
            );
            
            if (!ok) 
            { 
                ModelState.AddModelError("", err);
                ViewBag.Dealers = await _evmService.GetAllDealersAsync();
                return View(vm); 
            }

            TempData["Msg"] = "Đăng ký lái thử thành công! Mã đặt lịch: " + td.Id.ToString().Substring(0, 8).ToUpper();
            TempData["Success"] = "true";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var (ok, err, td) = await _service.GetAsync(id);
            if (!ok) return NotFound();
            return View(td);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(Guid id)
        {
            var (ok, err, td) = await _service.ConfirmAsync(id);
            if (!ok) return BadRequest(err);
            TempData["Msg"] = "Đã xác nhận lịch hẹn.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Complete(Guid id, bool success = true)
        {
            var (ok, err, td) = await _service.CompleteAsync(id, success);
            if (!ok) return BadRequest(err);
            TempData["Msg"] = success ? "Lái thử thành công" : "Lái thử thất bại";
            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var (ok, err, td) = await _service.CancelAsync(id);
            if (!ok) return BadRequest(err);
            TempData["Msg"] = "Đã hủy lịch hẹn.";
            return RedirectToAction(nameof(Detail), new { id });
        }
    }
}
