using System.ComponentModel.DataAnnotations;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager.Orders
{
	public class CreateModel : BaseDealerManagerPageModel
	{
		private readonly IProductService productService;

		public CreateModel(
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
			this.productService = productService;
		}

		[BindProperty]
		public InputModel Input { get; set; } = new();

		public List<DataAccessLayer.Entities.Product> Products { get; private set; } = new();
		public List<DataAccessLayer.Entities.Customer> Customers { get; private set; } = new();
		public List<DataAccessLayer.Entities.Users> Staff { get; private set; } = new();

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			var (okP, _, products) = await productService.SearchAsync(null, null, null, null, null, true);
			Products = products;

			var (okC, _, customers) = await CustomerService.GetAllByDealerAsync(dealerId.Value);
			Customers = customers;

			Staff = await ReportService.GetUsersByDealerAsync(dealerId.Value);

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			if (!ModelState.IsValid)
			{
				await OnGetAsync();
				return Page();
			}

			var (ok, err, order) = await OrderService.CreateQuotationAsync(
				Input.ProductId,
				Input.CustomerId,
				dealerId.Value,
				Input.SalesPersonId,
				Input.Price,
				Input.Discount,
				Input.Description ?? string.Empty,
				Input.Notes ?? string.Empty
			);

			if (!ok)
			{
				ModelState.AddModelError(string.Empty, err ?? "Không thể tạo báo giá");
				await OnGetAsync();
				return Page();
			}

			TempData["Success"] = "Tạo báo giá thành công";
			return RedirectToPage("/DealerManager/Orders/Index");
		}

		public class InputModel
		{
			[Required] public Guid ProductId { get; set; }
			[Required] public Guid CustomerId { get; set; }
			[Required] public decimal Price { get; set; }
			public decimal Discount { get; set; }
			public Guid? SalesPersonId { get; set; }
			public string? Description { get; set; }
			public string? Notes { get; set; }
		}
	}
}

