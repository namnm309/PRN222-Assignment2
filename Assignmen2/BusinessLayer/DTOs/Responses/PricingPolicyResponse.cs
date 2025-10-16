using System;

namespace BusinessLayer.DTOs.Responses
{
	public class PricingPolicyResponse
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public Guid? DealerId { get; set; }
		public Guid? RegionId { get; set; }
		public decimal WholesalePrice { get; set; }
		public decimal RetailPrice { get; set; }
		public decimal DiscountRate { get; set; }
		public decimal MinimumPrice { get; set; }
		public DateTime EffectiveDate { get; set; }
		public DateTime? ExpiryDate { get; set; }
		public string PolicyType { get; set; } = "Standard";
		public string ApplicableConditions { get; set; } = string.Empty;
		public int MinimumQuantity { get; set; }
		public int MaximumQuantity { get; set; }
		public string Status { get; set; } = "Active";
		public string Notes { get; set; } = string.Empty;
		public bool IsActive { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		
		// Navigation properties
		public string ProductName { get; set; } = string.Empty;
		public string DealerName { get; set; } = string.Empty;
		public string RegionName { get; set; } = string.Empty;
	}
}


