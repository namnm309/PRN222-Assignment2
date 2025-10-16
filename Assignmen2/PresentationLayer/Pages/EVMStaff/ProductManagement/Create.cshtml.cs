using BusinessLayer.Services;
using BusinessLayer.DTOs.Requests;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.ProductManagement
{
    public class CreateModel : BaseEVMStaffPageModel
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly IMappingService _mappingService;

        public CreateModel(
            IProductService productService,
            IBrandService brandService,
            IMappingService mappingService)
        {
            _productService = productService;
            _brandService = brandService;
            _mappingService = mappingService;
        }

        [BindProperty]
        public ProductCreateRequest Input { get; set; } = new();

        public List<Brand> Brands { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadBrandsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadBrandsAsync();
                return Page();
            }

            // Check SKU exists
            var skuExists = await _productService.IsSkuExistsAsync(Input.Sku);
            if (skuExists)
            {
                ModelState.AddModelError("Input.Sku", "SKU này đã tồn tại trong hệ thống");
                await LoadBrandsAsync();
                return Page();
            }

            // Map DTO to Entity
            var product = _mappingService.MapToProduct(Input);

            // Handle image upload
            if (Input.ImageFile != null && Input.ImageFile.Length > 0)
            {
                var imageUrl = await SaveImageAsync(Input.ImageFile);
                if (imageUrl != null)
                {
                    product.ImageUrl = imageUrl;
                }
                else
                {
                    ModelState.AddModelError("Input.ImageFile", "Lỗi khi upload hình ảnh");
                    await LoadBrandsAsync();
                    return Page();
                }
            }
            else if (!string.IsNullOrEmpty(Input.ImageUrl))
            {
                product.ImageUrl = Input.ImageUrl;
            }

            // Create product
            var result = await _productService.CreateAsync(product);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error);
                await LoadBrandsAsync();
                return Page();
            }

            TempData["Success"] = "Tạo sản phẩm thành công!";
            return RedirectToPage("./Index");
        }

        private async Task LoadBrandsAsync()
        {
            var brandsResult = await _brandService.GetAllAsync();
            if (brandsResult.Success && brandsResult.Data != null)
            {
                Brands = brandsResult.Data.Where(b => b.IsActive).ToList();
            }
        }

        private async Task<string?> SaveImageAsync(IFormFile imageFile)
        {
            try
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                    return null;

                if (imageFile.Length > 5 * 1024 * 1024) // 5MB
                    return null;

                var fileName = $"{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                return $"/images/products/{fileName}";
            }
            catch
            {
                return null;
            }
        }
    }
}

