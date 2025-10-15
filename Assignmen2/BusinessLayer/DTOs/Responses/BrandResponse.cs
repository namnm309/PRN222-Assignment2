using System;

namespace BusinessLayer.DTOs.Responses
{
	public class BrandResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Country { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public bool IsActive { get; set; } = true;
	}
}


