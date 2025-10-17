using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.BrandManagement
{
    public class IndexModel : BaseEVMStaffPageModel
    {
        private readonly IBrandService _brandService;
        private readonly IMappingService _mappingService;

        public IndexModel(IBrandService brandService, IMappingService mappingService)
        {
            _brandService = brandService;
            _mappingService = mappingService;
        }

        public List<BrandResponse> Brands { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToPage("/Dashboard/Index");
            }

            var (ok, err, data) = await _brandService.GetAllAsync();
            if (!ok || data == null)
            {
                TempData["Error"] = err ?? "Không thể tải danh sách thương hiệu";
                return Page();
            }

            Brands = _mappingService.MapToBrandViewModels(data);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToPage("/Dashboard/Index");
            }

            var (ok, err) = await _brandService.DeleteAsync(id);
            if (ok)
            {
                TempData["Success"] = "Xóa thương hiệu thành công!";
            }
            else
            {
                TempData["Error"] = err ?? "Không thể xóa thương hiệu";
            }

            return RedirectToPage();
        }
    }
}


