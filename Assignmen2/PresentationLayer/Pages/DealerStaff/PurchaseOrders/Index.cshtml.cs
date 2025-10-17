using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.PurchaseOrders
{
    public class IndexModel : PageModel
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IMappingService _mappingService;

        public IndexModel(IPurchaseOrderService purchaseOrderService, IMappingService mappingService)
        {
            _purchaseOrderService = purchaseOrderService;
            _mappingService = mappingService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        public List<PurchaseOrderResponse> PurchaseOrders { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Get current dealer ID from session
            var dealerId = GetCurrentDealerId();
            
            if (dealerId.HasValue)
            {
                var result = await _purchaseOrderService.GetAllAsync(dealerId.Value);
                if (result.Success && result.Data != null)
                {
                    // Map entities to DTOs using mapping service
                    PurchaseOrders = _mappingService.MapToPurchaseOrderViewModels(result.Data);

                    // Apply search filter
                    if (!string.IsNullOrWhiteSpace(Search))
                    {
                        var searchLower = Search.ToLower();
                        PurchaseOrders = PurchaseOrders.Where(o => 
                            o.OrderNumber.ToLower().Contains(searchLower) ||
                            o.ProductName.ToLower().Contains(searchLower) ||
                            o.ProductSku.ToLower().Contains(searchLower)
                        ).ToList();
                    }

                    // Apply status filter
                    if (!string.IsNullOrWhiteSpace(Status))
                    {
                        if (Enum.TryParse<BusinessLayer.Enums.PurchaseOrderStatus>(Status, out var statusEnum))
                        {
                            PurchaseOrders = PurchaseOrders.Where(o => o.Status.ToString() == Status).ToList();
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
                else
                {
                    PurchaseOrders = new List<PurchaseOrderResponse>();
                    TempData["Error"] = result.Error ?? "Không thể tải danh sách đơn đặt hàng";
                }
            }
            else
            {
                PurchaseOrders = new List<PurchaseOrderResponse>();
                TempData["Error"] = "Không xác định được đại lý hiện tại";
            }
        }

        private Guid? GetCurrentDealerId()
        {
            // Get dealer ID from session
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            if (Guid.TryParse(dealerIdString, out var dealerId))
            {
                return dealerId;
            }
            return null;
        }
    }
}
