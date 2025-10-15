using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.ViewModels;
using BusinessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    public class PurchaseOrderController : BaseDashboardController
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IEVMReportService _evmService;
        private readonly IMappingService _mappingService;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, IEVMReportService evmService, IMappingService mappingService)
        {
            _purchaseOrderService = purchaseOrderService;
            _evmService = evmService;
            _mappingService = mappingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(PurchaseOrderStatus? status = null)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            Guid? dealerIdFilter = null;

            // Dealer chỉ xem đơn của mình, Admin/EVM xem tất cả
            if (userRole == "DealerManager" || userRole == "DealerStaff")
            {
                if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out Guid dealerId))
                {
                    TempData["Error"] = $"Tài khoản {userEmail} chưa được gán đại lý. DealerId trong session: {dealerIdString ?? "NULL"}. Vui lòng liên hệ Admin để gán dealer.";
                    TempData["Debug"] = $"🔍 Debug: Role={userRole}, DealerId={dealerIdString ?? "NULL"}, Email={userEmail}";
                    return RedirectToAction("Index", "Dashboard");
                }
                dealerIdFilter = dealerId;
            }

            var (ok, err, purchaseOrders) = await _purchaseOrderService.GetAllAsync(dealerIdFilter, status);
            if (!ok)
            {
                TempData["Error"] = err;
            }

            ViewBag.Status = status;
            // Map entities to ViewModels
            var purchaseOrderViewModels = purchaseOrders != null ? _mappingService.MapToPurchaseOrderViewModels(purchaseOrders) : new List<PurchaseOrderViewModel>();
            return View(purchaseOrderViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var (ok, err, purchaseOrder) = await _purchaseOrderService.GetAsync(id);
            
            if (!ok)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra quyền truy cập
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            if (userRole == "DealerManager" || userRole == "DealerStaff")
            {
                if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out Guid dealerId))
                {
                    TempData["Error"] = "Tài khoản chưa được gán đại lý.";
                    return RedirectToAction(nameof(Index));
                }

                if (purchaseOrder.DealerId != dealerId)
                {
                    TempData["Error"] = "Bạn không có quyền xem đơn đặt hàng này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(purchaseOrder);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            // Chỉ Dealer mới được tạo đơn đặt hàng
            if (userRole != "DealerManager" && userRole != "DealerStaff")
            {
                TempData["Error"] = "Chỉ Dealer Manager/Staff mới có quyền đặt xe từ hãng.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out Guid dealerId))
            {
                TempData["Error"] = "Tài khoản chưa được gán đại lý. Vui lòng liên hệ Admin.";
                return RedirectToAction("Index", "Dashboard");
            }

            await LoadProductsToViewBag();
            return View(new PurchaseOrderCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseOrderCreateViewModel model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            var userIdString = HttpContext.Session.GetString("UserId");

            // Kiểm tra quyền
            if (userRole != "DealerManager" && userRole != "DealerStaff")
            {
                TempData["Error"] = "Chỉ Dealer Manager/Staff mới có quyền đặt xe từ hãng.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out Guid dealerId))
            {
                TempData["Error"] = "Tài khoản chưa được gán đại lý.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                TempData["Error"] = "Phiên đăng nhập không hợp lệ.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                await LoadProductsToViewBag();
                return View(model);
            }

            var (ok, err, purchaseOrder) = await _purchaseOrderService.CreateAsync(
                dealerId, model.ProductId, userId, model.RequestedQuantity, model.UnitPrice,
                model.Reason, model.Notes, model.ExpectedDeliveryDate);

            if (!ok)
            {
                ModelState.AddModelError("", $"Chi tiết lỗi: {err}");
                TempData["Error"] = $"Lỗi tạo đơn: {err}";
                await LoadProductsToViewBag();
                return View(model);
            }

            TempData["Success"] = $"Tạo đơn đặt hàng thành công! Mã đơn: {purchaseOrder.OrderNumber}";
            return RedirectToAction(nameof(Detail), new { id = purchaseOrder.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            // Kiểm tra quyền hủy đơn
            var (exists, err, purchaseOrder) = await _purchaseOrderService.GetAsync(id);
            if (!exists)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(Index));
            }

            // Dealer chỉ được hủy đơn của mình
            if (userRole == "DealerManager" || userRole == "DealerStaff")
            {
                if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out Guid dealerId))
                {
                    TempData["Error"] = "Tài khoản chưa được gán đại lý.";
                    return RedirectToAction(nameof(Index));
                }

                if (purchaseOrder.DealerId != dealerId)
                {
                    TempData["Error"] = "Bạn không có quyền hủy đơn đặt hàng này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var (ok, cancelErr, canceledOrder) = await _purchaseOrderService.CancelAsync(id);
            if (!ok)
            {
                TempData["Error"] = cancelErr;
            }
            else
            {
                TempData["Success"] = "Hủy đơn đặt hàng thành công!";
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        // Admin/EVM actions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id, DateTime? expectedDeliveryDate = null, string notes = "")
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền duyệt đơn đặt hàng.";
                return RedirectToAction(nameof(Index));
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                TempData["Error"] = "Phiên đăng nhập không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var (ok, err, approvedOrder) = await _purchaseOrderService.ApproveAsync(id, userId, expectedDeliveryDate, notes);
            if (!ok)
            {
                TempData["Error"] = err;
            }
            else
            {
                TempData["Success"] = "Duyệt đơn đặt hàng thành công!";
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id, string rejectReason)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền từ chối đơn đặt hàng.";
                return RedirectToAction(nameof(Index));
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                TempData["Error"] = "Phiên đăng nhập không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(rejectReason))
            {
                TempData["Error"] = "Vui lòng nhập lý do từ chối.";
                return RedirectToAction(nameof(Detail), new { id });
            }

            var (ok, err, rejectedOrder) = await _purchaseOrderService.RejectAsync(id, userId, rejectReason);
            if (!ok)
            {
                TempData["Error"] = err;
            }
            else
            {
                TempData["Success"] = "Từ chối đơn đặt hàng thành công!";
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid id, PurchaseOrderStatus status, DateTime? actualDeliveryDate = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền cập nhật trạng thái đơn đặt hàng.";
                return RedirectToAction(nameof(Index));
            }

            var (ok, err, updatedOrder) = await _purchaseOrderService.UpdateStatusAsync(id, status, actualDeliveryDate);
            if (!ok)
            {
                TempData["Error"] = err;
            }
            else
            {
                TempData["Success"] = "Cập nhật trạng thái thành công!";
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        private async Task LoadProductsToViewBag()
        {
            var products = await _evmService.GetAllProductsAsync();
            ViewBag.Products = products;
        }
    }
}
