using System;

namespace PresentationLayer.Models
{
    public class HomeProductViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string BrandName { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
    }
}


