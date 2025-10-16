using System;

namespace BusinessLayer.DTOs.Responses
{
	public class InventoryAllocationResponse
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public Guid DealerId { get; set; }
		public int AllocatedQuantity { get; set; }
		public int AvailableQuantity { get; set; }
		public int ReservedQuantity { get; set; }
		public int ActualQuantity { get; set; }
		public int MinimumStock { get; set; }
		public int MaximumStock { get; set; }
		public DateTime LastRestockDate { get; set; }
		public DateTime AllocationDate { get; set; }
		public DateTime? NextRestockDate { get; set; }
		public string Status { get; set; } = "Active";
		public string Priority { get; set; } = "Normal";
		public string? Notes { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public string DealerName { get; set; } = string.Empty;
		public bool IsActive { get; set; } = true;
	}
}


