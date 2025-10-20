using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using BusinessLayer.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.Pages.DealerManager.Users
{
	public class DetailModel : BaseDealerManagerPageModel
	{
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
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService)
		{
		}

		public UserVm User { get; set; } = new();
		public int TotalOrders { get; set; }
		public decimal TotalSales { get; set; }

		[BindProperty] public Guid UserId { get; set; }
		[BindProperty] public string Action { get; set; } = string.Empty;

		public async Task<IActionResult> OnGetAsync(Guid id)
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			var users = await ReportService.GetUsersByDealerAsync(dealerId.Value);
			var user = users.FirstOrDefault(u => u.Id == id);
			
			if (user == null) return NotFound();

			User = new UserVm
			{
				Id = user.Id,
				FullName = user.FullName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
                Role = (UserRole)user.Role,
				DealerName = user.Dealer?.Name,
				IsActive = user.IsActive
			};

			// Get user's orders and sales statistics
			var (ok, err, result) = await OrderService.SearchAsync(dealerId.Value, null, null, null, 1, 1000);
			if (ok)
			{
				var userOrders = result.Data.Where(o => o.SalesPersonId == id).ToList();
				TotalOrders = userOrders.Count;
				TotalSales = userOrders.Sum(o => o.FinalAmount);
			}

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

			return RedirectToPage(new { id = UserId });
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
