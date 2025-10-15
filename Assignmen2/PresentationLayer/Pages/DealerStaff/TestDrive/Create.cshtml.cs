using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Pages.DealerStaff.TestDrive
{
    public class CreateModel : PageModel
    {
        private readonly ITestDriveService _testDriveService;
        private readonly IProductService _productService;

        public CreateModel(ITestDriveService testDriveService, IProductService productService)
        {
            _testDriveService = testDriveService;
            _productService = productService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<Product> Products { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
            [Display(Name = "Tên khách hàng")]
            public string CustomerName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
            [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
            [Display(Name = "Số điện thoại")]
            public string CustomerPhone { get; set; } = string.Empty;

            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            [Display(Name = "Email")]
            public string CustomerEmail { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng chọn xe lái thử")]
            [Display(Name = "Xe lái thử")]
            public Guid ProductId { get; set; }

            [Required(ErrorMessage = "Ngày hẹn là bắt buộc")]
            [Display(Name = "Ngày hẹn")]
            public DateTime ScheduledDate { get; set; }

            [Required(ErrorMessage = "Giờ hẹn là bắt buộc")]
            [Display(Name = "Giờ hẹn")]
            public TimeSpan ScheduledTime { get; set; }

            [Display(Name = "Ghi chú")]
            public string? Notes { get; set; }
        }

        public async Task OnGetAsync()
        {
            // Load available products for test drive
            var productsResult = await _productService.SearchAsync(null, null, null, null, true, true);
            if (productsResult.Success)
            {
                Products = productsResult.Data;
            }

            // Set default date to tomorrow
            Input.ScheduledDate = DateTime.Today.AddDays(1);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload products if validation fails
                var productsResult = await _productService.SearchAsync(null, null, null, null, true, true);
                if (productsResult.Success)
                {
                    Products = productsResult.Data;
                }
                return Page();
            }

            // Get current dealer ID (you'll need to implement this)
            var dealerId = GetCurrentDealerId();
            if (!dealerId.HasValue)
            {
                ModelState.AddModelError("", "Không xác định được đại lý hiện tại");
                return Page();
            }

            // Combine date and time
            var scheduledDateTime = Input.ScheduledDate.Date.Add(Input.ScheduledTime);

            // Create test drive
            var result = await _testDriveService.CreatePublicAsync(
                Input.CustomerName,
                Input.CustomerPhone,
                Input.CustomerEmail,
                Input.Notes,
                Input.ProductId,
                dealerId.Value,
                scheduledDateTime
            );

            if (result.Success)
            {
                TempData["Success"] = "Tạo lịch hẹn lái thử thành công!";
                return RedirectToPage("/DealerStaff/TestDrive/Index");
            }
            else
            {
                ModelState.AddModelError("", result.Error ?? "Không thể tạo lịch hẹn");
                return Page();
            }
        }

        private Guid? GetCurrentDealerId()
        {
            // TODO: Implement getting current dealer ID from session/authentication
            // For now, return a dummy ID - you'll need to implement this based on your authentication system
            return Guid.NewGuid();
        }
    }
}
