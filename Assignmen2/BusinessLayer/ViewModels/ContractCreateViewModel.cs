using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.ViewModels
{
    public class ContractCreateViewModel
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số hợp đồng")]
        [Display(Name = "Số hợp đồng")]
        [MaxLength(50)]
        public string ContractNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập điều khoản")]
        [Display(Name = "Điều khoản hợp đồng")]
        [MaxLength(2000)]
        public string Terms { get; set; } = string.Empty;

        [Display(Name = "Ghi chú")]
        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;
    }
}
