using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using BusinessLayer.DTOs.Responses;

namespace PresentationLayer.Pages.DealerManager.Vehicles
{
	public class SearchModel : BaseDealerManagerPageModel
	{
		private readonly IProductService productService;
		private readonly IBrandService brandService;

		public SearchModel(
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
			this.brandService = brandService;
		}

		[BindProperty(SupportsGet = true)] public string? Q { get; set; }
		[BindProperty(SupportsGet = true)] public Guid? BrandId { get; set; }
		[BindProperty(SupportsGet = true)] public decimal? MinPrice { get; set; }
		[BindProperty(SupportsGet = true)] public decimal? MaxPrice { get; set; }
		[BindProperty(SupportsGet = true)] public bool? InStock { get; set; }

        public List<ProductResponse> Products { get; private set; } = new();
        public List<BrandResponse> Brands { get; private set; } = new();

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

            var (okB, _, brands) = await brandService.GetAllAsync();
            Brands = brands.Select(b => new BrandResponse { Id = b.Id, Name = b.Name, Country = b.Country, Description = b.Description, IsActive = b.IsActive }).ToList();

            var (okP, _, products) = await productService.SearchAsync(Q, BrandId, MinPrice, MaxPrice, InStock, true);
            Products = products.Select(p => new ProductResponse { Id = p.Id, Name = p.Name, Price = p.Price, StockQuantity = p.StockQuantity, BrandId = p.BrandId, BrandName = p.Brand?.Name ?? string.Empty }).ToList();

			return Page();
		}
	}
}

