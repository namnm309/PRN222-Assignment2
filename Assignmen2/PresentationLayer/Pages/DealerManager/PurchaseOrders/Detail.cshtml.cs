using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager.PurchaseOrders
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

        public DataAccessLayer.Entities.PurchaseOrder PurchaseOrder { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var dealerId = GetCurrentDealerId();
            if (dealerId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var (success, error, purchaseOrder) = await PurchaseOrderService.GetAsync(id);
            if (!success || purchaseOrder == null || purchaseOrder.DealerId != dealerId)
            {
                return NotFound();
            }

            PurchaseOrder = purchaseOrder;
            return Page();
        }
    }
}
