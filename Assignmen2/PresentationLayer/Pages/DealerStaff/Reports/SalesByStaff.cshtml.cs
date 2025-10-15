using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.Reports
{
    public class SalesByStaffModel : PageModel
    {
        private readonly IOrderService _orderService;

        public SalesByStaffModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? StaffId { get; set; }

        public List<SalesDataItem> SalesData { get; set; } = new();
        public List<Users> StaffList { get; set; } = new();
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public Users? TopPerformer { get; set; }

        public class SalesDataItem
        {
            public Guid StaffId { get; set; }
            public string StaffName { get; set; } = string.Empty;
            public string StaffEmail { get; set; } = string.Empty;
            public int OrderCount { get; set; }
            public decimal TotalSales { get; set; }
            public decimal AverageOrderValue { get; set; }
            public decimal CompletionRate { get; set; }
            public int Rank { get; set; }
        }

        public async Task OnGetAsync()
        {
            // Set default date range if not provided
            if (!FromDate.HasValue)
                FromDate = DateTime.Today.AddDays(-30);
            if (!ToDate.HasValue)
                ToDate = DateTime.Today;

            // Load staff list
            StaffList = GetSampleStaffList();

            // Load sales data
            SalesData = await GetSalesDataAsync();

            // Calculate summary
            TotalSales = SalesData.Sum(s => s.TotalSales);
            TotalOrders = SalesData.Sum(s => s.OrderCount);
            TopPerformer = SalesData.Any() ? StaffList.FirstOrDefault(s => s.Id == SalesData.OrderByDescending(x => x.TotalSales).First().StaffId) : null;
        }

        private async Task<List<SalesDataItem>> GetSalesDataAsync()
        {
            // TODO: Implement real sales data retrieval from database
            // For now, return sample data
            return new List<SalesDataItem>
            {
                new SalesDataItem
                {
                    StaffId = Guid.NewGuid(),
                    StaffName = "Nguyễn Văn A",
                    StaffEmail = "nguyenvana@dealer.com",
                    OrderCount = 15,
                    TotalSales = 2500000000,
                    AverageOrderValue = 166666667,
                    CompletionRate = 95.5m,
                    Rank = 1
                },
                new SalesDataItem
                {
                    StaffId = Guid.NewGuid(),
                    StaffName = "Trần Thị B",
                    StaffEmail = "tranthib@dealer.com",
                    OrderCount = 12,
                    TotalSales = 1800000000,
                    AverageOrderValue = 150000000,
                    CompletionRate = 88.2m,
                    Rank = 2
                },
                new SalesDataItem
                {
                    StaffId = Guid.NewGuid(),
                    StaffName = "Lê Văn C",
                    StaffEmail = "levanc@dealer.com",
                    OrderCount = 10,
                    TotalSales = 1200000000,
                    AverageOrderValue = 120000000,
                    CompletionRate = 75.0m,
                    Rank = 3
                },
                new SalesDataItem
                {
                    StaffId = Guid.NewGuid(),
                    StaffName = "Phạm Thị D",
                    StaffEmail = "phamthid@dealer.com",
                    OrderCount = 8,
                    TotalSales = 800000000,
                    AverageOrderValue = 100000000,
                    CompletionRate = 65.5m,
                    Rank = 4
                }
            };
        }

        private List<Users> GetSampleStaffList()
        {
            return new List<Users>
            {
                new Users
                {
                    Id = Guid.NewGuid(),
                    FullName = "Nguyễn Văn A",
                    Email = "nguyenvana@dealer.com"
                },
                new Users
                {
                    Id = Guid.NewGuid(),
                    FullName = "Trần Thị B",
                    Email = "tranthib@dealer.com"
                },
                new Users
                {
                    Id = Guid.NewGuid(),
                    FullName = "Lê Văn C",
                    Email = "levanc@dealer.com"
                },
                new Users
                {
                    Id = Guid.NewGuid(),
                    FullName = "Phạm Thị D",
                    Email = "phamthid@dealer.com"
                }
            };
        }
    }
}
