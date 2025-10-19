using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using BusinessLayer.DTOs.Responses;

namespace PresentationLayer.Pages.DealerManager.Customers
{
	public class IndexModel : BaseDealerManagerPageModel
	{
		public IndexModel(
			IDealerService dealerService,
			IOrderService orderService,
			ITestDriveService testDriveService,
			ICustomerService customerService,
			IEVMReportService reportService,
			IDealerDebtService dealerDebtService,
			IAuthenService authenService,
			IPurchaseOrderService purchaseOrderService,
			IProductService productService,
			IBrandService brandService)
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService) { }

		[BindProperty(SupportsGet = true)]
		public string? Search { get; set; }

        public List<CustomerResponse> Customers { get; private set; } = new();
		public int TotalCustomers { get; private set; }
		public int NewCustomersThisMonth { get; private set; }
		public int ActiveCustomers { get; private set; }

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

            var (ok, _, customers) = await CustomerService.GetAllByDealerAsync(dealerId.Value);
            if (!ok) customers = new List<DataAccessLayer.Entities.Customer>();

			if (!string.IsNullOrWhiteSpace(Search))
			{
				customers = customers.Where(c =>
					c.FullName.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
					(c.PhoneNumber ?? "").Contains(Search) ||
					(c.Email ?? "").Contains(Search, StringComparison.OrdinalIgnoreCase)
				).ToList();
			}

            Customers = customers.Select(c => new CustomerResponse { Id = c.Id, FullName = c.FullName, Email = c.Email, PhoneNumber = c.PhoneNumber, Address = c.Address, IsActive = c.IsActive, CreatedAt = c.CreatedAt, UpdatedAt = c.UpdatedAt }).ToList();
            TotalCustomers = Customers.Count;
            NewCustomersThisMonth = Customers.Count(c => c.CreatedAt.Month == DateTime.Today.Month && c.CreatedAt.Year == DateTime.Today.Year);
            ActiveCustomers = Customers.Count; // giả định tất cả đang hoạt động

			return Page();
		}
	}
}

