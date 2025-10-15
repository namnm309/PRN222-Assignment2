using System;
using DataAccessLayer.Enum;

namespace DataAccessLayer.Entities
{
    public class PurchaseOrder : BaseEntity
    {
        // Foreign keys
        public Guid DealerId { get; set; }
        public Guid ProductId { get; set; }
        public Guid RequestedById { get; set; } // Nhân viên tạo đơn
        public Guid? ApprovedById { get; set; } // EVM/Admin duyệt đơn
        
        // Navigation properties
        public virtual Dealer Dealer { get; set; }
        public virtual Product Product { get; set; }
        public virtual Users RequestedBy { get; set; }
        public virtual Users? ApprovedBy { get; set; }
        
        // Order information
        public string OrderNumber { get; set; } = string.Empty;
        public int RequestedQuantity { get; set; }
        public decimal UnitPrice { get; set; } // Giá nhập từ hãng
        public decimal TotalAmount { get; set; }
        
        // Status and dates
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Pending;
        public DateTime RequestedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        
        // Additional info
        public string Reason { get; set; } = string.Empty; // Lý do đặt hàng
        public string Notes { get; set; } = string.Empty;
        public string? RejectReason { get; set; } // Lý do từ chối (nếu có)
    }
}
