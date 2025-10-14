using System;

namespace BusinessLayer.DTOs.Responses
{
	public class PricingPolicyResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal DiscountPercentage { get; set; }
		public decimal MinOrderValue { get; set; }
		public DateTime ValidFrom { get; set; }
		public DateTime ValidTo { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
}


