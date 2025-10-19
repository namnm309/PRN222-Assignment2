using System;

namespace BusinessLayer.DTOs.Requests
{
    public class BookTestDriveRequest
    {
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string? Notes { get; set; }
        public Guid ProductId { get; set; }
        public Guid DealerId { get; set; }
        public DateTime ScheduledDate { get; set; }
    }
}
