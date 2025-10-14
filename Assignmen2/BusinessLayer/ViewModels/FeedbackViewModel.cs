using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.ViewModels
{
    public class FeedbackViewModel
    {
        public Guid Id { get; set; }
        [Required] public Guid CustomerId { get; set; }
        [Required] public Guid ProductId { get; set; }
        [Required, StringLength(2000, MinimumLength = 5)]
        public string Comment { get; set; } = string.Empty;
        [Range(0, 5)]
        public int Rating { get; set; } = 5;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties for display
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ReplyMessage { get; set; } = string.Empty;
        public DateTime? RepliedAt { get; set; }
        public string RepliedByName { get; set; } = string.Empty;
    }
}
