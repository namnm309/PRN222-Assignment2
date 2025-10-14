using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.ViewModels
{
    public class BrandViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
