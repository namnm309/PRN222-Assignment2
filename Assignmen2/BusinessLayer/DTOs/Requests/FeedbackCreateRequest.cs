using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs.Requests
{
	public class FeedbackCreateRequest
	{
		[Required] public Guid CustomerId { get; set; }
		[Required] public Guid ProductId { get; set; }
		[Required, StringLength(2000, MinimumLength = 5)]
		public string Comment { get; set; } = string.Empty;
		[Range(0, 5)]
		public int Rating { get; set; } = 5;
	}
}


