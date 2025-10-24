using System.ComponentModel.DataAnnotations;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using BusinessLayer.DTOs.Responses;
using DataAccessLayer.Entities;

namespace PresentationLayer.Pages.DealerManager.PurchaseOrders
{
	public class PricingInfo
	{
		public decimal WholesalePrice { get; set; }
		public decimal RetailPrice { get; set; }
		public decimal DiscountRate { get; set; }
		public decimal MinimumPrice { get; set; }
		public string PolicyType { get; set; } = string.Empty;
		public bool HasPolicy { get; set; }
	}

	public class CreateModel : BaseDealerManagerPageModel
	{
		private readonly IPurchaseOrderService purchaseOrderService;
		private readonly IProductService productService;
		private readonly IPricingManagementService pricingService;

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
			IMappingService mappingService,
			IPricingManagementService pricingService)
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService, mappingService)
		{
			this.purchaseOrderService = purchaseOrderService;
			this.productService = productService;
			this.pricingService = pricingService;
		}

		[BindProperty]
		public InputModel Input { get; set; } = new();

        public List<ProductResponse> Products { get; private set; } = new();
        public Dictionary<Guid, PricingInfo> ProductPricing { get; set; } = new();

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

            var (ok, _, products) = await productService.SearchAsync(null, null, null, null, null, true);
            Products = MappingService.MapToProductViewModels(products);

			// Load pricing information for each product
			var (dealerOk, _, dealer) = await DealerService.GetByIdAsync(dealerId.Value);
			if (dealerOk && dealer != null)
			{
				foreach (var product in products)
				{
					var pricingPolicy = await pricingService.GetActivePricingPolicyAsync(product.Id, dealerId.Value, dealer.RegionId);
					
					if (pricingPolicy != null)
					{
						ProductPricing[product.Id] = new PricingInfo
						{
							WholesalePrice = pricingPolicy.WholesalePrice,
							RetailPrice = pricingPolicy.RetailPrice,
							DiscountRate = pricingPolicy.DiscountRate,
							MinimumPrice = pricingPolicy.MinimumPrice,
							PolicyType = pricingPolicy.PolicyType,
							HasPolicy = true
						};
					}
					else
					{
						// Fallback to product base price
						ProductPricing[product.Id] = new PricingInfo
						{
							WholesalePrice = product.Price,
							RetailPrice = product.Price,
							DiscountRate = 0,
							MinimumPrice = product.Price,
							PolicyType = "Standard",
							HasPolicy = false
						};
					}
				}
			}

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

			// Tính giá theo PriceType
			decimal finalPrice = Input.UnitPrice;
			if (ProductPricing.ContainsKey(Input.ProductId))
			{
				var pricing = ProductPricing[Input.ProductId];
				finalPrice = Input.PriceType == "Retail" ? pricing.RetailPrice : pricing.WholesalePrice;
				
				// Apply discount nếu có
				if (pricing.DiscountRate > 0)
				{
					finalPrice = finalPrice * (1 - pricing.DiscountRate / 100);
					finalPrice = Math.Max(finalPrice, pricing.MinimumPrice);
				}
			}

			var (ok, err, po) = await purchaseOrderService.CreateAsync(
				dealerId.Value,
				Input.ProductId,
				userId,
				Input.Quantity,
				finalPrice,
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
				var dealerId = GetCurrentDealerId();
				if (dealerId == null)
				{
					return new JsonResult(new { success = false, message = "Không tìm thấy thông tin đại lý" });
				}

				// Kiểm tra tồn kho đã phân bổ cho đại lý
				var inventoryService = HttpContext.RequestServices.GetRequiredService<IInventoryManagementService>();
				var inventoryResult = await inventoryService.GetInventoryByDealerAndProductAsync(dealerId.Value, productId);
				
				if (inventoryResult.Success && inventoryResult.Data != null)
				{
					// Có allocation cho đại lý
					var inventory = inventoryResult.Data;
					var hasStock = inventory.AvailableQuantity > 0;
					
					string message;
					if (hasStock)
					{
						if (inventory.AvailableQuantity <= inventory.MinimumStock)
						{
							message = $"Tồn kho thấp! Chỉ còn {inventory.AvailableQuantity} xe (tối thiểu: {inventory.MinimumStock} xe)";
						}
						else
						{
							message = $"Có sẵn {inventory.AvailableQuantity} xe trong kho";
						}
					}
					else
					{
						message = "Không có xe trong kho. Vui lòng đặt hàng từ EVM.";
					}

					return new JsonResult(new
					{
						success = true,
						hasStock = hasStock,
						availableQuantity = inventory.AvailableQuantity,
						allocatedQuantity = inventory.AllocatedQuantity,
						reservedQuantity = inventory.ReservedQuantity,
						minimumStock = inventory.MinimumStock,
						message = message,
						isEVMStock = false
					});
				}
				else
				{
					// Không có allocation cho đại lý này
					return new JsonResult(new { 
						success = true,
						hasStock = false,
						availableQuantity = 0,
						allocatedQuantity = 0,
						reservedQuantity = 0,
						minimumStock = 0,
						message = "Chưa có xe được phân bổ cho đại lý này. Vui lòng đặt hàng từ EVM.",
						isEVMStock = false
					});
				}
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
			[Required] public string PriceType { get; set; } = "Wholesale"; // Wholesale or Retail
		}
	}
}

