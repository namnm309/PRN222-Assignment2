namespace DataAccessLayer.Enum
{
    public enum TestDriveStatus
    {
        Pending = 0,       // Vừa tạo, chờ xử lý
        Confirmed = 1,     // Đại lý đã xác nhận
        Successfully = 2,  // Lái thử thành công
        Failed = 3,        // Lái thử thất bại
        Canceled = 4       // Người dùng hoặc đại lý hủy
    }
}
