using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager.TestDrives
{
	public class IndexModel : BaseDealerManagerPageModel
	{
		public record Item(Guid Id, string CustomerName, string ProductName, string StatusName, DateTime ScheduledDate, string? CustomerPhone);
		public List<Item> Items { get; private set; } = new();
		[BindProperty(SupportsGet = true)] public string? StatusFilter { get; set; }
		[BindProperty(SupportsGet = true)] public int Page { get; set; } = 1;
		public int TotalPages { get; private set; }
		public int TotalTestDrives { get; private set; }
		public int PendingCount { get; private set; }
		public int ConfirmedCount { get; private set; }
		public int CompletedCount { get; private set; }

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
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService) {}

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			var (ok, _, list) = await TestDriveService.GetAllAsync(dealerId, StatusFilter);
			
			// Calculate statistics
			TotalTestDrives = list.Count;
			PendingCount = list.Count(td => td.Status.ToString() == "Pending");
			ConfirmedCount = list.Count(td => td.Status.ToString() == "Confirmed");
			CompletedCount = list.Count(td => td.Status.ToString() == "Successfully");
			
			var paged = list
				.OrderByDescending(td => td.ScheduledDate)
				.Skip((Page - 1) * 10)
				.Take(10)
				.ToList();
			Items = paged.Select(td => new Item(
				td.Id,
				td.Customer?.FullName ?? td.CustomerName,
				td.Product?.Name ?? string.Empty,
				td.Status.ToString(),
				td.ScheduledDate,
				td.Customer?.PhoneNumber)).ToList();
			TotalPages = (int)Math.Ceiling((double)list.Count / 10.0);
			return Page();
		}
	}
}


