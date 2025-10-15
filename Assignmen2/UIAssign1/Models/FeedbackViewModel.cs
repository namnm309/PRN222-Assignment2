using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models
{
    public class FeedbackViewModel
    {
        [Required] public Guid CustomerId { get; set; }
        [Required] public Guid ProductId { get; set; }
        [Required, StringLength(2000, MinimumLength = 5)]
        public string Comment { get; set; }
        [Range(0, 5)]
        public int Rating { get; set; } = 5;
    }
}
