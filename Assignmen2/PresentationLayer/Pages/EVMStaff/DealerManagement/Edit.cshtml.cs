using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.DealerManagement
{
    public class EditModel : BaseEVMStaffPageModel
    {
        private readonly IDealerService _dealerService;
        private readonly IEVMReportService _evmService;

        public EditModel(IDealerService dealerService, IEVMReportService evmService)
        {
            _dealerService = dealerService;
            _evmService = evmService;
        }

        [BindProperty]
        public Guid Id { get; set; }
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
        [BindProperty]
        public decimal OutstandingDebt { get; set; }
        [BindProperty]
        public string Status { get; set; } = "Active";
        [BindProperty]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> RegionOptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToPage("/Dashboard/Index");
            }

            var (ok, err, dealer) = await _dealerService.GetByIdAsync(id);
            if (!ok || dealer == null)
            {
                TempData["Error"] = err ?? "Không tìm thấy đại lý";
                return RedirectToPage("/EVMStaff/DealerManagement/Index");
            }

            Id = dealer.Id;
            Name = dealer.Name;
            Phone = dealer.Phone;
            Address = dealer.Address;
            City = dealer.City;
            Province = dealer.Province;
            RegionId = dealer.RegionId;
            DealerCode = dealer.DealerCode;
            ContactPerson = dealer.ContactPerson;
            Email = dealer.Email;
            LicenseNumber = dealer.LicenseNumber;
            CreditLimit = dealer.CreditLimit;
            OutstandingDebt = dealer.OutstandingDebt;
            Status = dealer.Status;
            IsActive = dealer.IsActive;

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

            var (ok, err) = await _dealerService.UpdateAsync(
                Id, Name, Phone, Address, City, Province, RegionId, DealerCode,
                ContactPerson, Email, LicenseNumber, CreditLimit, OutstandingDebt, Status, IsActive
            );

            if (!ok)
            {
                TempData["Error"] = err ?? "Không thể cập nhật đại lý";
                await LoadRegionsAsync();
                return Page();
            }

            TempData["Success"] = "Cập nhật đại lý thành công!";
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


