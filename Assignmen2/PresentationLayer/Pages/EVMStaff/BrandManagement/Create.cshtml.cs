using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.BrandManagement
{
    public class CreateModel : BaseEVMStaffPageModel
    {
        private readonly IBrandService _brandService;

        public CreateModel(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [BindProperty]
        public string Name { get; set; } = string.Empty;
        [BindProperty]
        public string Country { get; set; } = string.Empty;
        [BindProperty]
        public bool IsActive { get; set; } = true;
        [BindProperty]
        public string Description { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToPage("/Dashboard/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToPage("/Dashboard/Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var (ok, err, brand) = await _brandService.CreateAsync(Name, Country, Description);
            if (!ok)
            {
                TempData["Error"] = err ?? "Không thể tạo thương hiệu";
                return Page();
            }

            // Nếu service chưa nhận IsActive, có thể cập nhật tiếp nếu cần
            if (!IsActive)
            {
                await _brandService.UpdateAsync(brand.Id, brand.Name, brand.Country, brand.Description, IsActive);
            }

            TempData["Success"] = "Tạo thương hiệu thành công!";
            return RedirectToPage("/EVMStaff/BrandManagement/Index");
        }
    }
}


