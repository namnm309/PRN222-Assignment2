using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IPricingManagementService
    {
        // Pricing Policy Management
        Task<List<PricingPolicy>> GetAllPricingPoliciesAsync();
        Task<List<PricingPolicy>> GetPricingPoliciesByProductAsync(Guid productId);
        Task<List<PricingPolicy>> GetPricingPoliciesByDealerAsync(Guid dealerId);
        Task<List<PricingPolicy>> GetPricingPoliciesByRegionAsync(Guid regionId);
        Task<PricingPolicy> GetPricingPolicyAsync(Guid id);
        Task<bool> CreatePricingPolicyAsync(PricingPolicy policy);
        Task<bool> UpdatePricingPolicyAsync(PricingPolicy policy);
        Task<bool> DeletePricingPolicyAsync(Guid id);

        // Active Pricing
        Task<PricingPolicy> GetActivePricingPolicyAsync(Guid productId, Guid? dealerId = null, Guid? regionId = null);
        Task<List<PricingPolicy>> GetActivePricingPoliciesAsync(Guid? dealerId = null, Guid? regionId = null);
        Task<decimal> GetWholesalePriceAsync(Guid productId, Guid? dealerId = null, Guid? regionId = null, int quantity = 1);
        Task<decimal> GetRetailPriceAsync(Guid productId, Guid? dealerId = null, Guid? regionId = null);

        // Discount Management
        Task<decimal> CalculateDiscountAsync(Guid productId, Guid? dealerId = null, Guid? regionId = null, int quantity = 1);
        Task<List<PricingPolicy>> GetDiscountPoliciesAsync(Guid? dealerId = null, string policyType = null);

        // Policy Lifecycle
        Task<List<PricingPolicy>> GetExpiringPoliciesAsync(int daysAhead = 30);
        Task<List<PricingPolicy>> GetExpiredPoliciesAsync();
        Task<bool> BulkUpdatePricingPoliciesAsync(List<PricingPolicy> policies);
        Task<bool> DeactivateExpiredPoliciesAsync();

        // Reports
        Task<List<PricingPolicy>> GetPricingReportAsync(Guid? productId = null, Guid? dealerId = null, Guid? regionId = null, string status = null);
        Task<Dictionary<string, decimal>> GetPricingSummaryAsync();

        // Support methods
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Dealer>> GetAllDealersAsync();
        Task<List<Region>> GetAllRegionsAsync();
    }
}
