using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager.Orders
{
    public class DetailModel : BaseDealerManagerPageModel
    {
        public DetailModel(
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
        }

        public Order Order { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var dealerId = GetCurrentDealerId();
            if (dealerId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var (success, error, order) = await OrderService.GetAsync(id);
            if (!success || order == null || order.DealerId != dealerId)
            {
                return NotFound();
            }

            Order = order;
            return Page();
        }
    }
}
