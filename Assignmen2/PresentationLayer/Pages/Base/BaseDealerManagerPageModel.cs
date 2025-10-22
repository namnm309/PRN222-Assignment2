using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Security.Claims;

namespace PresentationLayer.Pages.Base
{
	[Authorize(Roles = "DealerManager")]
	public class BaseDealerManagerPageModel : PageModel
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

		public BaseDealerManagerPageModel(
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

		protected Guid? GetCurrentDealerId()
		{
			// Ưu tiên lấy claim tuỳ chỉnh "DealerId" (được gán khi login)
			var dealerIdClaim = User.FindFirst("DealerId")?.Value 
				?? User.FindFirst(ClaimTypes.GroupSid)?.Value; // fallback cho legacy
			return Guid.TryParse(dealerIdClaim, out var dealerId) ? dealerId : null;
		}
	}
}
