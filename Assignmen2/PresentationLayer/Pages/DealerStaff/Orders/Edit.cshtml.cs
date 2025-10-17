using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.Orders
{
    public class EditModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IMappingService _mappingService;

        public EditModel(IOrderService orderService, IMappingService mappingService)
        {
            _orderService = orderService;
            _mappingService = mappingService;
        }

        [BindProperty]
        public OrderResponse Order { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var result = await _orderService.GetByIdAsync(id);
                if (result.Success && result.Data != null)
                {
                    Order = _mappingService.MapToOrderCreateViewModel(result.Data);
                    return Page();
                }
                else
                {
                    TempData["Error"] = result.Error ?? "Không tìm thấy đơn hàng";
                    return RedirectToPage("/DealerStaff/Orders/Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToPage("/DealerStaff/Orders/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                // Tính toán lại thành tiền
                Order.FinalAmount = Order.Price - Order.Discount;
                
                // Đảm bảo thành tiền không âm
                if (Order.FinalAmount < 0)
                {
                    ModelState.AddModelError("Order.Discount", "Giảm giá không được lớn hơn giá bán");
                    return Page();
                }

                // Cập nhật thời gian
                Order.UpdatedAt = DateTime.UtcNow;

                // Kiểm tra quyền chỉnh sửa
                if (!CanEditOrder(Order))
                {
                    TempData["Error"] = "Không thể chỉnh sửa đơn hàng ở trạng thái này";
                    return Page();
                }

                // Cập nhật đơn hàng
                var result = await _orderService.UpdateOrderAsync(_mappingService.MapToOrderEntity(Order));
                if (result.Success)
                {
                    TempData["Success"] = "Cập nhật đơn hàng thành công!";
                    return RedirectToPage("/DealerStaff/Orders/Detail", new { id = Order.Id });
                }
                else
                {
                    TempData["Error"] = result.Error ?? "Không thể cập nhật đơn hàng";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return Page();
            }
        }

        private bool CanEditOrder(OrderResponse order)
        {
            // Chỉ cho phép chỉnh sửa các đơn hàng chưa giao và chưa có hợp đồng
            return order.Status != "Delivered" && order.Status != "Contract";
        }
    }
}
