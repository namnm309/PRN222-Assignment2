using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using BusinessLayer.DTOs.Responses;

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
            IBrandService brandService,
            IMappingService mappingService)
            : base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService, mappingService)
        {
        }

        public CustomerResponse Customer { get; set; } = new();
        public List<OrderResponse> Orders { get; set; } = new();

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

            Customer = new CustomerResponse
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };

            // Get customer's orders - filter by customer ID
            var (ok, err, result) = await OrderService.SearchAsync(dealerId.Value, null, null, null, 1, 100);
            if (ok)
            {
                Orders = result.Data.Where(o => o.CustomerId == id)
                    .Select(order => new OrderResponse
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
            }

            return Page();
        }
    }
}
