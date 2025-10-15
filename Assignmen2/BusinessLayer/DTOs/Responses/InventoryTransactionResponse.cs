using System;

namespace BusinessLayer.DTOs.Responses
{
	public class InventoryTransactionResponse
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public Guid DealerId { get; set; }
		public int Quantity { get; set; }
		public string TransactionType { get; set; } = string.Empty;
		public string Reason { get; set; } = string.Empty;
		public Guid? RelatedDealerId { get; set; }
		public Guid ProcessedByUserId { get; set; }
		public DateTime CreatedAt { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public string DealerName { get; set; } = string.Empty;
		public string? RelatedDealerName { get; set; }
		public string ProcessedByUserName { get; set; } = string.Empty;
	}
}


