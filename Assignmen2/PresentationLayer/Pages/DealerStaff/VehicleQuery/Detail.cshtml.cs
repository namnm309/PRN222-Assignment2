using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.VehicleQuery
{
    public class DetailModel : PageModel
    {
        private readonly IProductService _productService;

        public DetailModel(IProductService productService)
        {
            _productService = productService;
        }

        public ProductResponse? Product { get; set; }
        public List<ProductResponse> RelatedProducts { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var result = await _productService.GetAsync(id);
            if (!result.Success || result.Data == null)
            {
                return NotFound();
            }

            // Map entity to DTO response
            Product = new ProductResponse
            {
                Id = result.Data.Id,
                Sku = result.Data.Sku,
                Name = result.Data.Name,
                Description = result.Data.Description,
                Price = result.Data.Price,
                StockQuantity = result.Data.StockQuantity,
                IsActive = result.Data.IsActive,
                ImageUrl = result.Data.ImageUrl,
                BrandId = result.Data.BrandId,
                BrandName = result.Data.Brand != null ? result.Data.Brand.Name : string.Empty,
                CreatedAt = result.Data.CreatedAt,
                UpdatedAt = result.Data.UpdatedAt
            };

            // Get related products (same brand)
            if (Product.BrandId != Guid.Empty)
            {
                var relatedResult = await _productService.SearchAsync(
                    null, 
                    Product.BrandId, 
                    null, 
                    null, 
                    null, 
                    true
                );

                if (relatedResult.Success)
                {
                    RelatedProducts = relatedResult.Data
                        .Where(p => p.Id != Product.Id)
                        .Take(4)
                        .Select(p => new ProductResponse
                        {
                            Id = p.Id,
                            Sku = p.Sku,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.Price,
                            StockQuantity = p.StockQuantity,
                            IsActive = p.IsActive,
                            ImageUrl = p.ImageUrl,
                            BrandId = p.BrandId,
                            BrandName = p.Brand != null ? p.Brand.Name : string.Empty,
                            CreatedAt = p.CreatedAt,
                            UpdatedAt = p.UpdatedAt
                        })
                        .ToList();
                }
            }

            return Page();
        }
    }
}
