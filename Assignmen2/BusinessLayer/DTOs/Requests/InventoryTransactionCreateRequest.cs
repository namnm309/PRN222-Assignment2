using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs.Requests
{
	public class InventoryTransactionCreateRequest
	{
		[Required]
		public Guid ProductId { get; set; }
		[Required]
		public Guid DealerId { get; set; }
		[Required]
		public int Quantity { get; set; }
		[Required]
		public string TransactionType { get; set; } = string.Empty;
		public string Reason { get; set; } = string.Empty;
		public Guid? RelatedDealerId { get; set; }
		[Required]
		public Guid ProcessedByUserId { get; set; }
	}
}


