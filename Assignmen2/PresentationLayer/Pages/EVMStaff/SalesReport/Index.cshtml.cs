using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.SalesReport
{
    public class IndexModel : BaseEVMStaffPageModel
    {
        private readonly IEVMReportService _evmReportService;
        private readonly IOrderService _orderService;
        private readonly IDealerService _dealerService;

        public IndexModel(
            IEVMReportService evmReportService,
            IOrderService orderService,
            IDealerService dealerService)
        {
            _evmReportService = evmReportService;
            _orderService = orderService;
            _dealerService = dealerService;
        }

        [BindProperty(SupportsGet = true)]
        public string ReportType { get; set; } = "region"; // "region" or "dealer"

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? RegionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? DealerId { get; set; }

        // Report data
        public List<Region> Regions { get; set; } = new();
        public List<Dealer> Dealers { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
        
        // Summary statistics
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public decimal AverageOrderValue { get; set; }

        // Chart data for region/dealer
        public Dictionary<string, decimal> RevenueByGroup { get; set; } = new();
        public Dictionary<string, int> OrderCountByGroup { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Set default date range if not specified (last 30 days)
            if (!FromDate.HasValue)
                FromDate = DateTime.Now.AddDays(-30);
            
            if (!ToDate.HasValue)
                ToDate = DateTime.Now;

            // Load regions and dealers for filters
            await LoadRegionsAndDealersAsync();

            // Load report data
            if (ReportType == "dealer")
            {
                await LoadDealerReportAsync();
            }
            else
            {
                await LoadRegionReportAsync();
            }

            return Page();
        }

        private async Task LoadRegionsAndDealersAsync()
        {
            Regions = await _evmReportService.GetAllRegionsAsync() ?? new List<Region>();

            var dealersResult = await _dealerService.GetAllAsync();
            if (dealersResult.Success && dealersResult.Data != null)
            {
                Dealers = dealersResult.Data;
            }
        }

        private async Task LoadRegionReportAsync()
        {
            var ordersResult = await _orderService.GetAllAsync();
            if (ordersResult.Success && ordersResult.Data != null)
            {
                Orders = ordersResult.Data;

                // Filter by date range
                Orders = Orders.Where(o => 
                    o.OrderDate >= FromDate && 
                    o.OrderDate <= ToDate).ToList();

                // Filter by region if specified
                if (RegionId.HasValue)
                {
                    Orders = Orders.Where(o => o.RegionId == RegionId.Value).ToList();
                }

                // Calculate statistics
                TotalOrders = Orders.Count;
                TotalRevenue = Orders.Sum(o => o.FinalAmount);
                TotalProducts = Orders.Count; // Each order is 1 product
                AverageOrderValue = TotalOrders > 0 ? TotalRevenue / TotalOrders : 0m;

                // Group by region
                var groupedByRegion = Orders
                    .GroupBy(o => o.Region?.Name ?? "Không xác định")
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(o => o.FinalAmount)
                    );

                RevenueByGroup = groupedByRegion;

                OrderCountByGroup = Orders
                    .GroupBy(o => o.Region?.Name ?? "Không xác định")
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count()
                    );
            }
        }

        private async Task LoadDealerReportAsync()
        {
            var ordersResult = await _orderService.GetAllAsync();
            if (ordersResult.Success && ordersResult.Data != null)
            {
                Orders = ordersResult.Data;

                // Filter by date range
                Orders = Orders.Where(o => 
                    o.OrderDate >= FromDate && 
                    o.OrderDate <= ToDate).ToList();

                // Filter by dealer if specified
                if (DealerId.HasValue)
                {
                    Orders = Orders.Where(o => o.DealerId == DealerId.Value).ToList();
                }

                // Calculate statistics
                TotalOrders = Orders.Count;
                TotalRevenue = Orders.Sum(o => o.FinalAmount);
                TotalProducts = Orders.Count; // Each order is 1 product
                AverageOrderValue = TotalOrders > 0 ? TotalRevenue / TotalOrders : 0m;

                // Group by dealer
                var groupedByDealer = Orders
                    .GroupBy(o => o.Dealer?.Name ?? "Không xác định")
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(o => o.FinalAmount)
                    );

                RevenueByGroup = groupedByDealer;

                OrderCountByGroup = Orders
                    .GroupBy(o => o.Dealer?.Name ?? "Không xác định")
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count()
                    );
            }
        }
    }
}

