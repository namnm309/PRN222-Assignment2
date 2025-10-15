using System;

namespace BusinessLayer.DTOs.Responses
{
	public class OrderResponse
	{
		public Guid Id { get; set; }
		public string OrderNumber { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public decimal Discount { get; set; }
		public decimal FinalAmount { get; set; }
		public string Status { get; set; } = string.Empty;
		public string PaymentStatus { get; set; } = string.Empty;
		public string PaymentMethod { get; set; } = string.Empty;
		public DateTime? OrderDate { get; set; }
		public DateTime? DeliveryDate { get; set; }
		public DateTime? PaymentDueDate { get; set; }
		public Guid ProductId { get; set; }
		public Guid CustomerId { get; set; }
		public Guid? DealerId { get; set; }
		public Guid? SalesPersonId { get; set; }
		public string? CustomerName { get; set; }
		public string? ProductName { get; set; }
		public string? DealerName { get; set; }
		public string? SalesPersonName { get; set; }
	}
}


