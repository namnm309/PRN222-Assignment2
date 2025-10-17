using System;
using BusinessLayer.Enums;

namespace BusinessLayer.DTOs.Responses
{
	public class TestDriveResponse
	{
		public Guid Id { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public string CustomerName { get; set; } = string.Empty;
		public string CustomerPhone { get; set; } = string.Empty;
		public string CustomerEmail { get; set; } = string.Empty;
		public string? Notes { get; set; }
		public Guid ProductId { get; set; }
		public Guid DealerId { get; set; }
		public DateTime ScheduledDate { get; set; }
		public Guid? CustomerId { get; set; }
		public TestDriveStatus Status { get; set; }
		public string? ProductName { get; set; }
		public string? DealerName { get; set; }
		public string? CustomerFullName { get; set; }
		public string? CustomerPhoneNumber { get; set; }
		public string? CustomerEmailAddress { get; set; }
		
		// Navigation properties for compatibility
		public ProductResponse? Product { get; set; }
		public CustomerResponse? Customer { get; set; }
	}
}


