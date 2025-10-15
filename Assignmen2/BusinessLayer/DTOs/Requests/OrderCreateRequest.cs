using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs.Requests
{
	public class OrderCreateRequest
	{
		[Required]
		public Guid ProductId { get; set; }
		[Required]
		public Guid CustomerId { get; set; }
		public Guid? DealerId { get; set; }
		public Guid? SalesPersonId { get; set; }
		[Required]
		[Range(0, double.MaxValue)]
		public decimal Price { get; set; }
		[Range(0, double.MaxValue)]
		public decimal Discount { get; set; }
		[MaxLength(500)]
		public string Description { get; set; } = string.Empty;
		[MaxLength(1000)]
		public string Notes { get; set; } = string.Empty;
	}
}


