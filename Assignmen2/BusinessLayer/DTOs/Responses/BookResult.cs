using System;

namespace BusinessLayer.DTOs.Responses
{
    public class BookResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public Guid? TestDriveId { get; set; }
        public DateTime? ScheduledDate { get; set; }
    }
}
