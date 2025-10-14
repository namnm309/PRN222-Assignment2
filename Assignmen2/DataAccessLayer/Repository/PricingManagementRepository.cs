using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class PricingManagementRepository : IPricingManagementRepository
    {
        private readonly AppDbContext _context;

        public PricingManagementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PricingPolicy>> GetAllPricingPoliciesAsync()
        {
            return await _context.PricingPolicy
                .Include(pp => pp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(pp => pp.Dealer)
                .Include(pp => pp.Region)
                .Where(pp => pp.IsActive)
                .OrderBy(pp => pp.Product.Name)
                .ThenBy(pp => pp.EffectiveDate)
                .ToListAsync();
        }

        public async Task<List<PricingPolicy>> GetPricingPoliciesByProductAsync(Guid productId)
        {
            return await _context.PricingPolicy
                .Include(pp => pp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(pp => pp.Dealer)
                .Include(pp => pp.Region)
                .Where(pp => pp.ProductId == productId && pp.IsActive)
                .OrderBy(pp => pp.EffectiveDate)
                .ToListAsync();
        }

        public async Task<List<PricingPolicy>> GetPricingPoliciesByDealerAsync(Guid dealerId)
        {
            return await _context.PricingPolicy
                .Include(pp => pp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(pp => pp.Dealer)
                .Include(pp => pp.Region)
                .Where(pp => pp.DealerId == dealerId && pp.IsActive)
                .OrderBy(pp => pp.Product.Name)
                .ThenBy(pp => pp.EffectiveDate)
                .ToListAsync();
        }

        public async Task<List<PricingPolicy>> GetPricingPoliciesByRegionAsync(Guid regionId)
        {
            return await _context.PricingPolicy
                .Include(pp => pp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(pp => pp.Dealer)
                .Include(pp => pp.Region)
                .Where(pp => pp.RegionId == regionId && pp.IsActive)
                .OrderBy(pp => pp.Product.Name)
                .ThenBy(pp => pp.EffectiveDate)
                .ToListAsync();
        }

        public async Task<PricingPolicy> GetPricingPolicyAsync(Guid id)
        {
            return await _context.PricingPolicy
                .Include(pp => pp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(pp => pp.Dealer)
                .Include(pp => pp.Region)
                .FirstOrDefaultAsync(pp => pp.Id == id && pp.IsActive);
        }

        public async Task<bool> CreatePricingPolicyAsync(PricingPolicy policy)
        {
            policy.CreatedAt = DateTime.UtcNow;
            policy.UpdatedAt = DateTime.UtcNow;
            
            await _context.PricingPolicy.AddAsync(policy);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdatePricingPolicyAsync(PricingPolicy policy)
        {
            policy.UpdatedAt = DateTime.UtcNow;
            
            _context.PricingPolicy.Update(policy);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeletePricingPolicyAsync(Guid id)
        {
            var policy = await _context.PricingPolicy.FindAsync(id);
            if (policy == null) return false;

            policy.IsActive = false;
            policy.UpdatedAt = DateTime.UtcNow;
            
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<PricingPolicy> GetActivePricingPolicyAsync(Guid productId, Guid? dealerId = null, Guid? regionId = null)
        {
            var now = DateTime.UtcNow;
            
            var query = _context.PricingPolicy
                .Include(pp => pp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(pp => pp.Dealer)
                .Include(pp => pp.Region)
                .Where(pp => pp.ProductId == productId && 
                            pp.IsActive && 
                            pp.Status == "Active" &&
                            pp.EffectiveDate <= now &&
                            (pp.ExpiryDate == null || pp.ExpiryDate > now))
                .AsQueryable();

            // Priority: Dealer-specific > Region-specific > General
            if (dealerId.HasValue)
            {
                var dealerPolicy = await query
                    .Where(pp => pp.DealerId == dealerId.Value)
                    .OrderByDescending(pp => pp.EffectiveDate)
                    .FirstOrDefaultAsync();
                
                if (dealerPolicy != null) return dealerPolicy;
            }

            if (regionId.HasValue)
            {
                var regionPolicy = await query
                    .Where(pp => pp.RegionId == regionId.Value && pp.DealerId == null)
                    .OrderByDescending(pp => pp.EffectiveDate)
                    .FirstOrDefaultAsync();
                
                if (regionPolicy != null) return regionPolicy;
            }

            // General policy
            return await query
                .Where(pp => pp.DealerId == null && pp.RegionId == null)
                .OrderByDescending(pp => pp.EffectiveDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<PricingPolicy>> GetActivePricingPoliciesAsync(Guid? dealerId = null, Guid? regionId = null)
        {
            var now = DateTime.UtcNow;
            
            var query = _context.PricingPolicy
                .Include(pp => pp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(pp => pp.Dealer)
                .Include(pp => pp.Region)
                .Where(pp => pp.IsActive && 
                            pp.Status == "Active" &&
                            pp.EffectiveDate <= now &&
                            (pp.ExpiryDate == null || pp.ExpiryDate > now))
                .AsQueryable();

            if (dealerId.HasValue)
                query = query.Where(pp => pp.DealerId == dealerId.Value || pp.DealerId == null);

            if (regionId.HasValue)
                query = query.Where(pp => pp.RegionId == regionId.Value || pp.RegionId == null);

            return await query
                .OrderBy(pp => pp.Product.Name)
                .ThenByDescending(pp => pp.EffectiveDate)
                .ToListAsync();
        }

        public async Task<decimal> GetWholesalePriceAsync(Guid productId, Guid? dealerId = null, Guid? regionId = null, int quantity = 1)
        {
            var policy = await GetActivePricingPolicyAsync(productId, dealerId, regionId);
            
            if (policy == null)
            {
                // Fallback to product base price
                var product = await _context.Product.FindAsync(productId);
                return product?.Price ?? 0;
            }

            // Check quantity-based pricing
            if (quantity >= policy.MinimumQuantity && quantity <= policy.MaximumQuantity)
            {
                var discountedPrice = policy.WholesalePrice * (1 - policy.DiscountRate / 100);
                return Math.Max(discountedPrice, policy.MinimumPrice);
            }

            return policy.WholesalePrice;
        }

        public async Task<decimal> GetRetailPriceAsync(Guid productId, Guid? dealerId = null, Guid? regionId = null)
        {
            var policy = await GetActivePricingPolicyAsync(productId, dealerId, regionId);
            
            if (policy == null)
            {
                // Fallback to product base price
                var product = await _context.Product.FindAsync(productId);
                return product?.Price ?? 0;
            }

            return policy.RetailPrice;
        }

        public async Task<decimal> CalculateDiscountAsync(Guid productId, Guid? dealerId = null, Guid? regionId = null, int quantity = 1)
        {
            var policy = await GetActivePricingPolicyAsync(productId, dealerId, regionId);
            
            if (policy == null) return 0;

            // Check quantity-based discount
            if (quantity >= policy.MinimumQuantity && quantity <= policy.MaximumQuantity)
            {
                return policy.DiscountRate;
            }

            return 0;
        }

        public async Task<List<PricingPolicy>> GetDiscountPoliciesAsync(Guid? dealerId = null, string policyType = null)
        {
            var now = DateTime.UtcNow;
            
            var query = _context.PricingPolicy
                .Include(pp => pp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(pp => pp.Dealer)
                .Include(pp => pp.Region)
                .Where(pp => pp.IsActive && 
                            pp.Status == "Active" &&
                            pp.EffectiveDate <= now &&
                            (pp.ExpiryDate == null || pp.ExpiryDate > now) &&
                            pp.DiscountRate > 0)
                .AsQueryable();

            if (dealerId.HasValue)
                query = query.Where(pp => pp.DealerId == dealerId.Value);

            if (!string.IsNullOrEmpty(policyType))
                query = query.Where(pp => pp.PolicyType == policyType);

            return await query
                .OrderByDescending(pp => pp.DiscountRate)
                .ToListAsync();
        }

        public async Task<List<PricingPolicy>> GetExpiringPoliciesAsync(int daysAhead = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(daysAhead);
            
            return await _context.PricingPolicy
                .Include(pp => pp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(pp => pp.Dealer)
                .Include(pp => pp.Region)
                .Where(pp => pp.IsActive && 
                            pp.Status == "Active" &&
                            pp.ExpiryDate.HasValue &&
                            pp.ExpiryDate.Value <= cutoffDate &&
                            pp.ExpiryDate.Value > DateTime.UtcNow)
                .OrderBy(pp => pp.ExpiryDate)
                .ToListAsync();
        }

        public async Task<List<PricingPolicy>> GetExpiredPoliciesAsync()
        {
            var now = DateTime.UtcNow;
            
            return await _context.PricingPolicy
                .Include(pp => pp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(pp => pp.Dealer)
                .Include(pp => pp.Region)
                .Where(pp => pp.IsActive && 
                            pp.Status == "Active" &&
                            pp.ExpiryDate.HasValue &&
                            pp.ExpiryDate.Value <= now)
                .OrderBy(pp => pp.ExpiryDate)
                .ToListAsync();
        }

        public async Task<bool> BulkUpdatePricingPoliciesAsync(List<PricingPolicy> policies)
        {
            foreach (var policy in policies)
            {
                policy.UpdatedAt = DateTime.UtcNow;
            }
            
            _context.PricingPolicy.UpdateRange(policies);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeactivateExpiredPoliciesAsync()
        {
            var expiredPolicies = await GetExpiredPoliciesAsync();
            
            foreach (var policy in expiredPolicies)
            {
                policy.Status = "Expired";
                policy.UpdatedAt = DateTime.UtcNow;
            }
            
            if (expiredPolicies.Any())
            {
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            
            return true;
        }

        public async Task<List<PricingPolicy>> GetPricingReportAsync(Guid? productId = null, Guid? dealerId = null, Guid? regionId = null, string status = null)
        {
            var query = _context.PricingPolicy
                .Include(pp => pp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(pp => pp.Dealer)
                .Include(pp => pp.Region)
                .Where(pp => pp.IsActive)
                .AsQueryable();

            if (productId.HasValue)
                query = query.Where(pp => pp.ProductId == productId.Value);

            if (dealerId.HasValue)
                query = query.Where(pp => pp.DealerId == dealerId.Value);

            if (regionId.HasValue)
                query = query.Where(pp => pp.RegionId == regionId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(pp => pp.Status == status);

            return await query
                .OrderBy(pp => pp.Product.Name)
                .ThenBy(pp => pp.EffectiveDate)
                .ToListAsync();
        }

        public async Task<Dictionary<string, decimal>> GetPricingSummaryAsync()
        {
            var summary = new Dictionary<string, decimal>();
            var now = DateTime.UtcNow;

            var activePolicies = await _context.PricingPolicy
                .Where(pp => pp.IsActive && 
                            pp.Status == "Active" &&
                            pp.EffectiveDate <= now &&
                            (pp.ExpiryDate == null || pp.ExpiryDate > now))
                .ToListAsync();

            summary["TotalPolicies"] = activePolicies.Count;
            summary["AverageWholesalePrice"] = activePolicies.Any() ? activePolicies.Average(pp => pp.WholesalePrice) : 0;
            summary["AverageRetailPrice"] = activePolicies.Any() ? activePolicies.Average(pp => pp.RetailPrice) : 0;
            summary["AverageDiscountRate"] = activePolicies.Any() ? activePolicies.Average(pp => pp.DiscountRate) : 0;
            summary["MaxDiscountRate"] = activePolicies.Any() ? activePolicies.Max(pp => pp.DiscountRate) : 0;
            summary["PoliciesWithDiscount"] = activePolicies.Count(pp => pp.DiscountRate > 0);

            return summary;
        }
    }
}
