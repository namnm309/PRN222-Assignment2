using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using BusinessLayer.DTOs.Requests;
using BusinessLayer.Enums;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.UserManagement
{
    public class EditModel : BaseEVMStaffPageModel
    {
        private readonly IAuthenService _authenService;
        private readonly IMappingService _mappingService;

        public EditModel(
            IAuthenService authenService,
            IMappingService mappingService)
        {
            _authenService = authenService;
            _mappingService = mappingService;
        }

        [BindProperty]
        public UserUpdateRequest Input { get; set; } = new();

        public UserResponse? User { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var result = await _authenService.GetUserByIdAsync(id);
            
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng";
                return RedirectToPage("./Index");
            }

            User = _mappingService.MapToUserViewModel(result.Data);
            
            // Map to update request
            Input = new UserUpdateRequest
            {
                Id = User.Id,
                FullName = User.FullName,
                Email = User.Email,
                PhoneNumber = User.PhoneNumber ?? string.Empty,
                Address = User.Address ?? string.Empty,
                IsActive = User.IsActive
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload user data
                var result = await _authenService.GetUserByIdAsync(Input.Id);
                if (result.Success && result.Data != null)
                {
                    User = _mappingService.MapToUserViewModel(result.Data);
                }
                return Page();
            }

            // Get existing user
            var existingResult = await _authenService.GetUserByIdAsync(Input.Id);
            if (!existingResult.Success || existingResult.Data == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng";
                return RedirectToPage("./Index");
            }

            var user = existingResult.Data;

            // Update properties
            user.FullName = Input.FullName;
            user.Email = Input.Email;
            user.PhoneNumber = Input.PhoneNumber;
            user.Address = Input.Address;
            user.IsActive = Input.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // Update user
            var updateResult = await _authenService.UpdateUserAsync(user);

            if (updateResult.Success)
            {
                TempData["Success"] = "Cập nhật thông tin người dùng thành công!";
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError("", updateResult.Error ?? "Không thể cập nhật thông tin");
                
                // Reload user data
                var reloadResult = await _authenService.GetUserByIdAsync(Input.Id);
                if (reloadResult.Success && reloadResult.Data != null)
                {
                    User = _mappingService.MapToUserViewModel(reloadResult.Data);
                }
                return Page();
            }
        }
    }
}
