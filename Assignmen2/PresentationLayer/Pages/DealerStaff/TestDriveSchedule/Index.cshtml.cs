using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.TestDriveSchedule
{
    public class IndexModel : PageModel
    {
        private readonly ITestDriveService _testDriveService;
        private readonly IProductService _productService;

        public IndexModel(ITestDriveService testDriveService, IProductService productService)
        {
            _testDriveService = testDriveService;
            _productService = productService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? ProductId { get; set; }

        public List<DataAccessLayer.Entities.TestDrive> TestDrives { get; set; } = new();
        public List<Product> Products { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Load products for filter
            var productsResult = await _productService.SearchAsync(null, null, null, null, true, true);
            if (productsResult.Success)
            {
                Products = productsResult.Data;
            }

            // Get current dealer ID (you'll need to implement this)
            var dealerId = GetCurrentDealerId();
            
            if (dealerId.HasValue)
            {
                var result = await _testDriveService.GetByDealerAsync(dealerId.Value);
                if (result.Success)
                {
                    TestDrives = result.Data;

                    // Apply search filter
                    if (!string.IsNullOrWhiteSpace(Search))
                    {
                        var searchLower = Search.ToLower();
                        TestDrives = TestDrives.Where(t => 
                            (t.Customer?.FullName ?? t.CustomerName).ToLower().Contains(searchLower) ||
                            (t.Customer?.PhoneNumber ?? t.CustomerPhone).Contains(Search) ||
                            (t.Customer?.Email ?? t.CustomerEmail ?? "").ToLower().Contains(searchLower)
                        ).ToList();
                    }

                    // Apply status filter
                    if (!string.IsNullOrWhiteSpace(Status))
                    {
                        if (Enum.TryParse<DataAccessLayer.Enum.TestDriveStatus>(Status, out var statusEnum))
                        {
                            TestDrives = TestDrives.Where(t => t.Status == statusEnum).ToList();
                        }
                    }

                    // Apply date filters
                    if (FromDate.HasValue)
                    {
                        TestDrives = TestDrives.Where(t => t.ScheduledDate.Date >= FromDate.Value.Date).ToList();
                    }

                    if (ToDate.HasValue)
                    {
                        TestDrives = TestDrives.Where(t => t.ScheduledDate.Date <= ToDate.Value.Date).ToList();
                    }

                    // Apply product filter
                    if (ProductId.HasValue)
                    {
                        TestDrives = TestDrives.Where(t => t.ProductId == ProductId.Value).ToList();
                    }

                    // Sort by scheduled date descending
                    TestDrives = TestDrives.OrderByDescending(t => t.ScheduledDate).ToList();
                }
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
