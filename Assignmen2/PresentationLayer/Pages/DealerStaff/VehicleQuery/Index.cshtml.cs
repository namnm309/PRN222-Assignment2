using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.VehicleQuery
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;

        public IndexModel(IProductService productService, IBrandService brandService)
        {
            _productService = productService;
            _brandService = brandService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? BrandId { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 12;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public List<Product> Products { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Get all brands for filter dropdown
            var brandsResult = await _brandService.GetAllAsync();
            if (brandsResult.Success)
            {
                Brands = brandsResult.Data;
            }

            // Search products
            var productsResult = await _productService.SearchAsync(
                Search, 
                BrandId, 
                MinPrice, 
                MaxPrice, 
                null, // inStock - show all
                true  // isActive - only active products
            );

            if (productsResult.Success)
            {
                Products = productsResult.Data;
                TotalItems = Products.Count;
                TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);

                // Simple pagination (in real app, you'd implement this in the service layer)
                Products = Products
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
        }
    }
}
