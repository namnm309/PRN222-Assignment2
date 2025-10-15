using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs.Requests
{
	public class UserEditRequest
	{
		public Guid Id { get; set; }
		[Required]
		public string FullName { get; set; } = string.Empty;
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;
		[Required]
		[Phone]
		public string PhoneNumber { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public bool IsActive { get; set; }
	}
}


