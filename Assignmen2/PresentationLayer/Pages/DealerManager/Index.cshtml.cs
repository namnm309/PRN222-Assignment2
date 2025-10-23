using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager
{
	public class IndexModel : BaseDealerManagerPageModel
	{
		public string? DealerName { get; private set; }
		public int MonthlyOrders { get; private set; }
		public decimal MonthlyRevenue { get; private set; }
		public int MonthlyTestDrives { get; private set; }
		public int NewCustomers { get; private set; }

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
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService, mappingService)
		{
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null)
			{
				// Không có DealerId trong session -> chuyển về Dashboard chung
				return RedirectToPage("/Dashboard/Index");
			}

			var (ok, _, dealer) = await DealerService.GetByIdAsync(dealerId.Value);
			DealerName = dealer?.Name;

			// Sử dụng timezone Việt Nam cho tính toán tháng
			var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
			var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone).Date;
			var year = today.Year;
			var month = today.Month;
			
			var sales = await ReportService.GetSalesReportByRegionAsync(null, dealerId, "monthly", year, month, null);
			MonthlyRevenue = sales.Sum(o => o.FinalAmount);
			MonthlyOrders = sales.Count;

			var (tdOk, _, tds) = await TestDriveService.GetAllAsync(dealerId, null);
			MonthlyTestDrives = tds.Count(td => 
			{
				var tdDate = TimeZoneInfo.ConvertTimeFromUtc(td.ScheduledDate.ToUniversalTime(), vnTimeZone);
				return tdDate.Month == month && tdDate.Year == year;
			});

			var (custOk, _, customers) = await CustomerService.GetAllByDealerAsync(dealerId.Value);
			var customerResponses = MappingService.MapToCustomerViewModels(customers);
			NewCustomers = customerResponses.Count(c => 
			{
				var custDate = TimeZoneInfo.ConvertTimeFromUtc(c.CreatedAt.ToUniversalTime(), vnTimeZone);
				return custDate.Month == month && custDate.Year == year;
			});

			// Set ViewData for layout
			ViewData["DealerName"] = DealerName;
			return Page();
		}

		// API endpoints cho biểu đồ
		public async Task<IActionResult> OnGetSalesChartDataAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return new JsonResult(new { error = "Dealer not found" });

			var chartData = await ReportService.GetSalesChartDataAsync(dealerId.Value);
			return new JsonResult(chartData);
		}

		public async Task<IActionResult> OnGetInventoryChartDataAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return new JsonResult(new { error = "Dealer not found" });

			var chartData = await ReportService.GetInventoryChartDataAsync(dealerId.Value);
			return new JsonResult(chartData);
		}

		public async Task<IActionResult> OnGetDebtChartDataAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return new JsonResult(new { error = "Dealer not found" });

			var chartData = await ReportService.GetDebtChartDataAsync(dealerId.Value);
			return new JsonResult(chartData);
		}

		public async Task<IActionResult> OnGetOrdersChartDataAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return new JsonResult(new { error = "Dealer not found" });

			var chartData = await ReportService.GetOrdersChartDataAsync(dealerId.Value);
			return new JsonResult(chartData);
		}
	}
}


