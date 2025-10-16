using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager.PurchaseOrders
{
	public class IndexModel : BaseDealerManagerPageModel
	{
		private readonly IPurchaseOrderService purchaseOrderService;

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
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService)
		{
			this.purchaseOrderService = purchaseOrderService;
		}

		[BindProperty(SupportsGet = true)]
		public string? StatusFilter { get; set; }

		public List<POVm> PurchaseOrders { get; private set; } = new();

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			DataAccessLayer.Enum.PurchaseOrderStatus? status = null;
			if (!string.IsNullOrWhiteSpace(StatusFilter) && int.TryParse(StatusFilter, out var s))
			{
				status = (DataAccessLayer.Enum.PurchaseOrderStatus)s;
			}

			var (ok, _, orders) = await purchaseOrderService.GetAllAsync(dealerId, (BusinessLayer.Enums.PurchaseOrderStatus?)status);
			PurchaseOrders = orders.Select(po => new POVm
			{
				Id = po.Id,
				ProductName = po.Product?.Name ?? "N/A",
				DealerName = po.Dealer?.Name ?? "N/A",
				RequestedByName = po.RequestedBy?.FullName ?? "N/A",
				Quantity = po.RequestedQuantity,
				UnitPrice = po.UnitPrice,
				Status = po.Status,
				CreatedAt = po.CreatedAt
			}).ToList();

			return Page();
		}

		public class POVm
		{
			public Guid Id { get; set; }
			public string ProductName { get; set; } = string.Empty;
			public string DealerName { get; set; } = string.Empty;
			public string RequestedByName { get; set; } = string.Empty;
			public int Quantity { get; set; }
			public decimal UnitPrice { get; set; }
			public DataAccessLayer.Enum.PurchaseOrderStatus Status { get; set; }
			public DateTime CreatedAt { get; set; }
		}
	}
}

