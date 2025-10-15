using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.Customers
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public IndexModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "CreatedDate";

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public List<Customer> Customers { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Get all customers
            var result = await _customerService.GetAllAsync();
            if (result.Success)
            {
                Customers = result.Data;

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(Search))
                {
                    var searchLower = Search.ToLower();
                    Customers = Customers.Where(c => 
                        c.FullName.ToLower().Contains(searchLower) ||
                        (c.Name != null && c.Name.ToLower().Contains(searchLower)) ||
                        (c.Email != null && c.Email.ToLower().Contains(searchLower)) ||
                        (c.PhoneNumber != null && c.PhoneNumber.Contains(Search))
                    ).ToList();
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(Status))
                {
                    // For now, all customers are considered active
                    // You can add an IsActive property to Customer entity if needed
                }

                // Apply sorting
                Customers = SortBy switch
                {
                    "Name" => Customers.OrderBy(c => c.FullName).ToList(),
                    "NameDesc" => Customers.OrderByDescending(c => c.FullName).ToList(),
                    "CreatedDate" => Customers.OrderBy(c => c.CreatedAt).ToList(),
                    "CreatedDateDesc" => Customers.OrderByDescending(c => c.CreatedAt).ToList(),
                    _ => Customers.OrderByDescending(c => c.CreatedAt).ToList()
                };

                TotalItems = Customers.Count;
                TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);

                // Simple pagination (in real app, you'd implement this in the service layer)
                Customers = Customers
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
        }
    }
}
