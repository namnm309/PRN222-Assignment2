using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.Enums;
using BusinessLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    public class CustomerManagementController : BaseDashboardController
    {
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly ITestDriveService _testDriveService;
        private readonly IMappingService _mappingService;

        public CustomerManagementController(ICustomerService customerService, IOrderService orderService, ITestDriveService testDriveService, IMappingService mappingService)
        {
            _customerService = customerService;
            _orderService = orderService;
            _testDriveService = testDriveService;
            _mappingService = mappingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search = null)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdStr = HttpContext.Session.GetString("DealerId");

            List<CustomerViewModel> customers;

            // Admin và EVM Staff xem tất cả khách hàng
            if (userRole == "Admin" || userRole == "EVMStaff")
            {
                var (ok, err, data) = await _customerService.GetAllAsync();
                if (!ok)
                {
                    TempData["Error"] = err;
                    return View(new List<CustomerViewModel>());
                }
                customers = _mappingService.MapToCustomerViewModels(data);
            }
            // Dealer chỉ xem khách hàng của mình
            else if ((userRole == "DealerManager" || userRole == "DealerStaff") && !string.IsNullOrEmpty(dealerIdStr))
            {
                var dealerId = Guid.Parse(dealerIdStr);
                var (ok, err, data) = await _customerService.GetAllByDealerAsync(dealerId);
                if (!ok)
                {
                    TempData["Error"] = err;
                    return View(new List<CustomerViewModel>());
                }
                customers = _mappingService.MapToCustomerViewModels(data);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                customers = customers.Where(c =>
                    c.FullName.ToLower().Contains(searchLower) ||
                    c.PhoneNumber.Contains(search) ||
                    (c.Email != null && c.Email.ToLower().Contains(searchLower))
                ).ToList();
            }

            ViewBag.Search = search;
            return View(customers);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var (ok, err, customer) = await _customerService.GetAsync(id);
            if (!ok)
            {
                TempData["Error"] = err ?? "Không tìm thấy khách hàng";
                return RedirectToAction(nameof(Index));
            }

            // Lấy lịch sử đơn hàng qua service và lọc theo khách hàng
            var (_, _, allOrders) = await _orderService.GetAllAsync(null, null);
            var orders = allOrders
                .Where(o => o.CustomerId == id)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            // Lấy lịch sử lái thử qua service
            var (_, _, testDrives) = await _testDriveService.GetByCustomerAsync(id);

            ViewBag.Orders = orders;
            ViewBag.TestDrives = testDrives;
            ViewBag.TotalSpent = orders.Sum(o => o.FinalAmount);
            ViewBag.TotalOrders = orders.Count;

            return View(_mappingService.MapToCustomerViewModel(customer));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateFromTestDrive(string fullName = null, string email = null, string phoneNumber = null, string address = null)
        {
            // Pre-fill form với dữ liệu từ TestDrive nếu có
            ViewBag.PreFillData = new
            {
                FullName = fullName ?? "",
                Email = email ?? "",
                PhoneNumber = phoneNumber ?? "",
                Address = address ?? ""
            };
            
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string fullName, string email, string phoneNumber, string address = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var (ok, err, customer) = await _customerService.CreateAsync(fullName, email, phoneNumber, address);
            if (!ok)
            {
                ModelState.AddModelError("", err);
                return View();
            }

            TempData["Msg"] = "Thêm khách hàng thành công!";
            
            // Nếu đến từ TestDrive, redirect về TestDrive thay vì Detail
            if (Request.Headers["Referer"].ToString().Contains("TestDrive"))
            {
                TempData["Success"] = "Khách hàng đã được tạo thành công từ lịch lái thử!";
                return RedirectToAction("Index", "TestDrive");
            }
            
            return RedirectToAction(nameof(Detail), new { id = customer.Id });
        }
    }
}

