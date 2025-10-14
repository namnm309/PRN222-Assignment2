using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs.Requests
{
	public class ContractCreateRequest
	{
		[Required]
		public Guid OrderId { get; set; }
		[Required]
		[MaxLength(50)]
		public string ContractNumber { get; set; } = string.Empty;
		[Required]
		[MaxLength(2000)]
		public string Terms { get; set; } = string.Empty;
		[MaxLength(1000)]
		public string Notes { get; set; } = string.Empty;
	}
}


