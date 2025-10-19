using System.ComponentModel.DataAnnotations;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using BusinessLayer.DTOs.Responses;

namespace PresentationLayer.Pages.DealerManager.Orders
{
	public class CreateModel : BaseDealerManagerPageModel
	{
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
			IBrandService brandService)
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService)
		{
			this.productService = productService;
		}

		[BindProperty]
		public InputModel Input { get; set; } = new();

        public List<ProductResponse> Products { get; private set; } = new();
        public List<CustomerResponse> Customers { get; private set; } = new();
        public List<UserResponse> Staff { get; private set; } = new();

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

			var (ok, err, order) = await OrderService.CreateQuotationAsync(
				Input.ProductId,
				Input.CustomerId,
				dealerId.Value,
				Input.SalesPersonId,
				Input.Price,
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

				var stockQuantity = product.StockQuantity;
				var hasStock = stockQuantity > 0;
				
				string message;
				if (hasStock)
				{
					if (stockQuantity < 5)
					{
						message = $"Tồn kho thấp! Chỉ còn {stockQuantity} xe trong kho";
					}
					else
					{
						message = $"Có sẵn {stockQuantity} xe trong kho";
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
					availableQuantity = stockQuantity,
					allocatedQuantity = 0,
					reservedQuantity = 0,
					minimumStock = 0,
					message = message,
					isEVMStock = true
				});
			}
			catch (Exception ex)
			{
				return new JsonResult(new { success = false, message = $"Lỗi: {ex.Message}" });
			}
		}
	}
}

