using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.SalesReport
{
    public class IndexModel : BaseEVMStaffPageModel
    {
        private readonly IEVMReportService _evmReportService;
        private readonly IOrderService _orderService;
        private readonly IDealerService _dealerService;
        private readonly IMappingService _mappingService;

        public IndexModel(
            IEVMReportService evmReportService,
            IOrderService orderService,
            IDealerService dealerService,
            IMappingService mappingService)
        {
            _evmReportService = evmReportService;
            _orderService = orderService;
            _dealerService = dealerService;
            _mappingService = mappingService;
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
        public List<RegionResponse> Regions { get; set; } = new();
        public List<DealerResponse> Dealers { get; set; } = new();
        public List<OrderResponse> Orders { get; set; } = new();
        
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
            var regions = await _evmReportService.GetAllRegionsAsync();
            if (regions != null)
            {
                Regions = _mappingService.MapToRegionViewModels(regions);
            }

            var dealersResult = await _dealerService.GetAllAsync();
            if (dealersResult.Success && dealersResult.Data != null)
            {
                Dealers = _mappingService.MapToDealerViewModels(dealersResult.Data);
            }
        }

        private async Task LoadRegionReportAsync()
        {
            var ordersResult = await _orderService.GetAllAsync();
            if (ordersResult.Success && ordersResult.Data != null)
            {
                // Map entities to DTOs
                Orders = _mappingService.MapToOrderCreateViewModels(ordersResult.Data);

                // Filter by date range
                Orders = Orders.Where(o => 
                    o.OrderDate >= FromDate && 
                    o.OrderDate <= ToDate).ToList();

                // Filter by region if specified - need to get dealer's region
                if (RegionId.HasValue)
                {
                    // Get dealers in this region
                    var dealersInRegion = Dealers.Where(d => d.RegionId == RegionId.Value).Select(d => d.Id).ToList();
                    Orders = Orders.Where(o => o.DealerId.HasValue && dealersInRegion.Contains(o.DealerId.Value)).ToList();
                }

                // Calculate statistics
                TotalOrders = Orders.Count;
                TotalRevenue = Orders.Sum(o => o.FinalAmount);
                TotalProducts = Orders.Count; // Each order is 1 product
                AverageOrderValue = TotalOrders > 0 ? TotalRevenue / TotalOrders : 0m;

                // Group by region - use dealer's region
                var groupedByRegion = Orders
                    .GroupBy(o => {
                        var dealer = Dealers.FirstOrDefault(d => d.Id == o.DealerId);
                        return dealer?.RegionName ?? "Không xác định";
                    })
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(o => o.FinalAmount)
                    );

                RevenueByGroup = groupedByRegion;

                OrderCountByGroup = Orders
                    .GroupBy(o => {
                        var dealer = Dealers.FirstOrDefault(d => d.Id == o.DealerId);
                        return dealer?.RegionName ?? "Không xác định";
                    })
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
                // Map entities to DTOs
                Orders = _mappingService.MapToOrderCreateViewModels(ordersResult.Data);

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
                    .GroupBy(o => o.DealerName ?? "Không xác định")
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(o => o.FinalAmount)
                    );

                RevenueByGroup = groupedByDealer;

                OrderCountByGroup = Orders
                    .GroupBy(o => o.DealerName ?? "Không xác định")
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count()
                    );
            }
        }
    }
}

