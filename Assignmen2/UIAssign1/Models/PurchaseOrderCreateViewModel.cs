using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models
{
    public class PurchaseOrderCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        [Display(Name = "Sản phẩm")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Display(Name = "Số lượng")]
        [Range(1, 1000, ErrorMessage = "Số lượng phải từ 1 đến 1000")]
        public int RequestedQuantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đơn giá")]
        [Display(Name = "Đơn giá (VNĐ)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn 0")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do đặt hàng")]
        [Display(Name = "Lý do đặt hàng")]
        [MaxLength(500, ErrorMessage = "Lý do đặt hàng không được vượt quá 500 ký tự")]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Ghi chú")]
        [MaxLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
        public string Notes { get; set; } = string.Empty;

        [Display(Name = "Ngày giao dự kiến")]
        [DataType(DataType.Date)]
        public DateTime? ExpectedDeliveryDate { get; set; }

        // Computed property
        public decimal TotalAmount => RequestedQuantity * UnitPrice;
    }
}
