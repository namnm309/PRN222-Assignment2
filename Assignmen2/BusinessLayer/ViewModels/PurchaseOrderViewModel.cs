using System;
using BusinessLayer.Enums;

namespace BusinessLayer.ViewModels
{
    public class PurchaseOrderViewModel
    {
        public Guid Id { get; set; }
        public Guid DealerId { get; set; }
        public Guid ProductId { get; set; }
        public Guid RequestedById { get; set; }
        public Guid? ApprovedById { get; set; }
        
        // Order information
        public string OrderNumber { get; set; } = string.Empty;
        public int RequestedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        
        // Status and dates
        public PurchaseOrderStatus Status { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        
        // Additional info
        public string Reason { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string? RejectReason { get; set; }
        
        // Navigation properties (for display)
        public string DealerName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string RequestedByName { get; set; } = string.Empty;
        public string? ApprovedByName { get; set; }
    }
}
