using DataAccessLayer.Repository;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BusinessLayer.Services
{
    public class EVMReportService : IEVMReportService
    {
        private readonly IEVMRepository _evmRepository;
        private readonly IInventoryManagementService _inventoryManagementService;
        private readonly IDealerDebtService _dealerDebtService;

        public EVMReportService(IEVMRepository evmRepository, IInventoryManagementService inventoryManagementService, IDealerDebtService dealerDebtService)
        {
            _evmRepository = evmRepository;
            _inventoryManagementService = inventoryManagementService;
            _dealerDebtService = dealerDebtService;
        }

        public async Task<List<Order>> GetSalesReportByRegionAsync(Guid? regionId = null, Guid? dealerId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null)
        {
            return await _evmRepository.GetSalesReportByRegionAsync(regionId, dealerId, period, year, month, quarter);
        }

        public async Task<decimal> GetTotalSalesAsync(Guid? regionId = null, Guid? dealerId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null)
        {
            return await _evmRepository.GetTotalSalesAsync(regionId, dealerId, period, year, month, quarter);
        }

        public async Task<List<Product>> GetInventoryReportAsync(Guid? brandId = null, string priority = null)
        {
            return await _evmRepository.GetInventoryReportAsync(brandId, priority);
        }

        public async Task<List<Product>> GetLowStockProductsAsync()
        {
            return await _evmRepository.GetLowStockProductsAsync();
        }

        public async Task<List<Product>> GetCriticalStockProductsAsync()
        {
            return await _evmRepository.GetCriticalStockProductsAsync();
        }

        public async Task<List<Product>> GetDemandForecastAsync(int forecastPeriod = 6, Guid? productId = null, string priority = null)
        {
            return await _evmRepository.GetDemandForecastAsync(forecastPeriod, productId, priority);
        }

        public async Task<List<Product>> GetHighPriorityForecastsAsync()
        {
            return await _evmRepository.GetHighPriorityForecastsAsync();
        }

        public async Task<List<DealerContract>> GetContractManagementReportAsync(Guid? dealerId = null, string status = null, string riskLevel = null)
        {
            return await _evmRepository.GetContractManagementReportAsync(dealerId, status, riskLevel);
        }

        public async Task<List<DealerContract>> GetExpiringContractsAsync(int daysAhead = 30)
        {
            return await _evmRepository.GetExpiringContractsAsync(daysAhead);
        }

        public async Task<List<DealerContract>> GetHighRiskContractsAsync()
        {
            return await _evmRepository.GetHighRiskContractsAsync();
        }

        public async Task<List<Region>> GetAllRegionsAsync()
        {
            return await _evmRepository.GetAllRegionsAsync();
        }

        public async Task<List<Dealer>> GetAllDealersAsync()
        {
            return await _evmRepository.GetAllDealersAsync();
        }

        public async Task<List<Brand>> GetAllBrandsAsync()
        {
            return await _evmRepository.GetAllBrandsAsync();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _evmRepository.GetAllProductsAsync();
        }

        public async Task<List<Order>> GetSalesReportByStaffAsync(Guid? salesPersonId = null, Guid? dealerId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null)
        {
            return await _evmRepository.GetSalesReportByStaffAsync(salesPersonId, dealerId, period, year, month, quarter);
        }

        public async Task<List<Users>> GetAllSalesStaffAsync()
        {
            return await _evmRepository.GetAllSalesStaffAsync();
        }

        public async Task<List<Order>> GetCustomerDebtReportAsync(Guid? customerId = null, string paymentStatus = null)
        {
            return await _evmRepository.GetCustomerDebtReportAsync(customerId, paymentStatus);
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _evmRepository.GetAllCustomersAsync();
        }

        public async Task<List<Users>> GetAllUsersAsync()
        {
            return await _evmRepository.GetAllUsersAsync();
        }

        public async Task<List<Users>> GetUsersByDealerAsync(Guid dealerId)
        {
            return await _evmRepository.GetUsersByDealerAsync(dealerId);
        }

        public async Task<Users> GetUserByIdAsync(Guid userId)
        {
            return await _evmRepository.GetUserByIdAsync(userId);
        }

        public async Task<bool> UpdateUserAsync(Users user)
        {
            return await _evmRepository.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            return await _evmRepository.DeleteUserAsync(userId);
        }

        // Dashboard Chart Data Implementation
        public async Task<object> GetSalesChartDataAsync(Guid dealerId)
        {
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

                var sales = await GetSalesReportByRegionAsync(null, dealerId, "monthly", year, month, null);
                var totalRevenue = sales.Sum(o => o.FinalAmount);
                var orderCount = sales.Count;

                labels.Add(targetDate.ToString("MM/yyyy"));
                salesData.Add(new { revenue = totalRevenue, orders = orderCount });
            }

            return new { labels, data = salesData };
        }

        public async Task<object> GetInventoryChartDataAsync(Guid dealerId)
        {
            var inventoryAllocations = await _inventoryManagementService.GetInventoryAllocationsByDealerAsync(dealerId);
            var products = await _inventoryManagementService.GetAllProductsAsync();

            // Nhóm theo thương hiệu sản phẩm
            var categoryData = inventoryAllocations
                .Join(products, ia => ia.ProductId, p => p.Id, (ia, p) => new { ia, p })
                .GroupBy(x => x.p.Brand?.Name ?? "Chưa phân loại")
                .Select(g => new { 
                    category = g.Key, 
                    totalQuantity = g.Sum(x => x.ia.AllocatedQuantity),
                    totalValue = g.Sum(x => x.ia.AllocatedQuantity * x.p.Price)
                })
                .OrderByDescending(x => x.totalQuantity)
                .ToList();

            return categoryData;
        }

        public async Task<object> GetDebtChartDataAsync(Guid dealerId)
        {
            var debtReport = await _dealerDebtService.GetDebtReportAsync(dealerId);
            var customers = await _dealerDebtService.GetDealerCustomersAsync(dealerId);

            // Nhóm công nợ theo khách hàng
            var customerDebts = debtReport.Orders
                .GroupBy(o => o.CustomerId)
                .Select(g => {
                    var customer = customers.FirstOrDefault(c => c.Id == g.Key);
                    return new {
                        customerName = customer?.Name ?? "Khách hàng không xác định",
                        totalDebt = g.Sum(o => o.FinalAmount),
                        orderCount = g.Count()
                    };
                })
                .Where(x => x.totalDebt > 0)
                .OrderByDescending(x => x.totalDebt)
                .Take(10) // Top 10 khách hàng có công nợ cao nhất
                .ToList();

            return customerDebts;
        }

        public async Task<object> GetOrdersChartDataAsync(Guid dealerId)
        {
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone).Date;

            var ordersData = new List<object>();
            var labels = new List<string>();

            // Lấy dữ liệu 12 tuần gần nhất
            for (int i = 11; i >= 0; i--)
            {
                var weekStart = today.AddDays(-(i * 7 + (int)today.DayOfWeek));
                var weekEnd = weekStart.AddDays(6);

                var sales = await GetSalesReportByRegionAsync(null, dealerId, "monthly", weekStart.Year, weekStart.Month, null);
                var weekOrders = sales.Count(o => {
                    var orderDate = TimeZoneInfo.ConvertTimeFromUtc(o.CreatedAt.ToUniversalTime(), vnTimeZone).Date;
                    return orderDate >= weekStart && orderDate <= weekEnd;
                });

                labels.Add($"Tuần {weekStart.Day}/{weekStart.Month}");
                ordersData.Add(weekOrders);
            }

            return new { labels, data = ordersData };
        }
    }
}
