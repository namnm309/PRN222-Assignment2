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
                    return RedirectToPage("/DealerStaff/Customers/Index", new { page = 1, search = "", status = "", sortBy = "CreatedDateDesc" });
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

        // API endpoint to check if customer exists
        public async Task<IActionResult> OnPostCheckCustomerAsync()
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var phone = form["phone"].ToString();
                var name = form["name"].ToString();

                if (string.IsNullOrWhiteSpace(phone) && string.IsNullOrWhiteSpace(name))
                {
                    return new JsonResult(new { exists = false, message = "Vui lòng nhập số điện thoại hoặc tên khách hàng" });
                }

                // Check by phone
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    var result = await _customerService.GetByPhoneAsync(phone);
                    if (result.Success && result.Data != null)
                    {
                        return new JsonResult(new { 
                            exists = true, 
                            customer = new { 
                                name = result.Data.FullName, 
                                phone = result.Data.PhoneNumber, 
                                email = result.Data.Email 
                            } 
                        });
                    }
                }

                return new JsonResult(new { exists = false });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { exists = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // API endpoint to create customer
        public async Task<IActionResult> OnPostCreateCustomerAsync()
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var fullName = form["name"].ToString();
                var phone = form["phone"].ToString();
                var email = form["email"].ToString();
                var address = form["address"].ToString();
                var notes = form["notes"].ToString();

                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phone))
                {
                    return new JsonResult(new { success = false, message = "Tên và số điện thoại là bắt buộc" });
                }

                var result = await _customerService.CreateAsync(fullName, email, phone, address);
                
                if (result.Success)
                {
                    return new JsonResult(new { 
                        success = true, 
                        customer = new { 
                            id = result.Data.Id, 
                            name = result.Data.FullName, 
                            phone = result.Data.PhoneNumber, 
                            email = result.Data.Email 
                        } 
                    });
                }
                else
                {
                    return new JsonResult(new { success = false, message = result.Error ?? "Không thể tạo khách hàng" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

    }
}
