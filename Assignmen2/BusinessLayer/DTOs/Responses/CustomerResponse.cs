using System;

namespace BusinessLayer.DTOs.Responses
{
	public class CustomerResponse
	{
		public Guid Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public bool IsActive { get; set; } = true;
	}
}


