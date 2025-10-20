 using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using BusinessLayer.Enums;

namespace PresentationLayer.Pages.DealerManager.Users
{
	public class IndexModel : BaseDealerManagerPageModel
	{
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
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService) { }

		public List<UserVm> Users { get; private set; } = new();

		[BindProperty] public Guid UserId { get; set; }
		[BindProperty] public string Action { get; set; } = string.Empty;

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			var users = await ReportService.GetUsersByDealerAsync(dealerId.Value);
			Users = users.Select(u => new UserVm
			{
				Id = u.Id,
				FullName = u.FullName,
				Email = u.Email,
				PhoneNumber = u.PhoneNumber,
                Role = (UserRole)u.Role,
				DealerName = u.Dealer?.Name,
				IsActive = u.IsActive
			}).ToList();

			return Page();
		}

		public async Task<IActionResult> OnPostToggleUserStatusAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Account/Login");

			var isActive = Action == "unlock";
			var (success, error) = await AuthenService.ToggleUserStatusAsync(UserId, isActive);
			if (success)
			{
				var actionText = Action == "lock" ? "khóa" : "mở khóa";
				TempData["SuccessMessage"] = $"Đã {actionText} tài khoản thành công!";
			}
			else
			{
				TempData["ErrorMessage"] = $"Lỗi khi thay đổi trạng thái tài khoản: {error}";
			}

			return RedirectToPage();
		}

		public class UserVm
		{
			public Guid Id { get; set; }
			public string FullName { get; set; } = string.Empty;
			public string Email { get; set; } = string.Empty;
			public string? PhoneNumber { get; set; }
            public UserRole Role { get; set; }
			public string? DealerName { get; set; }
			public bool IsActive { get; set; }
		}
	}
}

