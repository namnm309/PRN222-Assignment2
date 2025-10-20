using System.ComponentModel.DataAnnotations;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using BusinessLayer.DTOs.Responses;

namespace PresentationLayer.Pages.DealerManager.Customers
{
    public class EditModel : BaseDealerManagerPageModel
    {
        public EditModel(
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
            : base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService, mappingService) { }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public CustomerResponse Customer { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var dealerId = GetCurrentDealerId();
            if (dealerId == null) return RedirectToPage("/Dashboard/Index");

            var (ok, err, customer) = await CustomerService.GetAsync(id);
            if (!ok || customer == null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng";
                return RedirectToPage("/DealerManager/Customers/Index");
            }

            Customer = new CustomerResponse
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };
            
            // Populate form with existing data
            Input.FullName = customer.FullName;
            Input.PhoneNumber = customer.PhoneNumber ?? "";
            Input.Email = customer.Email;
            Input.Address = customer.Address;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            var dealerId = GetCurrentDealerId();
            if (dealerId == null) return RedirectToPage("/Dashboard/Index");

            if (!ModelState.IsValid)
            {
                // Reload customer data
                var (success, error, customer) = await CustomerService.GetAsync(id);
                if (success && customer != null)
                {
                    Customer = new CustomerResponse
                    {
                        Id = customer.Id,
                        FullName = customer.FullName,
                        Email = customer.Email,
                        PhoneNumber = customer.PhoneNumber,
                        Address = customer.Address,
                        IsActive = customer.IsActive,
                        CreatedAt = customer.CreatedAt,
                        UpdatedAt = customer.UpdatedAt
                    };
                }
                return Page();
            }

            // Check if email is being changed and if it already exists
            var (ok, err, existingCustomer) = await CustomerService.GetAsync(id);
            if (!ok || existingCustomer == null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng";
                return RedirectToPage("/DealerManager/Customers/Index");
            }

            Customer = new CustomerResponse
            {
                Id = existingCustomer.Id,
                FullName = existingCustomer.FullName,
                Email = existingCustomer.Email,
                PhoneNumber = existingCustomer.PhoneNumber,
                Address = existingCustomer.Address,
                IsActive = existingCustomer.IsActive,
                CreatedAt = existingCustomer.CreatedAt,
                UpdatedAt = existingCustomer.UpdatedAt
            };

            // If email is being changed, check if new email already exists
            if (!string.IsNullOrEmpty(Input.Email) && Input.Email != existingCustomer.Email)
            {
                var (emailExists, _, _) = await CustomerService.GetByEmailAsync(Input.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Input.Email", "Email này đã được sử dụng bởi khách hàng khác");
                    return Page();
                }
            }

            // If phone is being changed, check if new phone already exists
            if (Input.PhoneNumber != existingCustomer.PhoneNumber)
            {
                var (phoneExists, _, _) = await CustomerService.GetByPhoneAsync(Input.PhoneNumber);
                if (phoneExists)
                {
                    ModelState.AddModelError("Input.PhoneNumber", "Số điện thoại này đã được sử dụng bởi khách hàng khác");
                    return Page();
                }
            }

            try
            {
                var (success, error) = await CustomerService.UpdateAsync(
                    id,
                    Input.FullName,
                    Input.Email ?? "",
                    Input.PhoneNumber,
                    Input.Address ?? ""
                );

                if (!success)
                {
                    ModelState.AddModelError("", error ?? "Không thể cập nhật khách hàng");
                    return Page();
                }

                TempData["Success"] = "Cập nhật khách hàng thành công!";
                return RedirectToPage("/DealerManager/Customers/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                return Page();
            }
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Họ và tên là bắt buộc")]
            [Display(Name = "Họ và tên")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
            [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; } = string.Empty;

            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Display(Name = "Địa chỉ")]
            public string? Address { get; set; }
        }
    }
}
