using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Pages.DealerStaff.PurchaseOrders
{
    public class CreateModel : PageModel
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IProductService _productService;

        public CreateModel(IPurchaseOrderService purchaseOrderService, IProductService productService)
        {
            _purchaseOrderService = purchaseOrderService;
            _productService = productService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<Product> Products { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
            [Display(Name = "Sản phẩm")]
            public Guid ProductId { get; set; }

            [Required(ErrorMessage = "Số lượng là bắt buộc")]
            [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
            [Display(Name = "Số lượng")]
            public int RequestedQuantity { get; set; }

            [Required(ErrorMessage = "Giá nhập là bắt buộc")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Giá nhập phải lớn hơn 0")]
            [Display(Name = "Giá nhập")]
            public decimal UnitPrice { get; set; }

            [Display(Name = "Ngày giao hàng dự kiến")]
            public DateTime? ExpectedDeliveryDate { get; set; }

            [Required(ErrorMessage = "Lý do đặt hàng là bắt buộc")]
            [Display(Name = "Lý do đặt hàng")]
            public string Reason { get; set; } = string.Empty;

            [Display(Name = "Ghi chú")]
            public string? Notes { get; set; }
        }

        public async Task OnGetAsync(Guid? productId = null)
        {
            // Load available products
            var productsResult = await _productService.SearchAsync(null, null, null, null, true, true);
            if (productsResult.Success)
            {
                Products = productsResult.Data;
            }

            // Pre-select product if provided
            if (productId.HasValue)
            {
                Input.ProductId = productId.Value;
                // Load product price
                var productResult = await _productService.GetAsync(productId.Value);
                if (productResult.Success && productResult.Data != null)
                {
                    Input.UnitPrice = productResult.Data.Price;
                }
            }
            else
            {
                // Reset unit price if no product selected
                Input.UnitPrice = 0;
            }

            // Set default values
            Input.RequestedQuantity = 1;
            Input.ExpectedDeliveryDate = DateTime.Today.AddDays(7);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload products if validation fails
                var productsResult = await _productService.SearchAsync(null, null, null, null, true, true);
                if (productsResult.Success)
                {
                    Products = productsResult.Data;
                }
                return Page();
            }

            // Get current dealer ID (you'll need to implement this)
            var dealerId = GetCurrentDealerId();
            if (!dealerId.HasValue)
            {
                ModelState.AddModelError("", "Không xác định được đại lý hiện tại");
                return Page();
            }

            // Get current user ID (you'll need to implement this)
            var requestedById = GetCurrentUserId();
            if (!requestedById.HasValue)
            {
                ModelState.AddModelError("", "Không xác định được người tạo đơn hàng");
                return Page();
            }

            try
            {
                var result = await _purchaseOrderService.CreateAsync(
                    dealerId.Value,
                    Input.ProductId,
                    requestedById.Value,
                    Input.RequestedQuantity,
                    Input.UnitPrice,
                    Input.Reason,
                    Input.Notes ?? "",
                    Input.ExpectedDeliveryDate
                );

                if (result.Success)
                {
                    TempData["Success"] = "Tạo đơn đặt hàng thành công! Đơn hàng đang chờ duyệt từ EVM.";
                    return RedirectToPage("/DealerStaff/PurchaseOrders/Index");
                }
                else
                {
                    ModelState.AddModelError("", result.Error ?? "Không thể tạo đơn đặt hàng");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                return Page();
            }
        }

        private Guid? GetCurrentDealerId()
        {
            // Get dealer ID from session or authentication context
            if (HttpContext.Session.GetString("DealerId") != null)
            {
                return Guid.Parse(HttpContext.Session.GetString("DealerId")!);
            }
            
            // Fallback: try to get from user claims
            var dealerIdClaim = User.FindFirst("DealerId");
            if (dealerIdClaim != null && Guid.TryParse(dealerIdClaim.Value, out var dealerId))
            {
                return dealerId;
            }
            
            return null;
        }

        private Guid? GetCurrentUserId()
        {
            // Get user ID from authentication context
            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst("sub");
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            
            return null;
        }

        public async Task<IActionResult> OnGetGetProductStockAsync(Guid productId)
        {
            try
            {
                var productResult = await _productService.GetAsync(productId);
                if (!productResult.Success || productResult.Data == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy sản phẩm" });
                }

                var product = productResult.Data;
                var hasStock = product.StockQuantity > 0;
                var minimumStock = 5; // You can make this configurable
                var availableQuantity = product.StockQuantity;
                var allocatedQuantity = 0; // TODO: Calculate from orders
                var reservedQuantity = 0; // TODO: Calculate from reservations

                var message = hasStock 
                    ? $"Có sẵn {availableQuantity} xe trong kho"
                    : "Không còn xe trong kho";

                return new JsonResult(new
                {
                    success = true,
                    hasStock = hasStock,
                    availableQuantity = availableQuantity,
                    minimumStock = minimumStock,
                    allocatedQuantity = allocatedQuantity,
                    reservedQuantity = reservedQuantity,
                    message = message
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}
