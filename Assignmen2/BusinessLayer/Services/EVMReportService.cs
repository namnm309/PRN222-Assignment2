using DataAccessLayer.Repository;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class EVMReportService : IEVMReportService
    {
        private readonly IEVMRepository _evmRepository;

        public EVMReportService(IEVMRepository evmRepository)
        {
            _evmRepository = evmRepository;
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
    }
}
