using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IMappingService _mappingService;

        public IndexModel(IOrderService orderService, IMappingService mappingService)
        {
            _orderService = orderService;
            _mappingService = mappingService;
        }

        public List<OrderResponse> Orders { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = string.Empty;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public async Task OnGetAsync(string? search, string? status, int page = 1)
        {
            SearchTerm = search ?? string.Empty;
            StatusFilter = status ?? string.Empty;
            CurrentPage = page;

            try
            {
                // Get current dealer ID from session
                var dealerId = GetCurrentDealerId();
                if (!dealerId.HasValue)
                {
                    Orders = new List<OrderResponse>();
                    TempData["Error"] = "Không xác định được đại lý hiện tại";
                    return;
                }

                // Search orders for current dealer using service
                var result = await _orderService.SearchAsync(
                    dealerId.Value,
                    SearchTerm,
                    StatusFilter,
                    null, // Bỏ orderType filter
                    CurrentPage,
                    PageSize
                );

                if (result.Success && result.Item3.Data != null)
                {
                    // Map entities to DTOs using mapping service
                    Orders = _mappingService.MapToOrderCreateViewModels(result.Item3.Data);
                    TotalPages = result.Item3.TotalPages;
                }
                else
                {
                    Orders = new List<OrderResponse>();
                    TempData["Error"] = result.Error ?? "Không thể tải danh sách đơn hàng";
                }
            }
            catch (Exception ex)
            {
                Orders = new List<OrderResponse>();
                TempData["Error"] = $"Lỗi: {ex.Message}";
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(id);
                if (result.Success)
                {
                    TempData["Success"] = "Xóa đơn hàng thành công!";
                }
                else
                {
                    TempData["Error"] = result.Error ?? "Không thể xóa đơn hàng";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToPage();
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
