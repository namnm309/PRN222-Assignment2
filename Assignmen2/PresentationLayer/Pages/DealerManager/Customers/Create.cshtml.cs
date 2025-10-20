using BusinessLayer.Services;
using BusinessLayer.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager.Customers
{
    public class CreateModel : BaseDealerManagerPageModel
    {
        public CreateModel(
            IDealerService dealerService,
            IOrderService orderService,
            ITestDriveService testDriveService,
            ICustomerService customerService,
            IEVMReportService reportService,
            IDealerDebtService dealerDebtService,
            IAuthenService authenService,
            IPurchaseOrderService purchaseOrderService,
            IProductService productService,
            IBrandService brandService,
            IMappingService mappingService)
            : base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService, mappingService) {}

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

            [Display(Name = "Ngày sinh")]
            public DateTime? BirthDate { get; set; }

            [Display(Name = "Giới tính")]
            public string? Gender { get; set; }

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
                var result = await CustomerService.CreateAsync(
                    Input.FullName,
                    Input.Email ?? "",
                    Input.PhoneNumber ?? "",
                    Input.Address ?? ""
                );

                if (result.Success)
                {
                    TempData["Success"] = "Tạo khách hàng thành công!";
                    return RedirectToPage("/DealerManager/Customers/Index");
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
                    var result = await CustomerService.GetByPhoneAsync(phone);
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
                var birthDate = form["birthDate"].ToString();
                var gender = form["gender"].ToString();
                var testDriveId = form["testDriveId"].ToString();

                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phone))
                {
                    return new JsonResult(new { success = false, message = "Tên và số điện thoại là bắt buộc" });
                }

                var result = await CustomerService.CreateAsync(fullName, email, phone, address);
                
                if (result.Success)
                {
                    // Link customer to test drive if testDriveId is provided
                    if (!string.IsNullOrWhiteSpace(testDriveId) && Guid.TryParse(testDriveId, out var tdId))
                    {
                        var (linkOk, linkErr) = await TestDriveService.UpdateCustomerAsync(tdId, result.Data.Id);
                        if (!linkOk)
                        {
                            Console.WriteLine($"Warning: Could not link customer to test drive: {linkErr}");
                        }
                    }

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