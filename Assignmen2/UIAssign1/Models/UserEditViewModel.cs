using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models
{
    public class UserEditViewModel
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        
        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; } = true;
    }
}