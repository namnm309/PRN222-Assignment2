using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.Enums;
using BusinessLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    public class DealerReportController : BaseDashboardController
    {
        private readonly IEVMReportService _evmReportService;
        private readonly IMappingService _mappingService;

        public DealerReportController(IEVMReportService evmReportService, IMappingService mappingService)
        {
            _evmReportService = evmReportService;
            _mappingService = mappingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Lấy dealerId của user hiện tại
            var dealerIdStr = HttpContext.Session.GetString("DealerId");
            if (!Guid.TryParse(dealerIdStr, out var dealerId))
            {
                TempData["Error"] = "Không tìm thấy thông tin đại lý.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Lấy dữ liệu tổng quan
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            // Doanh số tháng hiện tại
            var monthlySales = await _evmReportService.GetSalesReportByRegionAsync(null, dealerId, "monthly", currentYear, currentMonth, null);
            var monthlyTotal = monthlySales.Sum(o => o.FinalAmount);

            // Doanh số năm hiện tại
            var yearlySales = await _evmReportService.GetSalesReportByRegionAsync(null, dealerId, "yearly", currentYear, null, null);
            var yearlyTotal = yearlySales.Sum(o => o.FinalAmount);

            // Top nhân viên bán hàng tháng này
            var topEmployees = monthlySales
                .Where(o => o.SalesPersonId.HasValue)
                .GroupBy(o => new { o.SalesPersonId, o.SalesPerson.FullName })
                .Select(g => new
                {
                    EmployeeId = g.Key.SalesPersonId,
                    EmployeeName = g.Key.FullName,
                    TotalSales = g.Sum(o => o.FinalAmount),
                    OrderCount = g.Count()
                })
                .OrderByDescending(x => x.TotalSales)
                .Take(10)
                .Cast<dynamic>()
                .ToList();

            // Top xe bán chạy tháng này
            var topVehicles = monthlySales
                .GroupBy(o => new { o.ProductId, o.Product.Name, o.Product.Sku })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    ProductSku = g.Key.Sku,
                    TotalSales = g.Sum(o => o.FinalAmount),
                    OrderCount = g.Count(),
                    AveragePrice = g.Average(o => o.FinalAmount)
                })
                .OrderByDescending(x => x.TotalSales)
                .Take(10)
                .Cast<dynamic>()
                .ToList();

            ViewBag.MonthlyTotal = monthlyTotal;
            ViewBag.YearlyTotal = yearlyTotal;
            ViewBag.TopEmployees = topEmployees;
            ViewBag.TopVehicles = topVehicles;
            ViewBag.CurrentYear = currentYear;
            ViewBag.CurrentMonth = currentMonth;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SalesByEmployee(Guid? employeeId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null)
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Lấy dealerId của user hiện tại
            var dealerIdStr = HttpContext.Session.GetString("DealerId");
            if (!Guid.TryParse(dealerIdStr, out var dealerId))
            {
                TempData["Error"] = "Không tìm thấy thông tin đại lý.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Set default values
            if (year == 0)
                year = DateTime.Now.Year;
            if (month == null && period == "monthly")
                month = DateTime.Now.Month;

            var salesReport = await _evmReportService.GetSalesReportByStaffAsync(employeeId, dealerId, period, year, month, quarter);
            var totalSales = salesReport.Sum(o => o.FinalAmount);

            ViewBag.TotalSales = totalSales;
            ViewBag.EmployeeId = employeeId;
            ViewBag.Period = period;
            ViewBag.Year = year;
            ViewBag.Month = month;
            ViewBag.Quarter = quarter;

            // Get dropdown data - chỉ nhân viên của dealer này
            var users = await _evmReportService.GetUsersByDealerAsync(dealerId);
            ViewBag.Employees = _mappingService.MapToUserViewModels(users);

            return View(salesReport);
        }

        [HttpGet]
        public async Task<IActionResult> TopSellingVehicles(string period = "monthly", int year = 0, int? month = null, int? quarter = null)
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Lấy dealerId của user hiện tại
            var dealerIdStr = HttpContext.Session.GetString("DealerId");
            if (!Guid.TryParse(dealerIdStr, out var dealerId))
            {
                TempData["Error"] = "Không tìm thấy thông tin đại lý.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Set default values
            if (year == 0)
                year = DateTime.Now.Year;
            if (month == null && period == "monthly")
                month = DateTime.Now.Month;

            var sales = await _evmReportService.GetSalesReportByRegionAsync(null, dealerId, period, year, month, quarter);
            var topVehicles = sales
                .GroupBy(o => new { o.ProductId, o.Product.Name, o.Product.Sku })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    ProductSku = g.Key.Sku,
                    TotalSales = g.Sum(o => o.FinalAmount),
                    OrderCount = g.Count(),
                    AveragePrice = g.Average(o => o.FinalAmount)
                })
                .OrderByDescending(x => x.TotalSales)
                .Take(10)
                .Cast<dynamic>()
                .ToList();

            ViewBag.Period = period;
            ViewBag.Year = year;
            ViewBag.Month = month;
            ViewBag.Quarter = quarter;

            return View(topVehicles);
        }

        // Private helper methods
        // Đã thay thế bằng IEVMReportService và xử lý nhóm tại Controller

        // helper cũ đã bỏ, dùng service BL

        // Đã thay bằng IEVMReportService
    }
}
