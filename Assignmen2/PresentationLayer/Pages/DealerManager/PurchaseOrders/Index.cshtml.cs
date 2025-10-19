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

            BusinessLayer.Enums.PurchaseOrderStatus? status = null;
			if (!string.IsNullOrWhiteSpace(StatusFilter) && int.TryParse(StatusFilter, out var s))
			{
                status = (BusinessLayer.Enums.PurchaseOrderStatus)s;
			}

            var (ok, _, orders) = await purchaseOrderService.GetAllAsync(dealerId, status);
			PurchaseOrders = orders.Select(po => new POVm
			{
				Id = po.Id,
                ProductName = po.ProductName ?? "N/A",
                DealerName = po.DealerName ?? "N/A",
                RequestedByName = po.RequestedByName ?? "N/A",
                Quantity = po.RequestedQuantity,
				UnitPrice = po.UnitPrice,
                Status = po.Status,
                CreatedAt = po.RequestedDate
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
            public BusinessLayer.Enums.PurchaseOrderStatus Status { get; set; }
			public DateTime CreatedAt { get; set; }
		}
	}
}

