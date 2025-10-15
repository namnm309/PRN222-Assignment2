using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class PricingPolicy : BaseEntity
    {
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public Guid? DealerId { get; set; }
        public virtual Dealer Dealer { get; set; }

        public Guid? RegionId { get; set; }
        public virtual Region Region { get; set; }

        public decimal WholesalePrice { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal DiscountRate { get; set; } // %
        public decimal MinimumPrice { get; set; }
        
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        
        public string PolicyType { get; set; } = "Standard"; // Standard, VIP, Bulk, Seasonal
        public string ApplicableConditions { get; set; } // Điều kiện áp dụng
        public int MinimumQuantity { get; set; } = 1;
        public int MaximumQuantity { get; set; } = 999;
        
        public string Status { get; set; } = "Active"; // Active, Inactive, Expired
        public string Notes { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
