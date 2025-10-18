using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.TestDrive
{
    public class IndexModel : PageModel
    {
        private readonly ITestDriveService _testDriveService;
        private readonly IMappingService _mappingService;

        public IndexModel(ITestDriveService testDriveService, IMappingService mappingService)
        {
            _testDriveService = testDriveService;
            _mappingService = mappingService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        public List<TestDriveResponse> TestDrives { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Get current dealer ID from session
            var dealerId = GetCurrentDealerId();
            
            if (dealerId.HasValue)
            {
                var result = await _testDriveService.GetByDealerAsync(dealerId.Value);
                if (result.Success && result.Data != null)
                {
                    // Map entities to DTOs using mapping service
                    TestDrives = _mappingService.MapToTestDriveViewModels(result.Data);

                    // Apply filters
                    if (!string.IsNullOrWhiteSpace(Status))
                    {
                        if (Enum.TryParse<BusinessLayer.Enums.TestDriveStatus>(Status, out var statusEnum))
                        {
                            TestDrives = TestDrives.Where(t => t.Status == statusEnum).ToList();
                        }
                    }

                    if (FromDate.HasValue)
                    {
                        TestDrives = TestDrives.Where(t => t.ScheduledDate.Date >= FromDate.Value.Date).ToList();
                    }

                    if (ToDate.HasValue)
                    {
                        TestDrives = TestDrives.Where(t => t.ScheduledDate.Date <= ToDate.Value.Date).ToList();
                    }

                    // Sort by scheduled date descending
                    TestDrives = TestDrives.OrderByDescending(t => t.ScheduledDate).ToList();
                }
                else
                {
                    TestDrives = new List<TestDriveResponse>();
                    TempData["Error"] = result.Error ?? "Không thể tải danh sách lái thử";
                }
            }
            else
            {
                TestDrives = new List<TestDriveResponse>();
                TempData["Error"] = "Không xác định được đại lý hiện tại";
            }
        }

        // Handler for confirming test drive
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
            return RedirectToPage();
        }

        // Handler for canceling test drive
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
            return RedirectToPage();
        }

        private Guid? GetCurrentDealerId()
        {
            // Get dealer ID from session
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            if (Guid.TryParse(dealerIdString, out var dealerId))
            {
                return dealerId;
            }
            return null;
        }
    }
}
