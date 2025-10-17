using System;

namespace BusinessLayer.DTOs.Responses
{
	public class CustomerResponse
	{
		public Guid Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string? Name { get; set; } // Alias for FullName
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public bool IsActive { get; set; } = true;
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}


