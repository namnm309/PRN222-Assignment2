using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLayer.Enums;

namespace PresentationLayer.Pages.Base
{
	[Authorize(Roles = "DealerStaff")]
	public class BaseDealerStaffPageModel : PageModel
	{
		protected readonly IDealerService DealerService;
		protected readonly IOrderService OrderService;
		protected readonly ITestDriveService TestDriveService;
		protected readonly ICustomerService CustomerService;
		protected readonly IEVMReportService ReportService;
		protected readonly IDealerDebtService DealerDebtService;
		protected readonly IAuthenService AuthenService;
		protected readonly IPurchaseOrderService PurchaseOrderService;
		protected readonly IProductService ProductService;
		protected readonly IBrandService BrandService;
		protected readonly IMappingService MappingService;

		public BaseDealerStaffPageModel(
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
		{
			DealerService = dealerService;
			OrderService = orderService;
			TestDriveService = testDriveService;
			CustomerService = customerService;
			ReportService = reportService;
			DealerDebtService = dealerDebtService;
			AuthenService = authenService;
			PurchaseOrderService = purchaseOrderService;
			ProductService = productService;
			BrandService = brandService;
			MappingService = mappingService;
		}

		public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
		{
			base.OnPageHandlerExecuting(context);

			// Lấy thông tin user từ session
			var roleString = HttpContext.Session.GetString("UserRole");
			var userName = HttpContext.Session.GetString("UserFullName");
			var userEmail = HttpContext.Session.GetString("UserEmail");
			var userIdString = HttpContext.Session.GetString("UserId");

			// ✅ KIỂM TRA ĐĂNG NHẬP
			if (string.IsNullOrEmpty(roleString) || string.IsNullOrEmpty(userName))
			{
				TempData["Error"] = "Vui lòng đăng nhập để truy cập trang này.";
				context.Result = RedirectToPage("/Account/Login", new { returnUrl = HttpContext.Request.Path });
				return;
			}

			// Parse role
			UserRole? userRole = null;
			if (System.Enum.TryParse<UserRole>(roleString, out var role))
			{
				userRole = role;
			}

			// ✅ KIỂM TRA QUYỀN - CHỈ DealerStaff và DealerManager mới được truy cập
			if (userRole != UserRole.DealerStaff && userRole != UserRole.DealerManager)
			{
				TempData["Error"] = "Bạn không có quyền truy cập trang này.";
				context.Result = RedirectToPage("/Dashboard/Index");
				return;
			}

			// Set ViewData cho views
			ViewData["UserRole"] = userRole;
			ViewData["UserRoleName"] = userRole.ToString();
			ViewData["UserName"] = userName;
			ViewData["UserEmail"] = userEmail ?? "";

			// Lưu DealerId để sử dụng sau này
			var dealerId = GetCurrentDealerId();
			if (dealerId.HasValue)
			{
				ViewData["DealerId"] = dealerId.Value;
			}
		}

		/// <summary>
		/// Method để set ViewData cho DealerName - cần được gọi trong OnGetAsync của các page con
		/// </summary>
		protected async Task SetDealerNameViewDataAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId.HasValue)
			{
				try
				{
					var (dealerOk, _, dealer) = await DealerService.GetByIdAsync(dealerId.Value);
					if (dealerOk && dealer != null)
					{
						ViewData["DealerName"] = dealer.Name;
					}
				}
				catch (Exception ex)
				{
					// Log error but don't break the page
					System.Diagnostics.Debug.WriteLine($"Error loading dealer info: {ex.Message}");
				}
			}
		}

		protected Guid? GetCurrentDealerId()
		{
			// Ưu tiên lấy claim tuỳ chỉnh "DealerId" (được gán khi login)
			var dealerIdClaim = User.FindFirst("DealerId")?.Value 
				?? User.FindFirst(ClaimTypes.GroupSid)?.Value; // fallback cho legacy
			return Guid.TryParse(dealerIdClaim, out var dealerId) ? dealerId : null;
		}
	}
}
