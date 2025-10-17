using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager.Customers
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

        public Customer Customer { get; set; } = new();
        public List<Order> Orders { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var dealerId = GetCurrentDealerId();
            if (dealerId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var (success, error, customer) = await CustomerService.GetAsync(id);
            if (!success || customer == null)
            {
                return NotFound();
            }

            Customer = customer;

            // Get customer's orders - filter by customer ID
            var (ok, err, result) = await OrderService.SearchAsync(dealerId.Value, null, null, null, 1, 100);
            if (ok)
            {
                Orders = result.Data.Where(o => o.CustomerId == id).ToList();
            }

            return Page();
        }
    }
}
