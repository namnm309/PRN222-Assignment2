using System;
using BusinessLayer.Enums;

namespace BusinessLayer.DTOs.Responses
{
	public class PurchaseOrderResponse
	{
		public Guid Id { get; set; }
		public string OrderNumber { get; set; } = string.Empty;
		public Guid ProductId { get; set; }
		public Guid DealerId { get; set; }
		public string DealerName { get; set; } = string.Empty;
		public string ProductName { get; set; } = string.Empty;
		public string ProductSku { get; set; } = string.Empty;
		public string RequestedByName { get; set; } = string.Empty;
		public string ApprovedByName { get; set; } = string.Empty;
		public int RequestedQuantity { get; set; }
		public int Quantity { get; set; } // Alias for RequestedQuantity
		public decimal UnitPrice { get; set; }
		public decimal TotalAmount { get; set; }
		public PurchaseOrderStatus Status { get; set; }
		public DateTime RequestedDate { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime? ExpectedDeliveryDate { get; set; }
		public DateTime? ActualDeliveryDate { get; set; }
		public string Reason { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
	}
}


