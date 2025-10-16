using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.VehicleQuery
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly IMappingService _mappingService;

        public IndexModel(IProductService productService, IBrandService brandService, IMappingService mappingService)
        {
            _productService = productService;
            _brandService = brandService;
            _mappingService = mappingService;
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

        public List<ProductResponse> Products { get; set; } = new();
        public List<BrandResponse> Brands { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Get all brands for filter dropdown using service
            var brandsResult = await _brandService.GetAllAsync();
            if (brandsResult.Success && brandsResult.Data != null)
            {
                Brands = _mappingService.MapToBrandViewModels(brandsResult.Data);
            }

            // Search products using service
            var productsResult = await _productService.SearchAsync(
                Search, 
                BrandId, 
                MinPrice, 
                MaxPrice, 
                null, // inStock - show all
                true  // isActive - only active products
            );

            if (productsResult.Success && productsResult.Data != null)
            {
                // Map entities to DTOs using mapping service
                Products = _mappingService.MapToProductViewModels(productsResult.Data);
                TotalItems = Products.Count;
                TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);

                // Simple pagination (in real app, you'd implement this in the service layer)
                Products = Products
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
            else
            {
                Products = new List<ProductResponse>();
                TempData["Error"] = productsResult.Error ?? "Không thể tải danh sách sản phẩm";
            }
        }
    }
}
