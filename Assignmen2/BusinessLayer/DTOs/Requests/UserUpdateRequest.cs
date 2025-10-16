using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs.Requests
{
    public class UserUpdateRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string Address { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
