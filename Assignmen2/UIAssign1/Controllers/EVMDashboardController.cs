using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.Enums;

namespace PresentationLayer.Controllers
{
    public class EVMDashboardController : BaseDashboardController
    {
        private readonly IEVMReportService _evmReportService;

        public EVMDashboardController(IEVMReportService evmReportService)
        {
            _evmReportService = evmReportService;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "EVM Dashboard";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SalesReport(Guid? regionId = null, Guid? dealerId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Set default values if not provided
            if (year == 0)
                year = DateTime.Now.Year;
            if (month == null && period == "monthly")
                month = DateTime.Now.Month;

            var salesReport = await _evmReportService.GetSalesReportByRegionAsync(regionId, dealerId, period, year, month, quarter);
            var totalSales = await _evmReportService.GetTotalSalesAsync(regionId, dealerId, period, year, month, quarter);

            ViewBag.TotalSales = totalSales;
            ViewBag.RegionId = regionId;
            ViewBag.DealerId = dealerId;
            ViewBag.Period = period;
            ViewBag.Year = year;
            ViewBag.Month = month;
            ViewBag.Quarter = quarter;

            // Get dropdown data
            ViewBag.Regions = await _evmReportService.GetAllRegionsAsync();
            ViewBag.Dealers = await _evmReportService.GetAllDealersAsync();


            return View(salesReport);
        }

        [HttpGet]
        public async Task<IActionResult> InventoryReport(Guid? brandId = null, string priority = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var inventoryReport = await _evmReportService.GetInventoryReportAsync(brandId, priority);

            ViewBag.BrandId = brandId;
            ViewBag.Priority = priority;

            // Get dropdown data
            ViewBag.Brands = await _evmReportService.GetAllBrandsAsync();


            return View(inventoryReport);
        }

        [HttpGet]
        public async Task<IActionResult> DemandForecast(int forecastPeriod = 6, Guid? productId = null, string priority = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var demandForecast = await _evmReportService.GetDemandForecastAsync(forecastPeriod, productId, priority);

            ViewBag.ForecastPeriod = forecastPeriod;
            ViewBag.ProductId = productId;
            ViewBag.Priority = priority;

            // Get dropdown data
            ViewBag.Products = await _evmReportService.GetAllProductsAsync();


            return View(demandForecast);
        }

        [HttpGet]
        public async Task<IActionResult> ContractManagement(Guid? dealerId = null, string status = null, string riskLevel = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var contractReport = await _evmReportService.GetContractManagementReportAsync(dealerId, status, riskLevel);

            ViewBag.DealerId = dealerId;
            ViewBag.Status = status;
            ViewBag.RiskLevel = riskLevel;

            // Get dropdown data
            ViewBag.Dealers = await _evmReportService.GetAllDealersAsync();


            return View(contractReport);
        }

        // API endpoints for AJAX calls
        [HttpGet]
        public async Task<JsonResult> GetSalesData(Guid? regionId = null, Guid? dealerId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null)
        {
            if (!IsAdmin())
                return Json(new { error = "Unauthorized" });

            var salesReport = await _evmReportService.GetSalesReportByRegionAsync(regionId, dealerId, period, year, month, quarter);
            return Json(salesReport);
        }

        [HttpGet]
        public async Task<JsonResult> GetInventoryData(Guid? brandId = null, string priority = null)
        {
            if (!IsAdmin())
                return Json(new { error = "Unauthorized" });

            var inventoryReport = await _evmReportService.GetInventoryReportAsync(brandId, priority);
            return Json(inventoryReport);
        }

        [HttpGet]
        public async Task<JsonResult> GetDemandForecastData(int forecastPeriod = 6, Guid? productId = null, string priority = null)
        {
            if (!IsAdmin())
                return Json(new { error = "Unauthorized" });

            var demandForecast = await _evmReportService.GetDemandForecastAsync(forecastPeriod, productId, priority);
            return Json(demandForecast);
        }

        [HttpGet]
        public async Task<JsonResult> GetContractData(Guid? dealerId = null, string status = null, string riskLevel = null)
        {
            if (!IsAdmin())
                return Json(new { error = "Unauthorized" });

            var contractReport = await _evmReportService.GetContractManagementReportAsync(dealerId, status, riskLevel);
            return Json(contractReport);
        }

        [HttpGet]
        public async Task<IActionResult> SalesReportByStaff(Guid? salesPersonId = null, Guid? dealerId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Set default values if not provided
            if (year == 0)
                year = DateTime.Now.Year;
            if (month == null && period == "monthly")
                month = DateTime.Now.Month;

            var salesReport = await _evmReportService.GetSalesReportByStaffAsync(salesPersonId, dealerId, period, year, month, quarter);
            var totalSales = salesReport.Sum(o => o.FinalAmount);

            ViewBag.TotalSales = totalSales;
            ViewBag.SalesPersonId = salesPersonId;
            ViewBag.DealerId = dealerId;
            ViewBag.Period = period;
            ViewBag.Year = year;
            ViewBag.Month = month;
            ViewBag.Quarter = quarter;

            // Get dropdown data
            ViewBag.SalesStaff = await _evmReportService.GetAllSalesStaffAsync();
            ViewBag.Dealers = await _evmReportService.GetAllDealersAsync();


            return View(salesReport);
        }

        [HttpGet]
        public async Task<IActionResult> CustomerDebtReport(Guid? customerId = null, string paymentStatus = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var debtReport = await _evmReportService.GetCustomerDebtReportAsync(customerId, paymentStatus);
            var totalDebt = debtReport.Sum(o => o.FinalAmount);

            ViewBag.TotalDebt = totalDebt;
            ViewBag.CustomerId = customerId;
            ViewBag.PaymentStatus = paymentStatus;

            // Get dropdown data
            ViewBag.Customers = await _evmReportService.GetAllCustomersAsync();


            return View(debtReport);
        }
    }
}