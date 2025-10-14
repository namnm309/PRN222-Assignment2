using System;

namespace BusinessLayer.DTOs.Responses
{
	public class PurchaseOrderResponse
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public Guid DealerId { get; set; }
		public string DealerName { get; set; } = string.Empty;
		public string ProductName { get; set; } = string.Empty;
		public string RequestedByName { get; set; } = string.Empty;
		public string ApprovedByName { get; set; } = string.Empty;
		public int RequestedQuantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal TotalAmount { get; set; }
		public string Status { get; set; } = string.Empty;
	}
}


