using System.ComponentModel.DataAnnotations;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using BusinessLayer.DTOs.Responses;

namespace PresentationLayer.Pages.DealerManager.PurchaseOrders
{
	public class CreateModel : BaseDealerManagerPageModel
	{
		private readonly IPurchaseOrderService purchaseOrderService;
		private readonly IProductService productService;

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
			IBrandService brandService,
			IMappingService mappingService)
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService, mappingService)
		{
			this.purchaseOrderService = purchaseOrderService;
			this.productService = productService;
		}

		[BindProperty]
		public InputModel Input { get; set; } = new();

        public List<ProductResponse> Products { get; private set; } = new();

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

            var (ok, _, products) = await productService.SearchAsync(null, null, null, null, null, true);
            Products = MappingService.MapToProductViewModels(products);

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			var userId = Guid.Parse(HttpContext.Session.GetString("UserId") ?? Guid.Empty.ToString());

			if (!ModelState.IsValid)
			{
				await OnGetAsync();
				return Page();
			}

			var (ok, err, po) = await purchaseOrderService.CreateAsync(
				dealerId.Value,
				Input.ProductId,
				userId,
				Input.Quantity,
				Input.UnitPrice,
				Input.Reason ?? string.Empty,
				Input.Notes ?? string.Empty,
				Input.ExpectedDeliveryDate
			);

			if (!ok)
			{
				ModelState.AddModelError(string.Empty, err ?? "Không thể tạo đơn đặt");
				await OnGetAsync();
				return Page();
			}

			TempData["Success"] = "Tạo đơn đặt thành công";
			return RedirectToPage("/DealerManager/PurchaseOrders/Index");
		}

		public async Task<IActionResult> OnGetGetProductStockAsync(Guid productId)
		{
			try
			{
                var (ok, _, product) = await productService.GetAsync(productId);
                if (!ok || product == null)
				{
					return new JsonResult(new { success = false, message = "Không tìm thấy sản phẩm" });
				}

                var hasStock = product.StockQuantity > 0;
				var minimumStock = 5; // You can make this configurable
                var availableQuantity = product.StockQuantity;
				var allocatedQuantity = 0; // TODO: Calculate from orders
				var reservedQuantity = 0; // TODO: Calculate from reservations

				var message = hasStock 
					? $"Có sẵn {availableQuantity} xe trong kho"
					: "Không còn xe trong kho";

				return new JsonResult(new
				{
					success = true,
					hasStock = hasStock,
					availableQuantity = availableQuantity,
					minimumStock = minimumStock,
					allocatedQuantity = allocatedQuantity,
					reservedQuantity = reservedQuantity,
					message = message
				});
			}
			catch (Exception ex)
			{
				return new JsonResult(new { success = false, message = $"Lỗi: {ex.Message}" });
			}
		}

		public class InputModel
		{
			[Required] public Guid ProductId { get; set; }
			[Required] public int Quantity { get; set; }
			[Required] public decimal UnitPrice { get; set; }
			public DateTime? ExpectedDeliveryDate { get; set; }
			public string? Reason { get; set; }
			public string? Notes { get; set; }
		}
	}
}

