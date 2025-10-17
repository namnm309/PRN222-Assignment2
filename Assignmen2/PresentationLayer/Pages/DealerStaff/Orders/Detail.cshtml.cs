using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.Orders
{
    public class DetailModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IMappingService _mappingService;

        public DetailModel(IOrderService orderService, IMappingService mappingService)
        {
            _orderService = orderService;
            _mappingService = mappingService;
        }

        public OrderResponse? Order { get; set; }

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

        public async Task<IActionResult> OnPostConfirmOrderAsync(Guid id)
        {
            try
            {
                Console.WriteLine($"[DEBUG] OnPostConfirmOrderAsync called with id: {id}");
                
                var result = await _orderService.ConfirmOrderAsync(id);
                Console.WriteLine($"[DEBUG] ConfirmOrderAsync result: Success={result.Success}, Error={result.Error}");
                
                if (result.Success)
                {
                    TempData["Success"] = "Xác nhận đơn hàng thành công!";
                    Console.WriteLine($"[DEBUG] Order confirmed successfully");
                }
                else
                {
                    TempData["Error"] = result.Error ?? "Không thể xác nhận đơn hàng";
                    Console.WriteLine($"[DEBUG] Order confirmation failed: {result.Error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Exception in OnPostConfirmOrderAsync: {ex.Message}");
                TempData["Error"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostCancelOrderAsync(Guid id)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(id);
                if (result.Success)
                {
                    TempData["Success"] = "Hủy đơn hàng thành công!";
                }
                else
                {
                    TempData["Error"] = result.Error ?? "Không thể hủy đơn hàng";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostUpdatePaymentAsync(Guid id, string paymentStatus, string? paymentMethod, DateTime? paymentDueDate)
        {
            try
            {
                var result = await _orderService.UpdatePaymentAsync(id, paymentStatus, paymentMethod, paymentDueDate);
                if (result.Success)
                {
                    TempData["Success"] = "Cập nhật thanh toán thành công!";
                }
                else
                {
                    TempData["Error"] = result.Error ?? "Không thể cập nhật thanh toán";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostDeliverAsync(Guid id, DateTime deliveryDate)
        {
            try
            {
                var result = await _orderService.DeliverOrderAsync(id, deliveryDate);
                if (result.Success)
                {
                    TempData["Success"] = "Cập nhật giao hàng thành công!";
                }
                else
                {
                    TempData["Error"] = result.Error ?? "Không thể cập nhật giao hàng";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToPage(new { id });
        }
    }
}
