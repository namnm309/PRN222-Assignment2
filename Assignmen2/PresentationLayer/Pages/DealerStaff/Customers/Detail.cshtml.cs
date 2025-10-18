using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Services;

namespace Assignmen2.PresentationLayer.Pages.DealerStaff.Customers
{
    public class DetailModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public DetailModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public DataAccessLayer.Entities.Customer? Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var result = await _customerService.GetAsync(id);
                if (result.Success && result.Data != null)
                {
                    Customer = result.Data;
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
