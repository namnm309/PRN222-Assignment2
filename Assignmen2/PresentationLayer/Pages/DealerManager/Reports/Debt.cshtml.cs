using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

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

		public List<DataAccessLayer.Entities.Customer> Customers { get; private set; } = new();
		public List<DataAccessLayer.Entities.Order> Orders { get; private set; } = new();
		public decimal TotalDebt { get; private set; }

		public async Task<IActionResult> OnGetAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			Customers = await debtService.GetDealerCustomersAsync(dealerId.Value);
			(var orders, var total) = await debtService.GetDebtReportAsync(dealerId.Value, CustomerId, PaymentStatus);
			Orders = orders;
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
			Customers = await debtService.GetDealerCustomersAsync(dealerId);
			(var orders, var total) = await debtService.GetDebtReportAsync(dealerId, CustomerId, PaymentStatus);
			Orders = orders;
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


