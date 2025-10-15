using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.TestDrive
{
    public class IndexModel : PageModel
    {
        private readonly ITestDriveService _testDriveService;

        public IndexModel(ITestDriveService testDriveService)
        {
            _testDriveService = testDriveService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        public List<DataAccessLayer.Entities.TestDrive> TestDrives { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Get current dealer ID from session (you'll need to implement this)
            var dealerId = GetCurrentDealerId();
            
            if (dealerId.HasValue)
            {
                var result = await _testDriveService.GetByDealerAsync(dealerId.Value);
                if (result.Success)
                {
                    TestDrives = result.Data;

                    // Apply filters
                    if (!string.IsNullOrWhiteSpace(Status))
                    {
                        TestDrives = TestDrives.Where(t => t.Status.ToString() == Status).ToList();
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
            }
        }

        private Guid? GetCurrentDealerId()
        {
            // TODO: Implement getting current dealer ID from session/authentication
            // For now, return null - you'll need to implement this based on your authentication system
            return null;
        }
    }
}
