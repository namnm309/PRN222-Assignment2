using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Pages.DealerManager.Staff
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
			IBrandService brandService) : base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService)
		{
			this.authenService = authenService;
		}

		private readonly IAuthenService authenService;

		public string? DealerName { get; private set; }

		[BindProperty]
		public CreateDealerStaffInput Input { get; set; } = new();

		public class CreateDealerStaffInput
		{
			[Required]
			[StringLength(100)]
			public string FullName { get; set; } = string.Empty;

			[Required]
			[EmailAddress]
			public string Email { get; set; } = string.Empty;

			[Phone]
			public string? PhoneNumber { get; set; }
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");
			var (ok, _, dealer) = await DealerService.GetByIdAsync(dealerId.Value);
			DealerName = dealer?.Name;
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			if (!ModelState.IsValid) return Page();

			var (ok, err, user) = await authenService.RegisterAsync(
				fullName: Input.FullName,
				email: Input.Email,
				password: Guid.NewGuid().ToString("N").Substring(0, 10),
				phoneNumber: Input.PhoneNumber ?? string.Empty,
				address: string.Empty,
				role: BusinessLayer.Enums.UserRole.DealerStaff,
				dealerId: dealerId.Value);

			if (!ok)
			{
				ModelState.AddModelError(string.Empty, err ?? "Không thể tạo Dealer Staff");
				return Page();
			}

			TempData["Success"] = "Tạo Dealer Staff thành công";
			return RedirectToPage("/DealerManager/Index");
		}
	}
}


