using System.ComponentModel.DataAnnotations;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager.Customers
{
	public class CreateFromTestDriveModel : BaseDealerManagerPageModel
	{
		public CreateFromTestDriveModel(
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

		[BindProperty(SupportsGet = true)]
		public Guid TestDriveId { get; set; }

		[BindProperty]
		public InputModel Input { get; set; } = new();

		public class InputModel
		{
			[Required]
			[StringLength(100)]
			public string FullName { get; set; } = string.Empty;

			[EmailAddress]
			public string? Email { get; set; }

			[Phone]
			public string? PhoneNumber { get; set; }
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

            var (ok, _, td) = await TestDriveService.GetAsync(TestDriveId);
			if (!ok || td == null || td.DealerId != dealerId) return RedirectToPage("/DealerManager/TestDrives/Index");
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");
			if (!ModelState.IsValid) return Page();

			var (ok, err, customer) = await CustomerService.CreateAsync(
				fullName: Input.FullName,
				email: Input.Email ?? string.Empty,
				phoneNumber: Input.PhoneNumber ?? string.Empty,
				address: string.Empty);

			if (!ok)
			{
				ModelState.AddModelError(string.Empty, err ?? "Không thể tạo khách hàng");
				return Page();
			}

			// Cập nhật test drive để liên kết với customer mới tạo
			var (tdOk, tdErr, testDrive) = await TestDriveService.GetAsync(TestDriveId);
			if (tdOk && testDrive != null)
			{
				// Cập nhật test drive để liên kết với customer
				var (updateOk, updateErr) = await TestDriveService.UpdateCustomerAsync(TestDriveId, customer.Id);
				if (!updateOk)
				{
					// Log lỗi nhưng không fail toàn bộ process
					Console.WriteLine($"Warning: Could not link customer to test drive: {updateErr}");
				}
			}

			TempData["Success"] = "Tạo khách hàng thành công";
			return RedirectToPage("/DealerManager/TestDrives/Detail", new { id = TestDriveId });
		}
	}
}


