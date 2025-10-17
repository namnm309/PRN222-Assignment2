using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.BrandManagement
{
    public class EditModel : BaseEVMStaffPageModel
    {
        private readonly IBrandService _brandService;

        public EditModel(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [BindProperty]
        public Guid Id { get; set; }
        [BindProperty]
        public string Name { get; set; } = string.Empty;
        [BindProperty]
        public string Country { get; set; } = string.Empty;
        [BindProperty]
        public bool IsActive { get; set; } = true;
        [BindProperty]
        public string Description { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToPage("/Dashboard/Index");
            }

            var (ok, err, brand) = await _brandService.GetByIdAsync(id);
            if (!ok || brand == null)
            {
                TempData["Error"] = err ?? "Không tìm thấy thương hiệu";
                return RedirectToPage("/EVMStaff/BrandManagement/Index");
            }

            Id = brand.Id;
            Name = brand.Name;
            Country = brand.Country;
            IsActive = brand.IsActive;
            Description = brand.Description;
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

            var (ok, err) = await _brandService.UpdateAsync(Id, Name, Country, Description, IsActive);
            if (!ok)
            {
                TempData["Error"] = err ?? "Không thể cập nhật thương hiệu";
                return Page();
            }

            TempData["Success"] = "Cập nhật thương hiệu thành công!";
            return RedirectToPage("/EVMStaff/BrandManagement/Index");
        }
    }
}


