using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.Dashboard
{
    public class IndexModel : BaseEVMStaffPageModel
    {
        private readonly IProductService _productService;
        private readonly IDealerService _dealerService;
        private readonly IOrderService _orderService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IEVMReportService _evmReportService;

        public IndexModel(
            IProductService productService,
            IDealerService dealerService,
            IOrderService orderService,
            IPurchaseOrderService purchaseOrderService,
            IEVMReportService evmReportService)
        {
            _productService = productService;
            _dealerService = dealerService;
            _orderService = orderService;
            _purchaseOrderService = purchaseOrderService;
            _evmReportService = evmReportService;
        }

        // Dashboard Statistics
        public int TotalProducts { get; set; }
        public int TotalDealers { get; set; }
        public int TotalOrders { get; set; }
        public int PendingPurchaseOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int LowStockProducts { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Load dashboard statistics
            await LoadStatisticsAsync();

            return Page();
        }

        // API handlers for charts
        public async Task<IActionResult> OnGetSalesChartDataAsync()
        {
            try
            {
                // Lấy data thật từ tất cả dealers (không filter theo dealerId)
                var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone).Date;

                var salesData = new List<object>();
                var labels = new List<string>();

                // Lấy dữ liệu 6 tháng gần nhất
                for (int i = 5; i >= 0; i--)
                {
                    var targetDate = today.AddMonths(-i);
                    var month = targetDate.Month;
                    var year = targetDate.Year;

                    var sales = await _evmReportService.GetSalesReportByRegionAsync(null, null, "monthly", year, month, null);
                    var totalRevenue = sales.Sum(o => o.FinalAmount);
                    var orderCount = sales.Count;

                    labels.Add(targetDate.ToString("MM/yyyy"));
                    salesData.Add(new { month = targetDate.ToString("MM/yyyy"), revenue = totalRevenue, orders = sales.Count });
                }

                return new JsonResult(new
                {
                    labels = labels,
                    data = salesData
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
        }

        public async Task<IActionResult> OnGetOrdersChartDataAsync()
        {
            try
            {
                // Lấy data thật cho xu hướng đơn hàng 4 tuần gần nhất
                var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone).Date;

                var ordersData = new List<object>();
                var labels = new List<string>();

                // Lấy dữ liệu 4 tuần gần nhất
                for (int i = 3; i >= 0; i--)
                {
                    var weekStart = today.AddDays(-(i * 7 + (int)today.DayOfWeek));
                    var weekEnd = weekStart.AddDays(6);

                    // Lấy orders trong tuần này
                    var sales = await _evmReportService.GetSalesReportByRegionAsync(null, null, "monthly", weekStart.Year, weekStart.Month, null);
                    var weekOrders = sales.Where(o => 
                    {
                        var orderDate = TimeZoneInfo.ConvertTimeFromUtc(o.CreatedAt.ToUniversalTime(), vnTimeZone);
                        return orderDate >= weekStart && orderDate <= weekEnd;
                    }).ToList();

                    var totalRevenue = weekOrders.Sum(o => o.FinalAmount);
                    var orderCount = weekOrders.Count;

                    labels.Add($"Tuần {i + 1}");
                    ordersData.Add(new { week = $"Tuần {i + 1}", orders = orderCount, revenue = totalRevenue });
                }

                return new JsonResult(new
                {
                    labels = labels,
                    data = ordersData
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
        }

        public async Task<IActionResult> OnGetInventoryChartDataAsync()
        {
            try
            {
                // Lấy data thật cho inventory theo thương hiệu
                var inventoryReport = await _evmReportService.GetInventoryReportAsync();
                var brands = await _evmReportService.GetAllBrandsAsync();
                
                var brandInventory = inventoryReport
                    .GroupBy(p => p.Brand?.Name ?? "Chưa phân loại")
                    .Select(g => new { 
                        brandName = g.Key, 
                        totalQuantity = g.Count(),
                        totalValue = g.Sum(p => p.Price)
                    })
                    .OrderByDescending(x => x.totalQuantity)
                    .Take(5) // Top 5 thương hiệu
                    .ToList();

                return new JsonResult(brandInventory);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
        }

        public async Task<IActionResult> OnGetDealersChartDataAsync()
        {
            try
            {
                // Lấy data thật cho top dealers theo doanh số
                var dealers = await _evmReportService.GetAllDealersAsync();
                var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone).Date;
                var currentMonth = today.Month;
                var currentYear = today.Year;

                var dealersData = new List<object>();

                foreach (var dealer in dealers)
                {
                    var sales = await _evmReportService.GetSalesReportByRegionAsync(null, dealer.Id, "monthly", currentYear, currentMonth, null);
                    var totalSales = sales.Sum(o => o.FinalAmount);
                    var orderCount = sales.Count;

                    dealersData.Add(new { 
                        dealerName = dealer.Name ?? "Chưa có tên", 
                        totalSales = totalSales,
                        orderCount = orderCount
                    });
                }

                // Sắp xếp theo doanh số và lấy top 5
                var topDealers = dealersData
                    .OrderByDescending(d => ((dynamic)d).totalSales)
                    .Take(5)
                    .ToList();

                return new JsonResult(topDealers);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
        }

        private async Task LoadStatisticsAsync()
        {
            try
            {
                // 1. Total Products
                var productsResult = await _productService.SearchAsync(null, null, null, null, null, true);
                if (productsResult.Success && productsResult.Data != null)
                {
                    TotalProducts = productsResult.Data.Count;
                    LowStockProducts = productsResult.Data.Count(p => p.StockQuantity <= 10);
                }

                // 2. Total Dealers
                var dealersResult = await _dealerService.GetAllAsync();
                if (dealersResult.Success && dealersResult.Data != null)
                {
                    TotalDealers = dealersResult.Data.Count;
                }

                // 3. Total Orders
                var ordersResult = await _orderService.GetAllAsync();
                if (ordersResult.Success && ordersResult.Data != null)
                {
                    TotalOrders = ordersResult.Data.Count;
                    TotalRevenue = ordersResult.Data.Sum(o => o.FinalAmount);
                }

                // 4. Pending Purchase Orders
                var purchaseOrdersResult = await _purchaseOrderService.GetAllAsync();
                if (purchaseOrdersResult.Success && purchaseOrdersResult.Data != null)
                {
                    PendingPurchaseOrders = purchaseOrdersResult.Data.Count(po => po.Status == 0); // Pending
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi khi tải dữ liệu dashboard: {ex.Message}";
            }
        }
    }
}

