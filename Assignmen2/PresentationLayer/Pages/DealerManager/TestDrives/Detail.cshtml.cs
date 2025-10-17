using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager.TestDrives
{
	public class DetailModel : BaseDealerManagerPageModel
	{
		public record ItemVm(Guid Id, string CustomerName, string ProductName, string StatusName);
		public ItemVm? Item { get; private set; }
		public bool CanCreateCustomer { get; private set; }

		[BindProperty]
		public Guid Id { get; set; }

		public DetailModel(
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

		public async Task<IActionResult> OnGetAsync(Guid id)
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			var (ok, _, td) = await TestDriveService.GetAsync(id);
			if (!ok || td == null || td.DealerId != dealerId) return RedirectToPage("/DealerManager/TestDrives/Index");

			Item = new ItemVm(
				td.Id,
				td.Customer?.FullName ?? td.CustomerName,
				td.Product?.Name ?? string.Empty,
				td.Status.ToString());
			CanCreateCustomer = td.Status == DataAccessLayer.Enum.TestDriveStatus.Successfully || td.Status == DataAccessLayer.Enum.TestDriveStatus.Failed;
			Id = id;
			return Page();
		}

		public async Task<IActionResult> OnPostConfirmAsync()
		{
			await TestDriveService.ConfirmAsync(Id);
			return RedirectToPage("/DealerManager/TestDrives/Detail", new { id = Id });
		}

		public async Task<IActionResult> OnPostCompleteSuccessAsync()
		{
			await TestDriveService.CompleteAsync(Id, true);
			return RedirectToPage("/DealerManager/TestDrives/Detail", new { id = Id });
		}

		public async Task<IActionResult> OnPostCompleteFailAsync()
		{
			await TestDriveService.CompleteAsync(Id, false);
			return RedirectToPage("/DealerManager/TestDrives/Detail", new { id = Id });
		}

		public async Task<IActionResult> OnPostCancelAsync()
		{
			await TestDriveService.CancelAsync(Id);
			return RedirectToPage("/DealerManager/TestDrives/Detail", new { id = Id });
		}
	}
}


