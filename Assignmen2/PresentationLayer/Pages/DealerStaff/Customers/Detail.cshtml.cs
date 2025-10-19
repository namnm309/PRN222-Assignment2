using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;

namespace Assignmen2.PresentationLayer.Pages.DealerStaff.Customers
{
    public class DetailModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public DetailModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public CustomerResponse? Customer { get; set; }

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
    }
}
