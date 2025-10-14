using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.ViewModels
{
    public class OrderCreateViewModel
    {
        // Basic properties
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        [Display(Name = "Sản phẩm")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        [Display(Name = "Khách hàng")]
        public Guid CustomerId { get; set; }

        [Display(Name = "Đại lý")]
        public Guid? DealerId { get; set; }

        [Display(Name = "Nhân viên bán hàng")]
        public Guid? SalesPersonId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Display(Name = "Giá bán (VNĐ)")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Display(Name = "Giảm giá (VNĐ)")]
        [Range(0, double.MaxValue, ErrorMessage = "Giảm giá không hợp lệ")]
        public decimal Discount { get; set; }

        [Display(Name = "Mô tả")]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Ghi chú")]
        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;

        // Additional properties for display
        public string OrderNumber { get; set; } = string.Empty;
        public decimal FinalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? PaymentDueDate { get; set; }

        // Navigation properties for display
        public string? CustomerName { get; set; }
        public string? ProductName { get; set; }
        public string? DealerName { get; set; }
        public string? SalesPersonName { get; set; }
    }
}
