using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager.Reports
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
			IBrandService brandService,
			IMappingService mappingService)
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService, mappingService) {}

		[BindProperty(SupportsGet = true)]
		public DateTime? FromDate { get; set; }

		[BindProperty(SupportsGet = true)]
		public DateTime? ToDate { get; set; }

		[BindProperty(SupportsGet = true)]
		public string Period { get; set; } = "monthly"; // monthly|quarterly|yearly

		[BindProperty(SupportsGet = true)]
		public int Year { get; set; } = DateTime.Today.Year;

		[BindProperty(SupportsGet = true)]
		public int? Month { get; set; } = DateTime.Today.Month;

		[BindProperty(SupportsGet = true)]
		public int? Quarter { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			if (Year == 0) Year = DateTime.Today.Year;
			if (Period == "monthly" && Month == null) Month = DateTime.Today.Month;

		var currentSales = await ReportService.GetSalesReportByRegionAsync(null, dealerId, Period, Year, Month, Quarter);
		var yearlySales = await ReportService.GetSalesReportByRegionAsync(null, dealerId, "yearly", Year, null, null);

		CurrentTotal = currentSales.Sum(o => o.FinalAmount);
		YearTotal = yearlySales.Sum(o => o.FinalAmount);

		// Top nhân viên
		var staffSales = await ReportService.GetSalesReportByStaffAsync(null, dealerId, Period, Year, Month, Quarter);
		TopEmployees = staffSales
			.Where(o => o.SalesPersonId.HasValue)
			.GroupBy(o => new { o.SalesPersonId, Name = o.SalesPerson?.FullName })
			.Select(g => new TopEmployeeVm(
				g.Key.SalesPersonId!.Value, 
				g.Key.Name ?? "N/A", 
				g.Count(), 
				g.Sum(x => x.FinalAmount)))
			.OrderByDescending(x => x.TotalSales)
			.Take(10)
			.ToList();

		// Top xe bán chạy
		TopVehicles = currentSales
			.GroupBy(o => new { o.ProductId, Name = o.Product?.Name, Sku = o.Product?.Sku })
			.Select(g => new TopVehicleVm(
				g.Key.ProductId, 
				g.Key.Name ?? "N/A", 
				g.Key.Sku ?? "N/A",
				g.Count(), 
				g.Sum(x => x.FinalAmount)))
			.OrderByDescending(x => x.TotalSales)
			.Take(10)
			.ToList();
			return Page();
		}

	public decimal CurrentTotal { get; private set; }
	public decimal YearTotal { get; private set; }

	public List<TopEmployeeVm> TopEmployees { get; private set; } = new();
	public List<TopVehicleVm> TopVehicles { get; private set; } = new();

	public record TopEmployeeVm(Guid EmployeeId, string EmployeeName, int OrderCount, decimal TotalSales);
	public record TopVehicleVm(Guid ProductId, string ProductName, string ProductSku, int QuantitySold, decimal TotalSales);
	}
}


