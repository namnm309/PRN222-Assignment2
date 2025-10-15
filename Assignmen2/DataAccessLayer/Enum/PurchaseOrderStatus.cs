namespace DataAccessLayer.Enum
{
    public enum PurchaseOrderStatus
    {
        Pending = 0,        // Chờ duyệt
        Approved = 1,       // Đã duyệt
        Rejected = 2,       // Bị từ chối
        InTransit = 3,      // Đang vận chuyển
        Delivered = 4,      // Đã giao hàng
        Cancelled = 5       // Đã hủy
    }
}
