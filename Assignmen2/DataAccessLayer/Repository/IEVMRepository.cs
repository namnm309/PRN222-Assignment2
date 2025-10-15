using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public interface IEVMRepository
    {
        // Sales Reports
        Task<List<Order>> GetSalesReportByRegionAsync(Guid? regionId = null, Guid? dealerId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null);
        Task<decimal> GetTotalSalesAsync(Guid? regionId = null, Guid? dealerId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null);
        Task<List<Order>> GetSalesReportByStaffAsync(Guid? salesPersonId = null, Guid? dealerId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null);
        Task<List<Users>> GetAllSalesStaffAsync();

        // Inventory Reports
        Task<List<Product>> GetInventoryReportAsync(Guid? brandId = null, string priority = null);
        Task<List<Product>> GetLowStockProductsAsync();
        Task<List<Product>> GetCriticalStockProductsAsync();

        // Demand Forecast
        Task<List<Product>> GetDemandForecastAsync(int forecastPeriod = 6, Guid? productId = null, string priority = null);
        Task<List<Product>> GetHighPriorityForecastsAsync();

        // Contract Management
        Task<List<DealerContract>> GetContractManagementReportAsync(Guid? dealerId = null, string status = null, string riskLevel = null);
        Task<List<DealerContract>> GetExpiringContractsAsync(int daysAhead = 30);
        Task<List<DealerContract>> GetHighRiskContractsAsync();

        // Customer Debt Report
        Task<List<Order>> GetCustomerDebtReportAsync(Guid? customerId = null, string paymentStatus = null);

        // Support methods
        Task<List<Region>> GetAllRegionsAsync();
        Task<List<Dealer>> GetAllDealersAsync();
        Task<List<Brand>> GetAllBrandsAsync();
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Customer>> GetAllCustomersAsync();
        
        // User Management methods
        Task<List<Users>> GetAllUsersAsync();
        Task<List<Users>> GetUsersByDealerAsync(Guid dealerId);
        Task<Users> GetUserByIdAsync(Guid userId);
        Task<bool> UpdateUserAsync(Users user);
        Task<bool> DeleteUserAsync(Guid userId);
    }
}
