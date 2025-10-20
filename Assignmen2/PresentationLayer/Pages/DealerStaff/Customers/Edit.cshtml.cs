using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using System.ComponentModel.DataAnnotations;

namespace Assignmen2.PresentationLayer.Pages.DealerStaff.Customers
{
    public class EditModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public EditModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public CustomerResponse? Customer { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Họ và tên là bắt buộc")]
            [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
            [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
            public string PhoneNumber { get; set; } = string.Empty;

            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "Trạng thái là bắt buộc")]
            public bool IsActive { get; set; } = true;

            [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
            public string? Address { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var result = await _customerService.GetAsync(id);
                if (result.Success && result.Data != null)
                {
                    Customer = new CustomerResponse
                    {
                        Id = result.Data.Id,
                        FullName = result.Data.FullName,
                        Email = result.Data.Email,
                        PhoneNumber = result.Data.PhoneNumber,
                        Address = result.Data.Address,
                        IsActive = result.Data.IsActive,
                        CreatedAt = result.Data.CreatedAt,
                        UpdatedAt = result.Data.UpdatedAt
                    };
                    
                    // Pre-fill form
                    Input.FullName = Customer.FullName;
                    Input.PhoneNumber = Customer.PhoneNumber;
                    Input.Email = Customer.Email;
                    Input.IsActive = Customer.IsActive;
                    Input.Address = Customer.Address;
                    
                    return Page();
                }
                else
                {
                    TempData["Error"] = result.Error ?? "Không tìm thấy khách hàng";
                    return RedirectToPage("/DealerStaff/Customers/Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToPage("/DealerStaff/Customers/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                // Reload customer data for display
                var result = await _customerService.GetAsync(id);
                if (result.Success && result.Data != null)
                {
                    Customer = new CustomerResponse
                    {
                        Id = result.Data.Id,
                        FullName = result.Data.FullName,
                        Email = result.Data.Email,
                        PhoneNumber = result.Data.PhoneNumber,
                        Address = result.Data.Address,
                        IsActive = result.Data.IsActive,
                        CreatedAt = result.Data.CreatedAt,
                        UpdatedAt = result.Data.UpdatedAt
                    };
                }
                return Page();
            }

            try
            {
                // Update basic info
                var updateResult = await _customerService.UpdateAsync(id, Input.FullName, Input.Email, Input.PhoneNumber, Input.Address);
                
                if (updateResult.Success)
                {
                    // Update status
                    var statusResult = await _customerService.UpdateStatusAsync(id, Input.IsActive);
                    
                    if (statusResult.Success)
                    {
                        TempData["Success"] = "Cập nhật thông tin khách hàng thành công!";
                        return RedirectToPage("/DealerStaff/Customers/Detail", new { id });
                    }
                    else
                    {
                        TempData["Error"] = statusResult.Error ?? "Không thể cập nhật trạng thái khách hàng";
                    }
                }
                else
                {
                    TempData["Error"] = updateResult.Error ?? "Không thể cập nhật thông tin khách hàng";
                }
                
                // Reload customer data for display
                var result = await _customerService.GetAsync(id);
                if (result.Success && result.Data != null)
                {
                    Customer = new CustomerResponse
                    {
                        Id = result.Data.Id,
                        FullName = result.Data.FullName,
                        Email = result.Data.Email,
                        PhoneNumber = result.Data.PhoneNumber,
                        Address = result.Data.Address,
                        IsActive = result.Data.IsActive,
                        CreatedAt = result.Data.CreatedAt,
                        UpdatedAt = result.Data.UpdatedAt
                    };
                }
                return Page();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                
                // Reload customer data for display
                var result = await _customerService.GetAsync(id);
                if (result.Success && result.Data != null)
                {
                    Customer = new CustomerResponse
                    {
                        Id = result.Data.Id,
                        FullName = result.Data.FullName,
                        Email = result.Data.Email,
                        PhoneNumber = result.Data.PhoneNumber,
                        Address = result.Data.Address,
                        IsActive = result.Data.IsActive,
                        CreatedAt = result.Data.CreatedAt,
                        UpdatedAt = result.Data.UpdatedAt
                    };
                }
                return Page();
            }
        }
    }
}
