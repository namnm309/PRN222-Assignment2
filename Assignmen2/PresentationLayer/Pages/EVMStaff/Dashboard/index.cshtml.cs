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

