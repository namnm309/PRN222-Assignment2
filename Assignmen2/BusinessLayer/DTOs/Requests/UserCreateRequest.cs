using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs.Requests
{
	public class UserCreateRequest
	{
		[Required]
		public string FullName { get; set; } = string.Empty;
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;
		[Required]
		[StringLength(100, MinimumLength = 6)]
		public string Password { get; set; } = string.Empty;
		[Required]
		[Compare("Password")]
		public string ConfirmPassword { get; set; } = string.Empty;
		[Required]
		[Phone]
		public string PhoneNumber { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public Guid? DealerId { get; set; }
	}
}


