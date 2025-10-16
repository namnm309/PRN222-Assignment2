using System.ComponentModel.DataAnnotations;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager.Customers
{
	public class CreateModel : BaseDealerManagerPageModel
	{
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
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService) { }

		[BindProperty]
		public InputModel Input { get; set; } = new();

		public IActionResult OnGet()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			if (!ModelState.IsValid) return Page();

			var (ok, err, _) = await CustomerService.CreateAsync(
				Input.FullName,
				Input.Email ?? string.Empty,
				Input.PhoneNumber,
				Input.Address ?? string.Empty
			);

			if (!ok)
			{
				ModelState.AddModelError(string.Empty, err ?? "Không thể tạo khách hàng");
				return Page();
			}

			TempData["Success"] = "Tạo khách hàng thành công";
			return RedirectToPage("/DealerManager/Customers/Index");
		}

		public class InputModel
		{
			[Required] public string FullName { get; set; } = string.Empty;
			[Required] public string PhoneNumber { get; set; } = string.Empty;
			public string? Email { get; set; }
			public string? Address { get; set; }
		}
	}
}

