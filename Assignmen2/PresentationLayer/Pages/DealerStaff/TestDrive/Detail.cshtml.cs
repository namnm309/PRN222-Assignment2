using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using BusinessLayer.Enums;

namespace Assignmen2.PresentationLayer.Pages.DealerStaff.TestDrive
{
    public class DetailModel : PageModel
    {
        private readonly ITestDriveService _testDriveService;

        public DetailModel(ITestDriveService testDriveService)
        {
            _testDriveService = testDriveService;
        }

        public TestDriveResponse? TestDrive { get; set; }

        private Guid? GetCurrentDealerId()
        {
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            return Guid.TryParse(dealerIdString, out var dealerId) ? dealerId : null;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var dealerId = GetCurrentDealerId();
                if (!dealerId.HasValue)
                {
                    TempData["Error"] = "Không xác định được đại lý";
                    return RedirectToPage("/DealerStaff/TestDrive/Index");
                }

                var result = await _testDriveService.GetAsync(id);
                if (!result.Success || result.Data == null)
                {
                    TempData["Error"] = result.Error ?? "Không tìm thấy lịch hẹn lái thử";
                    return RedirectToPage("/DealerStaff/TestDrive/Index");
                }

                // Kiểm tra quyền truy cập
                if (result.Data.DealerId != dealerId.Value)
                {
                    TempData["Error"] = "Bạn không có quyền truy cập lịch hẹn này";
                    return RedirectToPage("/DealerStaff/TestDrive/Index");
                }

                TestDrive = result.Data;
                return Page();
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
                var dealerId = GetCurrentDealerId();
                if (!dealerId.HasValue)
                {
                    TempData["Error"] = "Không xác định được đại lý";
                    return RedirectToPage("/DealerStaff/TestDrive/Index");
                }

                // Kiểm tra quyền trước khi xác nhận
                var testDriveResult = await _testDriveService.GetAsync(id);
                if (!testDriveResult.Success || testDriveResult.Data == null)
                {
                    TempData["Error"] = "Không tìm thấy lịch hẹn";
                    return RedirectToPage("/DealerStaff/TestDrive/Index");
                }

                if (testDriveResult.Data.DealerId != dealerId.Value)
                {
                    TempData["Error"] = "Bạn không có quyền xác nhận lịch hẹn này";
                    return RedirectToPage("/DealerStaff/TestDrive/Index");
                }

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
                var dealerId = GetCurrentDealerId();
                if (!dealerId.HasValue)
                {
                    TempData["Error"] = "Không xác định được đại lý";
                    return RedirectToPage("/DealerStaff/TestDrive/Index");
                }

                // Kiểm tra quyền trước khi hủy
                var testDriveResult = await _testDriveService.GetAsync(id);
                if (!testDriveResult.Success || testDriveResult.Data == null)
                {
                    TempData["Error"] = "Không tìm thấy lịch hẹn";
                    return RedirectToPage("/DealerStaff/TestDrive/Index");
                }

                if (testDriveResult.Data.DealerId != dealerId.Value)
                {
                    TempData["Error"] = "Bạn không có quyền hủy lịch hẹn này";
                    return RedirectToPage("/DealerStaff/TestDrive/Index");
                }

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
