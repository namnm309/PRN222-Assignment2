using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using BusinessLayer.DTOs.Responses;

namespace PresentationLayer.Pages.DealerManager.Reports
{
	public class DebtModel : BaseDealerManagerPageModel
	{
		private readonly IDealerDebtService debtService;

		public DebtModel(
			IDealerService dealerService,
			IOrderService orderService,
			ITestDriveService testDriveService,
			ICustomerService customerService,
			IEVMReportService reportService,
			IDealerDebtService debtService,
			IAuthenService authenService,
			IPurchaseOrderService purchaseOrderService,
			IProductService productService,
			IBrandService brandService)
			: base(dealerService, orderService, testDriveService, customerService, reportService, debtService, authenService, purchaseOrderService, productService, brandService)
		{
			this.debtService = debtService;
		}

		[BindProperty(SupportsGet = true)]
		public Guid? CustomerId { get; set; }

		[BindProperty(SupportsGet = true)]
		public string? PaymentStatus { get; set; }

        public List<CustomerResponse> Customers { get; private set; } = new();
        public List<OrderResponse> Orders { get; private set; } = new();
		public decimal TotalDebt { get; private set; }

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

            var customersEntity = await debtService.GetDealerCustomersAsync(dealerId.Value);
            Customers = customersEntity.Select(c => new CustomerResponse { Id = c.Id, FullName = c.FullName, Email = c.Email, PhoneNumber = c.PhoneNumber, Address = c.Address, IsActive = c.IsActive, CreatedAt = c.CreatedAt, UpdatedAt = c.UpdatedAt }).ToList();
            (var orders, var total) = await debtService.GetDebtReportAsync(dealerId.Value, CustomerId, PaymentStatus);
            Orders = orders.Select(order => new OrderResponse
            {
                Id = order.Id,
                DealerId = order.DealerId,
                ProductId = order.ProductId,
                CustomerId = order.CustomerId,
                SalesPersonId = order.SalesPersonId,
                Description = order.Description,
                Price = order.Price,
                Discount = order.Discount,
                Notes = order.Notes,
                OrderNumber = order.OrderNumber,
                FinalAmount = order.FinalAmount,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate,
                PaymentDueDate = order.PaymentDueDate,
                CustomerName = order.Customer?.FullName ?? string.Empty,
                ProductName = order.Product?.Name ?? string.Empty,
                DealerName = order.Dealer?.Name ?? string.Empty,
                SalesPersonName = order.SalesPerson?.FullName ?? string.Empty
            }).ToList();
			TotalDebt = total;
			return Page();
		}

		[BindProperty]
		public PaymentInput PaymentInput { get; set; } = new();

		[BindProperty]
		public ExtendInput ExtendInput { get; set; } = new();

		public async Task<IActionResult> OnPostPaymentAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			if (!ModelState.IsValid)
			{
				await LoadDataAsync(dealerId.Value);
				return Page();
			}

			try
			{
				var (ok, err) = await debtService.ProcessPaymentAsync(PaymentInput.OrderId, PaymentInput.Amount, PaymentInput.Method, PaymentInput.Note);
				if (!ok)
				{
					ModelState.AddModelError(string.Empty, err ?? "Không thể xử lý thanh toán");
					await LoadDataAsync(dealerId.Value);
					return Page();
				}
				TempData["SuccessMessage"] = $"Thanh toán {PaymentInput.Amount:N0} VND thành công!";
				return RedirectToPage();
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"Lỗi khi xử lý thanh toán: {ex.Message}");
				await LoadDataAsync(dealerId.Value);
				return Page();
			}
		}

		public async Task<IActionResult> OnPostExtendAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			if (!ModelState.IsValid)
			{
				await LoadDataAsync(dealerId.Value);
				return Page();
			}

			try
			{
				var (ok, err) = await debtService.ExtendPaymentAsync(ExtendInput.OrderId, ExtendInput.ExtendDate, ExtendInput.Reason);
				if (!ok)
				{
					ModelState.AddModelError(string.Empty, err ?? "Không thể gia hạn");
					await LoadDataAsync(dealerId.Value);
					return Page();
				}
				TempData["SuccessMessage"] = $"Gia hạn thanh toán đến {ExtendInput.ExtendDate:dd/MM/yyyy} thành công!";
				return RedirectToPage();
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"Lỗi khi gia hạn: {ex.Message}");
				await LoadDataAsync(dealerId.Value);
				return Page();
			}
		}

		private async Task LoadDataAsync(Guid dealerId)
		{
            var customersEntity = await debtService.GetDealerCustomersAsync(dealerId);
            Customers = customersEntity.Select(c => new CustomerResponse { Id = c.Id, FullName = c.FullName, Email = c.Email, PhoneNumber = c.PhoneNumber, Address = c.Address, IsActive = c.IsActive, CreatedAt = c.CreatedAt, UpdatedAt = c.UpdatedAt }).ToList();
            (var orders, var total) = await debtService.GetDebtReportAsync(dealerId, CustomerId, PaymentStatus);
            Orders = orders.Select(order => new OrderResponse
            {
                Id = order.Id,
                DealerId = order.DealerId,
                ProductId = order.ProductId,
                CustomerId = order.CustomerId,
                SalesPersonId = order.SalesPersonId,
                Description = order.Description,
                Price = order.Price,
                Discount = order.Discount,
                Notes = order.Notes,
                OrderNumber = order.OrderNumber,
                FinalAmount = order.FinalAmount,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate,
                PaymentDueDate = order.PaymentDueDate,
                CustomerName = order.Customer?.FullName ?? string.Empty,
                ProductName = order.Product?.Name ?? string.Empty,
                DealerName = order.Dealer?.Name ?? string.Empty,
                SalesPersonName = order.SalesPerson?.FullName ?? string.Empty
            }).ToList();
			TotalDebt = total;
		}
	}

	public class PaymentInput
	{
		public Guid OrderId { get; set; }
		public decimal Amount { get; set; }
		public string Method { get; set; } = string.Empty;
		public string? Note { get; set; }
	}

	public class ExtendInput
	{
		public Guid OrderId { get; set; }
		public DateTime ExtendDate { get; set; }
		public string? Reason { get; set; }
	}
}


