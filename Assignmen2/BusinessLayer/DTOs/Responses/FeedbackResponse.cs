using System;

namespace BusinessLayer.DTOs.Responses
{
	public class FeedbackResponse
	{
		public Guid Id { get; set; }
		public Guid CustomerId { get; set; }
		public Guid ProductId { get; set; }
		public string Comment { get; set; } = string.Empty;
		public int Rating { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public string CustomerName { get; set; } = string.Empty;
		public string CustomerEmail { get; set; } = string.Empty;
		public string CustomerPhone { get; set; } = string.Empty;
		public string ProductName { get; set; } = string.Empty;
		public string ReplyMessage { get; set; } = string.Empty;
		public DateTime? RepliedAt { get; set; }
		public string RepliedByName { get; set; } = string.Empty;
	}
}


