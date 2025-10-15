using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models
{
    public class BrandViewModel
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Tên thương hiệu không được để trống")]
        [Display(Name = "Tên thương hiệu")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Quốc gia không được để trống")]
        [Display(Name = "Quốc gia")]
        public string Country { get; set; }
        
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
