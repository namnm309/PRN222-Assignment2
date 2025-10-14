using System;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.Enums;

namespace BusinessLayer.DTOs.Requests
{
	public class TestDriveCreateRequest
	{
		[Required]
		public string CustomerName { get; set; } = string.Empty;
		[Required]
		[Phone]
		public string CustomerPhone { get; set; } = string.Empty;
		[Required]
		[EmailAddress]
		public string CustomerEmail { get; set; } = string.Empty;
		public string? Notes { get; set; }
		[Required]
		public Guid ProductId { get; set; }
		[Required]
		public Guid DealerId { get; set; }
		public DateTime ScheduledDate { get; set; }
		public Guid? CustomerId { get; set; }
	}
}


