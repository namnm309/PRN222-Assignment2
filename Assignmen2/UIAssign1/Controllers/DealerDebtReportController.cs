using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Enums;
using BusinessLayer.Services;

namespace PresentationLayer.Controllers
{
    public class DealerDebtReportController : BaseDashboardController
    {
        private readonly IDealerDebtService _debtService;
        
        public DealerDebtReportController(IDealerDebtService debtService)
        {
            _debtService = debtService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? customerId = null, string? paymentStatus = null)
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var dealerIdStr = HttpContext.Session.GetString("DealerId");
            if (!Guid.TryParse(dealerIdStr, out var dealerId))
            {
                TempData["Error"] = "Không tìm thấy thông tin đại lý.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Lấy báo cáo công nợ từ service
            var (orders, totalDebt) = await _debtService.GetDebtReportAsync(dealerId, customerId, paymentStatus);
            
            // Lấy danh sách khách hàng từ service
            var customers = await _debtService.GetDealerCustomersAsync(dealerId);

            ViewBag.Customers = customers;
            ViewBag.CustomerId = customerId;
            ViewBag.PaymentStatus = paymentStatus;
            ViewBag.TotalDebt = totalDebt;

            return View(orders);
        }
    }
}
