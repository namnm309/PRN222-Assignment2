using BusinessLayer.Services;
using DataAccessLayer.Entities;
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

        public Product? Product { get; set; }
        public List<Product> RelatedProducts { get; set; } = new();

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

            Product = result.Data;

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
                        .ToList();
                }
            }

            return Page();
        }
    }
}
