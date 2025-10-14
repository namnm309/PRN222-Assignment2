using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.ViewModels;
using BusinessLayer.Services;

namespace PresentationLayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEVMReportService _evmService;

        public HomeController(ILogger<HomeController> logger, IEVMReportService evmService)
        {
            _logger = logger;
            _evmService = evmService;
        }

        public async Task<IActionResult> Index(string search)
        {
            var productsAll = await _evmService.GetAllProductsAsync();
            var query = productsAll.Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Sku.ToLower().Contains(term) ||
                    p.Brand.Name.ToLower().Contains(term));
            }

            var products = query
                .OrderBy(p => p.Name)
                .Take(5)
                .Select(p => new HomeProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    Description = p.Description,
                    Price = p.Price,
                    BrandName = p.Brand?.Name ?? string.Empty,
                    IsActive = p.IsActive,
                    ImageUrl = p.ImageUrl
                })
                .ToList();

            return View(products);
        }

        public async Task<IActionResult> All(string search)
        {
            var productsAll = await _evmService.GetAllProductsAsync();
            var query = productsAll.Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Sku.ToLower().Contains(term) ||
                    p.Brand.Name.ToLower().Contains(term));
            }

            var products = query
                .OrderBy(p => p.Name)
                .Select(p => new HomeProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    Description = p.Description,
                    Price = p.Price,
                    BrandName = p.Brand?.Name ?? string.Empty,
                    IsActive = p.IsActive,
                    ImageUrl = p.ImageUrl
                })
                .ToList();

            return View(products);
        }

        public async Task<IActionResult> TestDrive()
        {
            // Lấy tất cả sản phẩm active để khách hàng chọn lái thử
            var productsAll = await _evmService.GetAllProductsAsync();
            var products = productsAll
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .Select(p => new HomeProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    Description = p.Description,
                    Price = p.Price,
                    BrandName = p.Brand?.Name ?? string.Empty,
                    IsActive = p.IsActive,
                    ImageUrl = p.ImageUrl
                })
                .ToList();


            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
