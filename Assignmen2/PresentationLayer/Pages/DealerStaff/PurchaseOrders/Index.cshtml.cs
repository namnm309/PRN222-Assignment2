using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PurchaseOrderEntity = DataAccessLayer.Entities.PurchaseOrder;

namespace PresentationLayer.Pages.DealerStaff.PurchaseOrders
{
    public class IndexModel : PageModel
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public IndexModel(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        public List<PurchaseOrderEntity> PurchaseOrders { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Get current dealer ID (you'll need to implement this)
            var dealerId = GetCurrentDealerId();
            
            if (dealerId.HasValue)
            {
                var result = await _purchaseOrderService.GetAllAsync(dealerId.Value);
                if (result.Success)
                {
                    PurchaseOrders = result.Data;

                    // Apply search filter
                    if (!string.IsNullOrWhiteSpace(Search))
                    {
                        var searchLower = Search.ToLower();
                        PurchaseOrders = PurchaseOrders.Where(o => 
                            o.OrderNumber.ToLower().Contains(searchLower) ||
                            (o.Product != null && o.Product.Name.ToLower().Contains(searchLower)) ||
                            (o.Product != null && o.Product.Sku.ToLower().Contains(searchLower))
                        ).ToList();
                    }

                    // Apply status filter
                    if (!string.IsNullOrWhiteSpace(Status))
                    {
                        if (Enum.TryParse<DataAccessLayer.Enum.PurchaseOrderStatus>(Status, out var statusEnum))
                        {
                            PurchaseOrders = PurchaseOrders.Where(o => o.Status == statusEnum).ToList();
                        }
                    }

                    // Apply date filter
                    if (FromDate.HasValue)
                    {
                        PurchaseOrders = PurchaseOrders.Where(o => o.RequestedDate.Date >= FromDate.Value.Date).ToList();
                    }

                    // Sort by requested date descending
                    PurchaseOrders = PurchaseOrders.OrderByDescending(o => o.RequestedDate).ToList();
                }
            }
        }

        private Guid? GetCurrentDealerId()
        {
            // TODO: Implement getting current dealer ID from session/authentication
            // For now, return a dummy ID - you'll need to implement this based on your authentication system
            return Guid.NewGuid();
        }
    }
}
