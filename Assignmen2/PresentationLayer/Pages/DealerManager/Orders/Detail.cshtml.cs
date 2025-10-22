using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using BusinessLayer.DTOs.Responses;

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
            IBrandService brandService,
			IMappingService mappingService)
            : base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService, mappingService)
        {
        }

        public OrderResponse Order { get; set; } = new();

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

            // Assuming OrderService returns entity; map minimally for now
            Order = new OrderResponse
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
            };
            return Page();
        }
    }
}
