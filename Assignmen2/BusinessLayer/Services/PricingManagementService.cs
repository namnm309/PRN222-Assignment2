using DataAccessLayer.Repository;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class PricingManagementService : IPricingManagementService
    {
        private readonly IPricingManagementRepository _pricingRepository;
        private readonly IEVMRepository _evmRepository;

        public PricingManagementService(IPricingManagementRepository pricingRepository, IEVMRepository evmRepository)
        {
            _pricingRepository = pricingRepository;
            _evmRepository = evmRepository;
        }

        public async Task<List<PricingPolicy>> GetAllPricingPoliciesAsync()
        {
            return await _pricingRepository.GetAllPricingPoliciesAsync();
        }

        public async Task<List<PricingPolicy>> GetPricingPoliciesByProductAsync(Guid productId)
        {
            return await _pricingRepository.GetPricingPoliciesByProductAsync(productId);
        }

        public async Task<List<PricingPolicy>> GetPricingPoliciesByDealerAsync(Guid dealerId)
        {
            return await _pricingRepository.GetPricingPoliciesByDealerAsync(dealerId);
        }

        public async Task<List<PricingPolicy>> GetPricingPoliciesByRegionAsync(Guid regionId)
        {
            return await _pricingRepository.GetPricingPoliciesByRegionAsync(regionId);
        }

        public async Task<PricingPolicy> GetPricingPolicyAsync(Guid id)
        {
            return await _pricingRepository.GetPricingPolicyAsync(id);
        }

        public async Task<bool> CreatePricingPolicyAsync(PricingPolicy policy)
        {
            // Validate business rules
            if (policy.WholesalePrice <= 0 || policy.RetailPrice <= 0)
                return false;

            if (policy.WholesalePrice > policy.RetailPrice)
                return false;

            if (policy.DiscountRate < 0 || policy.DiscountRate > 100)
                return false;

            if (policy.MinimumPrice > policy.WholesalePrice)
                return false;

            if (policy.EffectiveDate > policy.ExpiryDate)
                return false;

            policy.Id = Guid.NewGuid();
            policy.CreatedAt = DateTime.UtcNow;
            policy.UpdatedAt = DateTime.UtcNow;
            return await _pricingRepository.CreatePricingPolicyAsync(policy);
        }

        public async Task<bool> UpdatePricingPolicyAsync(PricingPolicy policy)
        {
            // Validate business rules
            if (policy.WholesalePrice <= 0 || policy.RetailPrice <= 0)
                return false;

            if (policy.WholesalePrice > policy.RetailPrice)
                return false;

            if (policy.DiscountRate < 0 || policy.DiscountRate > 100)
                return false;

            if (policy.MinimumPrice > policy.WholesalePrice)
                return false;

            if (policy.EffectiveDate > policy.ExpiryDate)
                return false;

            policy.UpdatedAt = DateTime.UtcNow;
            return await _pricingRepository.UpdatePricingPolicyAsync(policy);
        }

        public async Task<bool> DeletePricingPolicyAsync(Guid id)
        {
            return await _pricingRepository.DeletePricingPolicyAsync(id);
        }

        public async Task<PricingPolicy> GetActivePricingPolicyAsync(Guid productId, Guid? dealerId = null, Guid? regionId = null)
        {
            return await _pricingRepository.GetActivePricingPolicyAsync(productId, dealerId, regionId);
        }

        public async Task<List<PricingPolicy>> GetActivePricingPoliciesAsync(Guid? dealerId = null, Guid? regionId = null)
        {
            return await _pricingRepository.GetActivePricingPoliciesAsync(dealerId, regionId);
        }

        public async Task<decimal> GetWholesalePriceAsync(Guid productId, Guid? dealerId = null, Guid? regionId = null, int quantity = 1)
        {
            return await _pricingRepository.GetWholesalePriceAsync(productId, dealerId, regionId, quantity);
        }

        public async Task<decimal> GetRetailPriceAsync(Guid productId, Guid? dealerId = null, Guid? regionId = null)
        {
            return await _pricingRepository.GetRetailPriceAsync(productId, dealerId, regionId);
        }

        public async Task<decimal> CalculateDiscountAsync(Guid productId, Guid? dealerId = null, Guid? regionId = null, int quantity = 1)
        {
            return await _pricingRepository.CalculateDiscountAsync(productId, dealerId, regionId, quantity);
        }

        public async Task<List<PricingPolicy>> GetDiscountPoliciesAsync(Guid? dealerId = null, string policyType = null)
        {
            return await _pricingRepository.GetDiscountPoliciesAsync(dealerId, policyType);
        }

        public async Task<List<PricingPolicy>> GetExpiringPoliciesAsync(int daysAhead = 30)
        {
            return await _pricingRepository.GetExpiringPoliciesAsync(daysAhead);
        }

        public async Task<List<PricingPolicy>> GetExpiredPoliciesAsync()
        {
            return await _pricingRepository.GetExpiredPoliciesAsync();
        }

        public async Task<bool> BulkUpdatePricingPoliciesAsync(List<PricingPolicy> policies)
        {
            // Validate each policy
            foreach (var policy in policies)
            {
                if (policy.WholesalePrice <= 0 || policy.RetailPrice <= 0)
                    return false;

                if (policy.WholesalePrice > policy.RetailPrice)
                    return false;

                if (policy.DiscountRate < 0 || policy.DiscountRate > 100)
                    return false;
            }

            return await _pricingRepository.BulkUpdatePricingPoliciesAsync(policies);
        }

        public async Task<bool> DeactivateExpiredPoliciesAsync()
        {
            return await _pricingRepository.DeactivateExpiredPoliciesAsync();
        }

        public async Task<List<PricingPolicy>> GetPricingReportAsync(Guid? productId = null, Guid? dealerId = null, Guid? regionId = null, string status = null)
        {
            return await _pricingRepository.GetPricingReportAsync(productId, dealerId, regionId, status);
        }

        public async Task<Dictionary<string, decimal>> GetPricingSummaryAsync()
        {
            return await _pricingRepository.GetPricingSummaryAsync();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _evmRepository.GetAllProductsAsync();
        }

        public async Task<List<Dealer>> GetAllDealersAsync()
        {
            return await _evmRepository.GetAllDealersAsync();
        }

        public async Task<List<Region>> GetAllRegionsAsync()
        {
            return await _evmRepository.GetAllRegionsAsync();
        }
    }
}
