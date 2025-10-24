using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerStaff.Customers
{
    public class IndexModel : BaseDealerStaffPageModel
    {
        public IndexModel(ICustomerService customerService, IMappingService mappingService, IDealerService dealerService)
            : base(dealerService, null, null, customerService, null, null, null, null, null, null, mappingService)
        {
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "CreatedDateDesc";

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public List<CustomerResponse> Customers { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Clear any potential caching issues
            Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Append("Pragma", "no-cache");
            Response.Headers.Append("Expires", "0");
            
            // Get page parameter directly from query string
            var pageFromQuery = Request.Query["page"].FirstOrDefault();
            int.TryParse(pageFromQuery, out int pageFromUrl);
            
            // Set ViewData for DealerName
            await SetDealerNameViewDataAsync();
            
            // Get all customers using service
            var result = await CustomerService.GetAllAsync();
            if (result.Success && result.Data != null)
            {
                // Map entities to DTOs using mapping service
                Customers = MappingService.MapToCustomerViewModels(result.Data);

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(Search))
                {
                    var searchLower = Search.ToLower();
                    Customers = Customers.Where(c => 
                        c.FullName.ToLower().Contains(searchLower) ||
                        (c.Name != null && c.Name.ToLower().Contains(searchLower)) ||
                        c.Email.ToLower().Contains(searchLower) ||
                        c.PhoneNumber.Contains(Search)
                    ).ToList();
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(Status))
                {
                    bool isActive = Status == "Active";
                    Customers = Customers.Where(c => c.IsActive == isActive).ToList();
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

                // Use page parameter from URL, fallback to PageNumber, then CurrentPage
                var pageToUse = pageFromUrl > 0 ? pageFromUrl : (PageNumber > 0 ? PageNumber : CurrentPage);
                if (pageToUse < 1) pageToUse = 1;
                if (pageToUse > TotalPages && TotalPages > 0) pageToUse = TotalPages;


                // Simple pagination (in real app, you'd implement this in the service layer)
                Customers = Customers
                    .Skip((pageToUse - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                // Update CurrentPage to reflect actual page used
                CurrentPage = pageToUse;
            }
            else
            {
                Customers = new List<CustomerResponse>();
                TempData["Error"] = result.Error ?? "Không thể tải danh sách khách hàng";
            }
        }
    }
}
