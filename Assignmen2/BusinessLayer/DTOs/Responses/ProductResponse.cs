using System;

namespace BusinessLayer.DTOs.Responses
{
	public class ProductResponse
	{
		public Guid Id { get; set; }
		public string Sku { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public int StockQuantity { get; set; }
		public bool IsActive { get; set; } = true;
		public string? ImageUrl { get; set; }
		public Guid BrandId { get; set; }
		public string BrandName { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		// Search-related fields removed from response
	}
}


