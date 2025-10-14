using System;

namespace BusinessLayer.DTOs.Responses
{
	public class LoginResponse
	{
		public bool Success { get; set; }
		public string Error { get; set; } = string.Empty;
		public Guid? UserId { get; set; }
		public string? FullName { get; set; }
		public string? Email { get; set; }
	}
}


