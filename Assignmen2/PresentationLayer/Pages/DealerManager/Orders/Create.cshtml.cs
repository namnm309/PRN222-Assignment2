using System.ComponentModel.DataAnnotations;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using BusinessLayer.DTOs.Responses;
using DataAccessLayer.Entities;

namespace PresentationLayer.Pages.DealerManager.Orders
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
		private readonly IProductService productService;
		private readonly IInventoryManagementService inventoryService;
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
			IInventoryManagementService inventoryService,
			IPricingManagementService pricingService)
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService, mappingService)
		{
			this.productService = productService;
			this.inventoryService = inventoryService;
			this.pricingService = pricingService;
		}

		[BindProperty]
		public InputModel Input { get; set; } = new();

        public List<ProductResponse> Products { get; private set; } = new();
        public List<CustomerResponse> Customers { get; private set; } = new();
        public List<UserResponse> Staff { get; private set; } = new();
        public Dictionary<Guid, PricingInfo> ProductPricing { get; set; } = new();

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

            var (okP, _, products) = await productService.SearchAsync(null, null, null, null, null, true);
            Products = products.Select(p => new ProductResponse { Id = p.Id, Name = p.Name, Price = p.Price, StockQuantity = p.StockQuantity, BrandId = p.BrandId, BrandName = p.Brand?.Name ?? string.Empty }).ToList();

            var (okC, _, customers) = await CustomerService.GetAllByDealerAsync(dealerId.Value);
            Customers = customers.Select(c => new CustomerResponse { Id = c.Id, FullName = c.FullName, Email = c.Email, PhoneNumber = c.PhoneNumber, Address = c.Address, IsActive = c.IsActive, CreatedAt = c.CreatedAt, UpdatedAt = c.UpdatedAt }).ToList();

            var staffEntities = await ReportService.GetUsersByDealerAsync(dealerId.Value);
            Staff = staffEntities.Select(u => new UserResponse { Id = u.Id, FullName = u.FullName, Email = u.Email, PhoneNumber = u.PhoneNumber ?? string.Empty, Role = BusinessLayer.Enums.UserRole.DealerStaff, DealerId = u.DealerId, DealerName = u.Dealer?.Name ?? string.Empty, IsActive = u.IsActive, CreatedAt = u.CreatedAt, UpdatedAt = u.UpdatedAt }).ToList();

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

			if (!ModelState.IsValid)
			{
				await OnGetAsync();
				return Page();
			}

			// Kiểm tra tồn kho trước khi tạo đơn hàng
			var inventoryResult = await inventoryService.GetInventoryByDealerAndProductAsync(dealerId.Value, Input.ProductId);
			if (!inventoryResult.Success || inventoryResult.Data == null || inventoryResult.Data.AvailableQuantity <= 0)
			{
				ModelState.AddModelError(string.Empty, "Không có xe trong kho. Vui lòng liên hệ EVM để đặt hàng hoặc chọn sản phẩm khác.");
				await OnGetAsync();
				return Page();
			}

			// Tính giá theo PriceType
			decimal finalPrice = Input.Price;
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

			var (ok, err, order) = await OrderService.CreateQuotationAsync(
				Input.ProductId,
				Input.CustomerId,
				dealerId.Value,
				Input.SalesPersonId,
				finalPrice,
				Input.Discount,
				Input.Description ?? string.Empty,
				Input.Notes ?? string.Empty
			);

			if (!ok)
			{
				ModelState.AddModelError(string.Empty, err ?? "Không thể tạo báo giá");
				await OnGetAsync();
				return Page();
			}

			TempData["Success"] = "Tạo báo giá thành công";
			return RedirectToPage("/DealerManager/Orders/Index");
		}

		public class InputModel
		{
			[Required] public Guid ProductId { get; set; }
			[Required] public Guid CustomerId { get; set; }
			[Required] public decimal Price { get; set; }
			public decimal Discount { get; set; }
			public Guid? SalesPersonId { get; set; }
			public string? Description { get; set; }
			public string? Notes { get; set; }
			[Required] public string PriceType { get; set; } = "Wholesale"; // Wholesale or Retail
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

				// Get product info
				var (ok, err, product) = await productService.GetAsync(productId);
				if (!ok)
				{
					return new JsonResult(new { success = false, message = err ?? "Không thể tìm thấy sản phẩm" });
				}

				// Kiểm tra tồn kho đã phân bổ cho đại lý
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
						message = "Không có xe trong kho. Vui lòng liên hệ EVM để đặt hàng.";
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
						message = "Chưa có xe được phân bổ cho đại lý này. Vui lòng liên hệ EVM để đặt hàng.",
						isEVMStock = false
					});
				}
			}
			catch (Exception ex)
			{
				return new JsonResult(new { success = false, message = $"Lỗi: {ex.Message}" });
			}
		}
	}
}

