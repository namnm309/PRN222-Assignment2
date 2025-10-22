using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager.Orders
{
	public class IndexModel : BaseDealerManagerPageModel
	{
		public record Item(
			Guid Id, 
			string Code, 
			DateTime CreatedAt, 
			string Status, 
			decimal Total,
			string? ProductName,
			string? ProductSku,
			string? CustomerName,
			string? CustomerPhone,
			decimal Price,
			decimal Discount
		);
		
		public List<Item> Items { get; private set; } = new();
		[BindProperty(SupportsGet = true)] public string? Search { get; set; }
		[BindProperty(SupportsGet = true)] public string? StatusFilter { get; set; }
		[BindProperty(SupportsGet = true)] public int Page { get; set; } = 1;
		public int TotalPages { get; private set; }

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
			IBrandService brandService,
			IMappingService mappingService)
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService, mappingService) {}

		[BindProperty] public Guid OrderId { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			var (ok, err, result) = await OrderService.SearchAsync(dealerId.Value, Search, StatusFilter, null, Page, 10);
			Items = result.Data.Select(o => new Item(
				o.Id,
				o.OrderNumber ?? o.Id.ToString("N").Substring(0, 8),
				o.CreatedAt,
				o.Status,
				o.FinalAmount,
				o.Product?.Name,
				o.Product?.Sku,
				o.Customer?.FullName,
				o.Customer?.PhoneNumber,
				o.Price,
				o.Discount
			)).ToList();
			TotalPages = result.TotalPages;
			return Page();
		}

		public async Task<IActionResult> OnPostProcessPaymentAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Account/Login");

			var (success, error, data) = await OrderService.UpdatePaymentAsync(OrderId, "Paid", "Cash", null);
			if (success)
			{
				TempData["SuccessMessage"] = "Thanh toán đã được xác nhận thành công!";
			}
			else
			{
				TempData["ErrorMessage"] = $"Lỗi khi xác nhận thanh toán: {error}";
			}

			return RedirectToPage();
		}
	}
}


