using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.ProductManagement
{
    public class IndexModel : BaseEVMStaffPageModel
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly IMappingService _mappingService;

        public IndexModel(
            IProductService productService,
            IBrandService brandService,
            IMappingService mappingService)
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
        public bool? InStock { get; set; }

        public List<ProductResponse> Products { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Load brands for filter
            var brandsResult = await _brandService.GetAllAsync();
            if (brandsResult.Success && brandsResult.Data != null)
            {
                Brands = brandsResult.Data;
            }

            // Search products
            var productsResult = await _productService.SearchAsync(
                Search,
                BrandId,
                MinPrice,
                MaxPrice,
                InStock,
                null // isActive - show all (active and inactive)
            );

            if (productsResult.Success && productsResult.Data != null)
            {
                // Map entities to DTOs
                Products = _mappingService.MapToProductViewModels(productsResult.Data);
            }
            else
            {
                TempData["Error"] = productsResult.Error ?? "Có lỗi khi tải danh sách sản phẩm";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStockAsync(Guid id, int stockQuantity)
        {
            var result = await _productService.UpdateStockAsync(id, stockQuantity);

            if (result.Success)
            {
                TempData["Success"] = "Cập nhật tồn kho thành công!";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Không thể cập nhật tồn kho";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var result = await _productService.DeleteAsync(id);

            if (result.Success)
            {
                TempData["Success"] = "Xóa sản phẩm thành công!";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Không thể xóa sản phẩm";
            }

            return RedirectToPage();
        }
    }
}

