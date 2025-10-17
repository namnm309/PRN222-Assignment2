using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.Reports
{
    public class CustomerDebtModel : PageModel
    {
        private readonly IOrderService _orderService;

        public CustomerDebtModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? DebtStatus { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? DebtRange { get; set; }

        public List<DebtDataItem> DebtData { get; set; } = new();
        public decimal TotalDebt { get; set; }
        public int CustomersWithDebt { get; set; }
        public decimal OverdueDebt { get; set; }
        public decimal CollectionRate { get; set; }
        public int NoDebtCount { get; set; }
        public int LowDebtCount { get; set; }
        public int MediumDebtCount { get; set; }
        public int HighDebtCount { get; set; }

        public class DebtDataItem
        {
            public Guid CustomerId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string CustomerEmail { get; set; } = string.Empty;
            public string CustomerPhone { get; set; } = string.Empty;
            public int OrderCount { get; set; }
            public decimal TotalPurchased { get; set; }
            public decimal TotalPaid { get; set; }
            public decimal DebtAmount { get; set; }
            public DateTime? OldestDebtDate { get; set; }
        }

        public async Task OnGetAsync()
        {
            // Load debt data
            DebtData = await GetDebtDataAsync();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(Search))
            {
                var searchLower = Search.ToLower();
                DebtData = DebtData.Where(d => 
                    d.CustomerName.ToLower().Contains(searchLower) ||
                    d.CustomerEmail.ToLower().Contains(searchLower) ||
                    d.CustomerPhone.Contains(Search)
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(DebtStatus))
            {
                DebtData = DebtStatus switch
                {
                    "NoDebt" => DebtData.Where(d => d.DebtAmount == 0).ToList(),
                    "HasDebt" => DebtData.Where(d => d.DebtAmount > 0).ToList(),
                    "Overdue" => DebtData.Where(d => d.OldestDebtDate.HasValue && d.OldestDebtDate.Value < DateTime.Now.AddDays(-30)).ToList(),
                    "HighDebt" => DebtData.Where(d => d.DebtAmount > 100000000).ToList(),
                    _ => DebtData
                };
            }

            if (!string.IsNullOrWhiteSpace(DebtRange))
            {
                DebtData = DebtRange switch
                {
                    "0-10M" => DebtData.Where(d => d.DebtAmount >= 0 && d.DebtAmount <= 10000000).ToList(),
                    "10M-50M" => DebtData.Where(d => d.DebtAmount > 10000000 && d.DebtAmount <= 50000000).ToList(),
                    "50M-100M" => DebtData.Where(d => d.DebtAmount > 50000000 && d.DebtAmount <= 100000000).ToList(),
                    "100M+" => DebtData.Where(d => d.DebtAmount > 100000000).ToList(),
                    _ => DebtData
                };
            }

            // Calculate summary statistics
            CalculateSummary();
        }

        private async Task<List<DebtDataItem>> GetDebtDataAsync()
        {
            // TODO: Implement real debt data retrieval using service
            // This should call a service method to get debt data from database
            // For now, return sample data
            return new List<DebtDataItem>
            {
                new DebtDataItem
                {
                    CustomerId = Guid.NewGuid(),
                    CustomerName = "Nguyễn Văn A",
                    CustomerEmail = "nguyenvana@email.com",
                    CustomerPhone = "0901234567",
                    OrderCount = 3,
                    TotalPurchased = 500000000,
                    TotalPaid = 500000000,
                    DebtAmount = 0,
                    OldestDebtDate = null
                },
                new DebtDataItem
                {
                    CustomerId = Guid.NewGuid(),
                    CustomerName = "Trần Thị B",
                    CustomerEmail = "tranthib@email.com",
                    CustomerPhone = "0901234568",
                    OrderCount = 2,
                    TotalPurchased = 300000000,
                    TotalPaid = 200000000,
                    DebtAmount = 100000000,
                    OldestDebtDate = DateTime.Now.AddDays(-15)
                },
                new DebtDataItem
                {
                    CustomerId = Guid.NewGuid(),
                    CustomerName = "Lê Văn C",
                    CustomerEmail = "levanc@email.com",
                    CustomerPhone = "0901234569",
                    OrderCount = 1,
                    TotalPurchased = 150000000,
                    TotalPaid = 50000000,
                    DebtAmount = 100000000,
                    OldestDebtDate = DateTime.Now.AddDays(-45)
                },
                new DebtDataItem
                {
                    CustomerId = Guid.NewGuid(),
                    CustomerName = "Phạm Thị D",
                    CustomerEmail = "phamthid@email.com",
                    CustomerPhone = "0901234570",
                    OrderCount = 4,
                    TotalPurchased = 800000000,
                    TotalPaid = 600000000,
                    DebtAmount = 200000000,
                    OldestDebtDate = DateTime.Now.AddDays(-60)
                },
                new DebtDataItem
                {
                    CustomerId = Guid.NewGuid(),
                    CustomerName = "Hoàng Văn E",
                    CustomerEmail = "hoangvane@email.com",
                    CustomerPhone = "0901234571",
                    OrderCount = 2,
                    TotalPurchased = 400000000,
                    TotalPaid = 400000000,
                    DebtAmount = 0,
                    OldestDebtDate = null
                }
            };
        }

        private void CalculateSummary()
        {
            TotalDebt = DebtData.Sum(d => d.DebtAmount);
            CustomersWithDebt = DebtData.Count(d => d.DebtAmount > 0);
            OverdueDebt = DebtData.Where(d => d.OldestDebtDate.HasValue && d.OldestDebtDate.Value < DateTime.Now.AddDays(-30))
                .Sum(d => d.DebtAmount);
            
            var totalPurchased = DebtData.Sum(d => d.TotalPurchased);
            var totalPaid = DebtData.Sum(d => d.TotalPaid);
            CollectionRate = totalPurchased > 0 ? (totalPaid / totalPurchased) * 100 : 0;

            NoDebtCount = DebtData.Count(d => d.DebtAmount == 0);
            LowDebtCount = DebtData.Count(d => d.DebtAmount > 0 && d.DebtAmount <= 10000000);
            MediumDebtCount = DebtData.Count(d => d.DebtAmount > 10000000 && d.DebtAmount <= 50000000);
            HighDebtCount = DebtData.Count(d => d.DebtAmount > 50000000);
        }
    }
}
