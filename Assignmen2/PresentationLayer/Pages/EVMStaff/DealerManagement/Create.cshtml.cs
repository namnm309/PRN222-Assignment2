using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.DealerManagement
{
    public class CreateModel : BaseEVMStaffPageModel
    {
        private readonly IDealerService _dealerService;
        private readonly IEVMReportService _evmService;

        public CreateModel(IDealerService dealerService, IEVMReportService evmService)
        {
            _dealerService = dealerService;
            _evmService = evmService;
        }

        [BindProperty]
        public string Name { get; set; } = string.Empty;
        [BindProperty]
        public string Phone { get; set; } = string.Empty;
        [BindProperty]
        public string Address { get; set; } = string.Empty;
        [BindProperty]
        public string City { get; set; } = string.Empty;
        [BindProperty]
        public string Province { get; set; } = string.Empty;
        [BindProperty]
        public Guid? RegionId { get; set; }
        [BindProperty]
        public string DealerCode { get; set; } = string.Empty;
        [BindProperty]
        public string ContactPerson { get; set; } = string.Empty;
        [BindProperty]
        public string Email { get; set; } = string.Empty;
        [BindProperty]
        public string LicenseNumber { get; set; } = string.Empty;
        [BindProperty]
        public decimal CreditLimit { get; set; }

        public List<SelectListItem> RegionOptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToPage("/Dashboard/Index");
            }

            await LoadRegionsAsync();
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
                await LoadRegionsAsync();
                return Page();
            }

            var (ok, err, dealer) = await _dealerService.CreateAsync(
                Name, Phone, Address, City, Province, RegionId, DealerCode, ContactPerson, Email, LicenseNumber, CreditLimit
            );

            if (!ok)
            {
                TempData["Error"] = err ?? "Không thể tạo đại lý";
                await LoadRegionsAsync();
                return Page();
            }

            TempData["Success"] = "Tạo đại lý thành công!";
            return RedirectToPage("/EVMStaff/DealerManagement/Index");
        }

        private async Task LoadRegionsAsync()
        {
            var regions = await _evmService.GetAllRegionsAsync();
            RegionOptions = regions
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
                .ToList();
        }
    }
}


