using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    public class ProductManagementController : BaseDashboardController
    {
        private readonly IProductService _productService;
        private readonly IEVMReportService _evmService;
        private readonly IBrandService _brandService;
        private readonly IMappingService _mappingService;

        public ProductManagementController(IProductService productService,
            IEVMReportService evmService
            , IBrandService brandService,
            IMappingService mappingService)
        {
            _productService = productService;
            _evmService = evmService;
            _brandService = brandService;
            _mappingService = mappingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q = null, Guid? brandId = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err, products) = await _productService.SearchAsync(q, brandId, null, null, null, null);
            if (!ok)
            {
                TempData["Error"] = err;
            }

            // Map entities to ViewModels
            var productViewModels = products != null ? _mappingService.MapToProductViewModels(products) : new List<ProductViewModel>();

            ViewBag.Brands = await _evmService.GetAllBrandsAsync();
            ViewBag.SearchQuery = q;
            ViewBag.SelectedBrandId = brandId;

            return View(productViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(Guid id, int stockQuantity)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err) = await _productService.UpdateStockAsync(id, stockQuantity);
            if (!ok)
            {
                TempData["Error"] = err;
            }
            else
            {
                TempData["Success"] = "Cập nhật tồn kho thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            await LoadBrandsToViewBag();
            return View(new ProductCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                await LoadBrandsToViewBag();
                return View(model);
            }

            // Xử lý upload hình ảnh
            string? imageUrl = null;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                imageUrl = await SaveImageAsync(model.ImageFile);
                if (imageUrl == null)
                {
                    ModelState.AddModelError("ImageFile", "Lỗi khi upload hình ảnh");
                    await LoadBrandsToViewBag();
                    return View(model);
                }
            }
            else if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                imageUrl = model.ImageUrl;
            }

            // Map view model to entity using AutoMapper
            var product = _mappingService.MapToProduct(model);
            product.ImageUrl = imageUrl;

            var (ok, err) = await _productService.CreateAsync(product);
            if (!ok)
            {
                ModelState.AddModelError("", err);
                await LoadBrandsToViewBag();
                return View(model);
            }

            TempData["Success"] = "Tạo sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err, product) = await _productService.GetAsync(id);
            if (!ok)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(Index));
            }

            var model = new ProductEditViewModel
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                BrandId = product.BrandId,
                CurrentImageUrl = product.ImageUrl,
                ImageUrl = product.ImageUrl
            };

            await LoadBrandsToViewBag();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                await LoadBrandsToViewBag();
                return View(model);
            }

            // Xử lý upload hình ảnh
            string? imageUrl = model.CurrentImageUrl; // Giữ hình cũ nếu không upload mới
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var newImageUrl = await SaveImageAsync(model.ImageFile);
                if (newImageUrl == null)
                {
                    ModelState.AddModelError("ImageFile", "Lỗi khi upload hình ảnh");
                    await LoadBrandsToViewBag();
                    return View(model);
                }
                
                // Xóa hình cũ nếu có
                if (!string.IsNullOrWhiteSpace(model.CurrentImageUrl))
                {
                    DeleteImage(model.CurrentImageUrl);
                }
                
                imageUrl = newImageUrl;
            }
            else if (!string.IsNullOrWhiteSpace(model.ImageUrl) && model.ImageUrl != model.CurrentImageUrl)
            {
                imageUrl = model.ImageUrl;
            }

            // Map view model to entity using AutoMapper
            var product = _mappingService.MapToProduct(model);
            product.ImageUrl = imageUrl;

            var (ok, err) = await _productService.UpdateAsync(product);
            if (!ok)
            {
                ModelState.AddModelError("", err);
                await LoadBrandsToViewBag();
                return View(model);
            }

            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err) = await _productService.DeleteAsync(id);
            if (!ok)
            {
                TempData["Error"] = err;
            }
            else
            {
                TempData["Success"] = "Xóa sản phẩm thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadBrandsToViewBag()
        {
            var (ok, err, brands) = await _brandService.GetAllAsync();
            ViewBag.Brands = ok ? brands : new List<DataAccessLayer.Entities.Brand>();
        }

        private async Task<string?> SaveImageAsync(IFormFile imageFile)
        {
            try
            {
                // Kiểm tra định dạng file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                {
                    return null;
                }

                // Kiểm tra kích thước file (tối đa 5MB)
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    return null;
                }

                // Tạo tên file unique
                var fileName = $"{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                
                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Trả về relative path
                return $"/images/products/{fileName}";
            }
            catch
            {
                return null;
            }
        }

        private void DeleteImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl) || !imageUrl.StartsWith("/images/products/"))
                    return;

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch
            {
                // Log error if needed but don't throw
            }
        }
    }
}

