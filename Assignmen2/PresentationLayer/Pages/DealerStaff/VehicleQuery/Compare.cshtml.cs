using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.VehicleQuery
{
    public class CompareModel : PageModel
    {
        private readonly IProductService _productService;

        public CompareModel(IProductService productService)
        {
            _productService = productService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Ids { get; set; }

        public List<ProductResponse> Products { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(Ids))
            {
                return RedirectToPage("/DealerStaff/VehicleQuery/Index");
            }

            var productIds = Ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => Guid.TryParse(id, out var guid) ? guid : Guid.Empty)
                .Where(id => id != Guid.Empty)
                .ToList();

            if (productIds.Count < 2)
            {
                TempData["Error"] = "Cần ít nhất 2 sản phẩm để so sánh";
                return RedirectToPage("/DealerStaff/VehicleQuery/Index");
            }

            if (productIds.Count > 4)
            {
                TempData["Error"] = "Chỉ có thể so sánh tối đa 4 sản phẩm";
                return RedirectToPage("/DealerStaff/VehicleQuery/Index");
            }

            var products = new List<ProductResponse>();

            foreach (var id in productIds)
            {
                var result = await _productService.GetAsync(id);
                if (result.Success && result.Data != null)
                {
                    // Map DAL entity to DTO if needed; here assuming service returns entity, adapt mapping when service updated
                    products.Add(new ProductResponse
                    {
                        Id = result.Data.Id,
                        Name = result.Data.Name,
                        Price = result.Data.Price,
                        StockQuantity = result.Data.StockQuantity,
                        BrandId = result.Data.BrandId
                    });
                }
            }

            Products = products;

            if (Products.Count < 2)
            {
                TempData["Error"] = "Không tìm thấy đủ sản phẩm để so sánh";
                return RedirectToPage("/DealerStaff/VehicleQuery/Index");
            }

            return Page();
        }
    }
}
