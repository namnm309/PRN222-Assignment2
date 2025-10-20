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
			IBrandService brandService,
			IMappingService mappingService)
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService, mappingService)
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
            Brands = MappingService.MapToBrandViewModels(brands);

            var (okP, _, products) = await productService.SearchAsync(Q, BrandId, MinPrice, MaxPrice, InStock, true);
            Products = MappingService.MapToProductViewModels(products);

			return Page();
		}
	}
}

