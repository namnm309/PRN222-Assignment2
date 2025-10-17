using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Services;
using DataAccessLayer.Entities;

namespace Assignmen2.PresentationLayer.Pages.DealerStaff.TestDrive
{
    public class DetailModel : PageModel
    {
        private readonly ITestDriveService _testDriveService;

        public DetailModel(ITestDriveService testDriveService)
        {
            _testDriveService = testDriveService;
        }

        public DataAccessLayer.Entities.TestDrive? TestDrive { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var result = await _testDriveService.GetAsync(id);
                if (result.Success && result.Data != null)
                {
                    TestDrive = result.Data;
                    return Page();
                }
                else
                {
                    TempData["Error"] = result.Error ?? "Không tìm thấy lịch hẹn lái thử";
                    return RedirectToPage("/DealerStaff/TestDrive/Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToPage("/DealerStaff/TestDrive/Index");
            }
        }

        public async Task<IActionResult> OnPostConfirmAsync(Guid id)
        {
            try
            {
                var result = await _testDriveService.ConfirmAsync(id);
                if (result.Success)
                {
                    TempData["Success"] = "Xác nhận lái thử thành công!";
                }
                else
                {
                    TempData["Error"] = result.Error ?? "Không thể xác nhận lái thử";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
            }
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostCancelAsync(Guid id)
        {
            try
            {
                var result = await _testDriveService.CancelAsync(id);
                if (result.Success)
                {
                    TempData["Success"] = "Hủy lái thử thành công!";
                }
                else
                {
                    TempData["Error"] = result.Error ?? "Không thể hủy lái thử";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
            }
            return RedirectToPage(new { id });
        }
    }
}
