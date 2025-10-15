using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IOrderService _orderService;

        public IndexModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public List<Order> Orders { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = string.Empty;
        public string OrderTypeFilter { get; set; } = string.Empty;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public async Task OnGetAsync(string? search, string? status, string? orderType, int page = 1)
        {
            SearchTerm = search ?? string.Empty;
            StatusFilter = status ?? string.Empty;
            OrderTypeFilter = orderType ?? string.Empty;
            CurrentPage = page;

            try
            {
                // Get current dealer ID (you'll need to implement this based on your authentication)
                var dealerId = GetCurrentDealerId();
                if (!dealerId.HasValue)
                {
                    // Handle case where dealer ID is not available
                    Orders = new List<Order>();
                    return;
                }

                // Search orders for current dealer
                var result = await _orderService.SearchAsync(
                    dealerId.Value,
                    SearchTerm,
                    StatusFilter,
                    OrderTypeFilter,
                    CurrentPage,
                    PageSize
                );

                if (result.Success)
                {
                    Orders = result.Item3.Data;
                    TotalPages = result.Item3.TotalPages;
                }
                else
                {
                    Orders = new List<Order>();
                    TempData["Error"] = result.Error ?? "Không thể tải danh sách đơn hàng";
                }
            }
            catch (Exception ex)
            {
                Orders = new List<Order>();
                TempData["Error"] = $"Lỗi: {ex.Message}";
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
