using System;
namespace PresentationLayer.Models
{
    public class ProductViewModel
    {
        public string? Q { get; set; }
        public Guid? BrandId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStock { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
