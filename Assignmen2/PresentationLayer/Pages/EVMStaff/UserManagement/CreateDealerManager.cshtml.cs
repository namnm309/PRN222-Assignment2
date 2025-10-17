using BusinessLayer.DTOs.Requests;
using BusinessLayer.Enums;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.UserManagement
{
    public class CreateDealerManagerModel : BaseEVMStaffPageModel
    {
        private readonly IAuthenService _authenService;
        private readonly IEVMReportService _evmService;

        public CreateDealerManagerModel(IAuthenService authenService, IEVMReportService evmService)
        {
            _authenService = authenService;
            _evmService = evmService;
        }

        [BindProperty]
        public string FullName { get; set; } = string.Empty;
        [BindProperty]
        public string Email { get; set; } = string.Empty;
        [BindProperty]
        public string PhoneNumber { get; set; } = string.Empty;
        [BindProperty]
        public string Address { get; set; } = string.Empty;
        [BindProperty]
        public Guid? DealerId { get; set; }

        public List<SelectListItem> DealerOptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Chỉ Admin/EVMStaff mới có quyền tạo Dealer Manager.";
                return RedirectToPage("/Dashboard/Index");
            }

            await LoadDealersAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Chỉ Admin/EVMStaff mới có quyền tạo Dealer Manager.";
                return RedirectToPage("/Dashboard/Index");
            }

            if (!ModelState.IsValid)
            {
                await LoadDealersAsync();
                return Page();
            }

            // check email tồn tại
            var (existing, error) = await _authenService.GetUserByEmailAsync(Email);
            if (existing != null)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                await LoadDealersAsync();
                return Page();
            }

            // Generate default initial password (e.g., DealerManager@123 or based on email)
            var initialPassword = GenerateInitialPassword();

            var (success, registerError, user) = await _authenService.RegisterAsync(
                FullName,
                Email,
                initialPassword,
                PhoneNumber,
                Address,
                UserRole.DealerManager,
                DealerId
            );

            if (!success)
            {
                TempData["Error"] = registerError ?? "Không thể tạo Dealer Manager.";
                await LoadDealersAsync();
                return Page();
            }

            TempData["Success"] = $"Tạo tài khoản Dealer Manager thành công! Email: {Email}. Mật khẩu tạm thời đã được khởi tạo.";
            return RedirectToPage("/EVMStaff/UserManagement/Index");
        }

        private async Task LoadDealersAsync()
        {
            var dealers = await _evmService.GetAllDealersAsync();
            DealerOptions = dealers.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.Name} ({d.DealerCode})"
            }).ToList();
        }

        private string GenerateInitialPassword()
        {
            // Simple default password policy; can be replaced by stronger logic or email notification flow
            return "DealerManager@123";
        }
    }
}


