using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Pages.DealerStaff.Orders
{
    public class CreateContractModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IMappingService _mappingService;

        public CreateContractModel(IOrderService orderService, IMappingService mappingService)
        {
            _orderService = orderService;
            _mappingService = mappingService;
        }

        public OrderResponse? Order { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Số hợp đồng là bắt buộc")]
            [Display(Name = "Số hợp đồng")]
            public string ContractNumber { get; set; } = string.Empty;

            [Required(ErrorMessage = "Ngày ký hợp đồng là bắt buộc")]
            [Display(Name = "Ngày ký hợp đồng")]
            public DateTime ContractDate { get; set; }

            [Required(ErrorMessage = "Địa chỉ giao xe là bắt buộc")]
            [Display(Name = "Địa chỉ giao xe")]
            public string DeliveryAddress { get; set; } = string.Empty;

            [Required(ErrorMessage = "Điều khoản thanh toán là bắt buộc")]
            [Display(Name = "Điều khoản thanh toán")]
            public string PaymentTerms { get; set; } = string.Empty;

            [Display(Name = "Thời gian bảo hành")]
            public int WarrantyPeriod { get; set; } = 24;

            [Display(Name = "Kỳ hạn trả góp")]
            public int? InstallmentPeriod { get; set; }

            [Display(Name = "Điều khoản hợp đồng")]
            public string? ContractTerms { get; set; }

            [Display(Name = "Ghi chú")]
            public string? Notes { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid orderId)
        {
            try
            {
                var result = await _orderService.GetByIdAsync(orderId);
                if (result.Success && result.Data != null)
                {
                    Order = _mappingService.MapToOrderCreateViewModel(result.Data);
                    
                    // Check if order is delivered
                    if (Order.Status != "Delivered")
                    {
                        TempData["Error"] = "Chỉ có thể tạo hợp đồng cho đơn hàng đã giao xe";
                        return RedirectToPage("/DealerStaff/Orders/Index");
                    }

                    // Pre-fill some fields
                    Input.ContractDate = DateTime.Today;
                    Input.DeliveryAddress = Order.CustomerName; // Default to customer name, user can edit
                    
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

        public async Task<IActionResult> OnPostAsync(Guid orderId)
        {
            if (!ModelState.IsValid)
            {
                // Reload order data
                var result = await _orderService.GetByIdAsync(orderId);
                if (result.Success && result.Data != null)
                {
                    Order = _mappingService.MapToOrderCreateViewModel(result.Data);
                }
                return Page();
            }

            try
            {
                // For now, we'll just update the order status to "Contract"
                // In a real application, you would create a separate Contract entity
                var orderResult = await _orderService.GetByIdAsync(orderId);
                if (!orderResult.Success || orderResult.Data == null)
                {
                    TempData["Error"] = "Không tìm thấy đơn hàng";
                    return RedirectToPage("/DealerStaff/Orders/Index");
                }

                // Update order with contract information
                var order = orderResult.Data;
                order.Status = "Contract";
                order.Notes = $"{order.Notes}\n\n--- HỢP ĐỒNG BÁN HÀNG ---\n" +
                             $"Số hợp đồng: {Input.ContractNumber}\n" +
                             $"Ngày ký: {Input.ContractDate:dd/MM/yyyy}\n" +
                             $"Địa chỉ giao xe: {Input.DeliveryAddress}\n" +
                             $"Điều khoản thanh toán: {Input.PaymentTerms}\n" +
                             $"Bảo hành: {Input.WarrantyPeriod} tháng\n" +
                             $"Trả góp: {(Input.InstallmentPeriod.HasValue ? $"{Input.InstallmentPeriod} tháng" : "Không")}\n" +
                             $"Điều khoản: {Input.ContractTerms ?? "Theo quy định của công ty"}\n" +
                             $"Ghi chú: {Input.Notes ?? ""}";
                order.UpdatedAt = DateTime.UtcNow;

                // Update the order
                var updateResult = await _orderService.UpdateOrderAsync(order);
                if (updateResult.Success)
                {
                    TempData["Success"] = "Tạo hợp đồng bán hàng thành công!";
                    return RedirectToPage("/DealerStaff/Orders/Index");
                }
                else
                {
                    TempData["Error"] = updateResult.Error ?? "Không thể tạo hợp đồng";
                    return RedirectToPage("/DealerStaff/Orders/Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToPage("/DealerStaff/Orders/Index");
            }
        }
    }
}
