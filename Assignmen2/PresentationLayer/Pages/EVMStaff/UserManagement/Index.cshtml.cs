using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using BusinessLayer.Enums;
using BusinessLayer.Helpers;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.UserManagement
{
    public class IndexModel : BaseEVMStaffPageModel
    {
        private readonly IAuthenService _authenService;
        private readonly IMappingService _mappingService;

        public IndexModel(
            IAuthenService authenService,
            IMappingService mappingService)
        {
            _authenService = authenService;
            _mappingService = mappingService;
        }

        public List<UserResponse> Users { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Role { get; set; } // Filter by role

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get all users (dealers only)
            var result = await _authenService.GetAllUsersAsync();
            
            if (result.Success && result.Data != null)
            {
                // Map entities to DTOs
                Users = _mappingService.MapToUserViewModels(result.Data);

                // Filter only dealer roles (DealerStaff and DealerManager)
                Users = Users.Where(u => 
                    u.Role == UserRole.DealerStaff || 
                    u.Role == UserRole.DealerManager).ToList();

                // Apply role filter if specified
                if (!string.IsNullOrEmpty(Role) && Enum.TryParse<UserRole>(Role, out var roleEnum))
                {
                    Users = Users.Where(u => u.Role == roleEnum).ToList();
                }

                // Apply search filter (case-insensitive, diacritics-insensitive)
                // Tìm kiếm theo chữ cái đầu cho tên, contains cho email và đại lý
                if (!string.IsNullOrEmpty(Search))
                {
                    Users = Users.Where(u =>
                        SearchHelper.StartsWithLetter(u.FullName, Search) ||
                        SearchHelper.Contains(u.Email, Search) ||
                        (u.DealerName != null && SearchHelper.StartsWithLetter(u.DealerName, Search))
                    ).ToList();
                }
            }
            else
            {
                TempData["Error"] = result.Error ?? "Không thể tải danh sách tài khoản";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(Guid id, bool isActive)
        {
            var result = await _authenService.ToggleUserStatusAsync(id, isActive);

            if (result.Success)
            {
                TempData["Success"] = $"Đã {(isActive ? "kích hoạt" : "vô hiệu hóa")} tài khoản!";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Không thể thay đổi trạng thái tài khoản";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var result = await _authenService.DeleteUserAsync(id);

            if (result.Success)
            {
                TempData["Success"] = "Xóa tài khoản thành công!";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Không thể xóa tài khoản";
            }

            return RedirectToPage();
        }
    }
}

