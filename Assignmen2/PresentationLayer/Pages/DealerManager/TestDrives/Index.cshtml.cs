using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using BusinessLayer.DTOs.Responses;

namespace PresentationLayer.Pages.DealerManager.TestDrives
{
	public class IndexModel : BaseDealerManagerPageModel
	{
		[BindProperty(SupportsGet = true)] public string? Status { get; set; }
		[BindProperty(SupportsGet = true)] public DateTime? FromDate { get; set; }
		[BindProperty(SupportsGet = true)] public DateTime? ToDate { get; set; }

		public List<TestDriveResponse> TestDrives { get; private set; } = new();
		// Optional summary counters to match prior view expectations
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

			var result = await TestDriveService.GetByDealerAsync(dealerId.Value);
			if (result.Success && result.Data != null)
			{
				TestDrives = result.Data;
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
				TestDrives = TestDrives.OrderByDescending(t => t.ScheduledDate).ToList();
				// Fill summary counters
				TotalTestDrives = TestDrives.Count;
				PendingCount = TestDrives.Count(t => t.Status == BusinessLayer.Enums.TestDriveStatus.Pending);
				ConfirmedCount = TestDrives.Count(t => t.Status == BusinessLayer.Enums.TestDriveStatus.Confirmed);
				CompletedCount = TestDrives.Count(t => t.Status == BusinessLayer.Enums.TestDriveStatus.Successfully);
			}
			else
			{
				TempData["Error"] = result.Error ?? "Không thể tải danh sách lái thử";
			}
			return Page();
		}

		public async Task<IActionResult> OnPostConfirmAsync(Guid id)
		{
			await TestDriveService.ConfirmAsync(id);
			return RedirectToPage();
		}

		public async Task<IActionResult> OnPostCancelAsync(Guid id)
		{
			await TestDriveService.CancelAsync(id);
			return RedirectToPage();
		}
	}
}


