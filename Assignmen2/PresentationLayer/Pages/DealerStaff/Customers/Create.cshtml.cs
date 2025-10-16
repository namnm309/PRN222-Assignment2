using BusinessLayer.Services;
using BusinessLayer.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Pages.DealerStaff.Customers
{
    public class CreateModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public CreateModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Họ và tên là bắt buộc")]
            [Display(Name = "Họ và tên")]
            public string FullName { get; set; } = string.Empty;

            [Display(Name = "Tên gọi")]
            public string? Name { get; set; }

            [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
            [Display(Name = "Số điện thoại")]
            public string? PhoneNumber { get; set; }

            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Display(Name = "Địa chỉ")]
            public string? Address { get; set; }

            [Display(Name = "Ghi chú")]
            public string? Notes { get; set; }
        }

        public void OnGet()
        {
            // Initialize form
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Custom validation: at least one contact method required
            if (string.IsNullOrWhiteSpace(Input.PhoneNumber) && string.IsNullOrWhiteSpace(Input.Email))
            {
                ModelState.AddModelError("Input.PhoneNumber", "Phải có ít nhất một phương thức liên hệ (số điện thoại hoặc email)");
                ModelState.AddModelError("Input.Email", "Phải có ít nhất một phương thức liên hệ (số điện thoại hoặc email)");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Create customer using service
                var result = await _customerService.CreateAsync(
                    Input.FullName,
                    Input.Email ?? "",
                    Input.PhoneNumber ?? "",
                    Input.Address ?? ""
                );

                if (result.Success)
                {
                    TempData["Success"] = "Tạo khách hàng thành công!";
                    return RedirectToPage("/DealerStaff/Customers/Index");
                }
                else
                {
                    ModelState.AddModelError("", result.Error ?? "Không thể tạo khách hàng");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                return Page();
            }
        }
    }
}
