using BusinessLayer.Services;
using BusinessLayer.DTOs.Requests;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.ProductManagement
{
    public class EditModel : BaseEVMStaffPageModel
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly IMappingService _mappingService;

        public EditModel(
            IProductService productService,
            IBrandService brandService,
            IMappingService mappingService)
        {
            _productService = productService;
            _brandService = brandService;
            _mappingService = mappingService;
        }

        [BindProperty]
        public ProductUpdateRequest Input { get; set; } = new();

        public List<BrandResponse> Brands { get; set; } = new();
        public string? CurrentImageUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm";
                return RedirectToPage("./Index");
            }

            var product = result.Data;
            
            // Map entity to DTO
            Input = new ProductUpdateRequest
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Description = product.Description ?? string.Empty,
                BrandId = product.BrandId,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl ?? string.Empty,
                IsActive = product.IsActive
            };

            CurrentImageUrl = product.ImageUrl;
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

            // Get existing product
            var existingResult = await _productService.GetByIdAsync(Input.Id);
            if (!existingResult.Success || existingResult.Data == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm";
                return RedirectToPage("./Index");
            }

            var product = existingResult.Data;

            // Update properties
            product.Name = Input.Name;
            product.Sku = Input.Sku;
            product.Description = Input.Description;
            product.BrandId = Input.BrandId;
            product.Price = Input.Price;
            product.StockQuantity = Input.StockQuantity;
            product.IsActive = Input.IsActive;

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

            // Update product
            var result = await _productService.UpdateAsync(product);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error);
                await LoadBrandsAsync();
                return Page();
            }

            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToPage("./Index");
        }

        private async Task LoadBrandsAsync()
        {
            var brandsResult = await _brandService.GetAllAsync();
            if (brandsResult.Success && brandsResult.Data != null)
            {
                Brands = _mappingService.MapToBrandViewModels(brandsResult.Data.Where(b => b.IsActive).ToList());
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

