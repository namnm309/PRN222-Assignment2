using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.Enums;
using BusinessLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    public class BrandController : BaseDashboardController
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err, brands) = await _brandService.GetAllAsync();
            if (!ok)
            {
                TempData["Error"] = err;
                return View(new List<BrandViewModel>());
            }
            return View(brands);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string country, string description)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err, brand) = await _brandService.CreateAsync(name, country, description);
            if (!ok)
            {
                TempData["Error"] = err;
                return View();
            }

            TempData["Success"] = "Tạo thương hiệu thành công!";
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

            var (ok, err, brand) = await _brandService.GetByIdAsync(id);
            if (!ok)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, string name, string country, string description, bool isActive)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Validation
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Tên thương hiệu không được để trống.";
                var (_, __, brand) = await _brandService.GetByIdAsync(id);
                return View(brand);
            }

            var (ok, err) = await _brandService.UpdateAsync(id, name, country, description, isActive);
            if (!ok)
            {
                TempData["Error"] = err;
                var (_, __, brand) = await _brandService.GetByIdAsync(id);
                return View(brand);
            }

            TempData["Success"] = "Cập nhật thương hiệu thành công!";
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

            var (ok, err) = await _brandService.DeleteAsync(id);
            if (!ok)
            {
                TempData["Error"] = err;
            }
            else
            {
                TempData["Success"] = "Xóa thương hiệu thành công!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

