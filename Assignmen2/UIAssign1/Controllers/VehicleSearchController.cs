using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.ViewModels;
using BusinessLayer.Enums;

namespace PresentationLayer.Controllers
{
    public class VehicleSearchController : BaseDashboardController
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly IEVMReportService _evmService;
        private readonly IMappingService _mappingService;

        public VehicleSearchController(
            IProductService productService, 
            IBrandService brandService,
            IEVMReportService evmService,
            IMappingService mappingService)
        {
            _productService = productService;
            _brandService = brandService;
            _evmService = evmService;
            _mappingService = mappingService;
        }

        // GET: VehicleSearch - Trang tra cứu xe cho Dealer Staff
        [HttpGet]
        public async Task<IActionResult> Index(ProductViewModel searchModel)
        {
            // Chỉ Dealer Staff và Dealer Manager mới được truy cập
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Load danh sách brand cho dropdown
            var (brandsOk, _, brands) = await _brandService.GetAllAsync();
            ViewBag.Brands = brandsOk ? brands : new List<DataAccessLayer.Entities.Brand>();

            // Nếu không có tham số tìm kiếm, hiển thị tất cả sản phẩm
            if (string.IsNullOrEmpty(searchModel.Q) && 
                searchModel.BrandId == Guid.Empty && 
                !searchModel.MinPrice.HasValue && 
                !searchModel.MaxPrice.HasValue && 
                !searchModel.InStock.HasValue)
            {
                // Hiển thị tất cả sản phẩm active
                var (allSuccess, allError, allProducts) = await _productService.SearchAsync(
                    null, null, null, null, null, true
                );
                
                if (allSuccess)
                {
                    var allProductViewModels = _mappingService.MapToProductViewModels(allProducts);
                    ViewBag.SearchModel = searchModel;
                    return View(allProductViewModels);
                }
            }

            // Thực hiện tìm kiếm với tham số
            var (success, error, products) = await _productService.SearchAsync(
                searchModel.Q, 
                searchModel.BrandId, 
                searchModel.MinPrice, 
                searchModel.MaxPrice, 
                searchModel.InStock, 
                searchModel.IsActive
            );

            if (!success)
            {
                TempData["Error"] = error;
                return View(new List<ProductViewModel>());
            }

            // Debug: Log số lượng sản phẩm tìm được
            System.Diagnostics.Debug.WriteLine($"Tìm thấy {products?.Count ?? 0} sản phẩm");

            // Map entities to view models
            var productViewModels = _mappingService.MapToProductViewModels(products);
            
            // Truyền search model để giữ lại giá trị tìm kiếm
            ViewBag.SearchModel = searchModel;
            
            return View(productViewModels);
        }

        // GET: VehicleSearch/Detail/{id} - Chi tiết xe
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (success, error, product) = await _productService.GetAsync(id);
            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }

            // Map entity to view model
            var productViewModel = _mappingService.MapToProductViewModel(product);
            
            return View(productViewModel);
        }

        // GET: VehicleSearch/QuickSearch - Tìm kiếm nhanh (AJAX)
        [HttpGet]
        public async Task<IActionResult> QuickSearch(string query)
        {
            if (!IsDealer())
            {
                return Json(new { success = false, message = "Không có quyền truy cập" });
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { success = false, message = "Vui lòng nhập từ khóa tìm kiếm" });
            }

            var (success, error, products) = await _productService.SearchAsync(
                query, null, null, null, null, true
            );

            if (!success)
            {
                return Json(new { success = false, message = error });
            }

            // Trả về kết quả tìm kiếm dạng JSON cho AJAX
            var results = products.Take(10).Select(p => new
            {
                id = p.Id,
                name = p.Name,
                sku = p.Sku,
                brandName = p.Brand?.Name ?? "",
                price = p.Price,
                stockQuantity = p.StockQuantity,
                imageUrl = p.ImageUrl
            }).ToList();

            return Json(new { success = true, data = results });
        }

        // GET: VehicleSearch/ByBrand/{brandId} - Tìm kiếm theo thương hiệu
        [HttpGet]
        public async Task<IActionResult> ByBrand(Guid brandId)
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (success, error, products) = await _productService.SearchAsync(
                null, brandId, null, null, null, true
            );

            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }

            // Map entities to view models
            var productViewModels = _mappingService.MapToProductViewModels(products);
            
            // Load brand info
            var (brandOk, _, brand) = await _brandService.GetByIdAsync(brandId);
            ViewBag.BrandName = brandOk ? brand.Name : "Thương hiệu";
            
            return View("Index", productViewModels);
        }

        // GET: VehicleSearch/InStock - Xe còn hàng
        [HttpGet]
        public async Task<IActionResult> InStock()
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (success, error, products) = await _productService.SearchAsync(
                null, null, null, null, true, true
            );

            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }

            // Map entities to view models
            var productViewModels = _mappingService.MapToProductViewModels(products);
            
            return View("Index", productViewModels);
        }
    }
}
