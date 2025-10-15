using System;

namespace BusinessLayer.DTOs.Responses
{
	public class RegionResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Code { get; set; } = string.Empty;
		public bool IsActive { get; set; }
	}
}


