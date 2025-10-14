using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs.Requests
{
	public class PurchaseOrderCreateRequest
	{
		[Required]
		public Guid ProductId { get; set; }
		[Required]
		[Range(1, 1000)]
		public int RequestedQuantity { get; set; }
		[Required]
		[Range(0.01, double.MaxValue)]
		public decimal UnitPrice { get; set; }
		[Required]
		[MaxLength(500)]
		public string Reason { get; set; } = string.Empty;
		[MaxLength(1000)]
		public string Notes { get; set; } = string.Empty;
		[DataType(DataType.Date)]
		public DateTime? ExpectedDeliveryDate { get; set; }
	}
}


