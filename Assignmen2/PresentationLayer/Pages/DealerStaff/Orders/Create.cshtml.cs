using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Pages.DealerStaff.Orders
{
    public class CreateModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IMappingService _mappingService;

        public CreateModel(
            IOrderService orderService,
            ICustomerService customerService,
            IProductService productService,
            IMappingService mappingService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
            _mappingService = mappingService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<CustomerResponse> Customers { get; set; } = new();
        public List<ProductResponse> Products { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Loại đơn hàng là bắt buộc")]
            [Display(Name = "Loại đơn hàng")]
            public string OrderType { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
            [Display(Name = "Khách hàng")]
            public Guid CustomerId { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
            [Display(Name = "Sản phẩm")]
            public Guid ProductId { get; set; }

            [Required(ErrorMessage = "Giá bán là bắt buộc")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0")]
            [Display(Name = "Giá bán")]
            public decimal Price { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Giảm giá không được âm")]
            [Display(Name = "Giảm giá")]
            public decimal Discount { get; set; }

            [Display(Name = "Phương thức thanh toán")]
            public string? PaymentMethod { get; set; }

            [Display(Name = "Hạn thanh toán")]
            public DateTime? PaymentDueDate { get; set; }

            [Display(Name = "Mô tả")]
            public string? Description { get; set; }

            [Display(Name = "Ghi chú")]
            public string? Notes { get; set; }
        }

        public async Task OnGetAsync(Guid? customerId = null, Guid? productId = null)
        {
            // Load customers using service
            var customersResult = await _customerService.GetAllAsync();
            if (customersResult.Success && customersResult.Data != null)
            {
                Customers = _mappingService.MapToCustomerViewModels(customersResult.Data);
            }

            // Load products using service
            var productsResult = await _productService.SearchAsync(null, null, null, null, true, true);
            if (productsResult.Success && productsResult.Data != null)
            {
                Products = _mappingService.MapToProductViewModels(productsResult.Data);
            }

            // Pre-select if parameters provided
            if (customerId.HasValue)
            {
                Input.CustomerId = customerId.Value;
            }

            if (productId.HasValue)
            {
                Input.ProductId = productId.Value;
                // Load product price using service
                var productResult = await _productService.GetAsync(productId.Value);
                if (productResult.Success && productResult.Data != null)
                {
                    Input.Price = productResult.Data.Price;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload data if validation fails
                await LoadDataAsync();
                return Page();
            }

            // Get current dealer ID from session
            var dealerId = GetCurrentDealerId();
            if (!dealerId.HasValue)
            {
                ModelState.AddModelError("", "Không xác định được đại lý hiện tại");
                await LoadDataAsync();
                return Page();
            }

            // Get current sales person ID from session
            var salesPersonId = GetCurrentSalesPersonId();

            try
            {
                var result = await _orderService.CreateQuotationAsync(
                    Input.ProductId,
                    Input.CustomerId,
                    dealerId.Value,
                    salesPersonId,
                    Input.Price,
                    Input.Discount,
                    Input.OrderType,
                    Input.Notes ?? ""
                );

                if (result.Success)
                {
                    TempData["Success"] = $"Tạo {Input.OrderType.ToLower()} thành công!";
                    return RedirectToPage("/DealerStaff/Orders/Index");
                }
                else
                {
                    ModelState.AddModelError("", result.Error ?? "Không thể tạo đơn hàng");
                    await LoadDataAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                await LoadDataAsync();
                return Page();
            }
        }

        private async Task LoadDataAsync()
        {
            // Load customers using service
            var customersResult = await _customerService.GetAllAsync();
            if (customersResult.Success && customersResult.Data != null)
            {
                Customers = _mappingService.MapToCustomerViewModels(customersResult.Data);
            }

            // Load products using service
            var productsResult = await _productService.SearchAsync(null, null, null, null, true, true);
            if (productsResult.Success && productsResult.Data != null)
            {
                Products = _mappingService.MapToProductViewModels(productsResult.Data);
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

        private Guid? GetCurrentSalesPersonId()
        {
            // Get user ID from session
            var userIdString = HttpContext.Session.GetString("UserId");
            if (Guid.TryParse(userIdString, out var userId))
            {
                return userId;
            }
            return null;
        }

        // API endpoint để kiểm tra tồn kho sản phẩm
        public async Task<IActionResult> OnGetGetProductStockAsync(Guid productId)
        {
            try
            {
                var dealerId = GetCurrentDealerId();
                if (!dealerId.HasValue)
                {
                    return new JsonResult(new { success = false, message = "Không xác định được đại lý hiện tại" });
                }

                // Kiểm tra tồn kho từ InventoryAllocation
                var inventoryService = HttpContext.RequestServices.GetRequiredService<IInventoryManagementService>();
                var result = await inventoryService.GetInventoryByDealerAndProductAsync(dealerId.Value, productId);

                if (!result.Success || result.Data == null)
                {
                    // Fallback: Kiểm tra tồn kho tổng từ Product (như EVMStaff)
                    var productService = HttpContext.RequestServices.GetRequiredService<IProductService>();
                    var productResult = await productService.GetByIdAsync(productId);
                    
                    if (productResult.Success && productResult.Data != null)
                    {
                        var product = productResult.Data;
                        var evmHasStock = product.StockQuantity > 0;
                        
                        string evmMessage;
                        if (evmHasStock)
                        {
                            evmMessage = $"Có sẵn {product.StockQuantity} xe trong kho";
                        }
                        else
                        {
                            evmMessage = "Không có xe trong kho. Vui lòng liên hệ EVM để đặt hàng.";
                        }

                        return new JsonResult(new { 
                            success = true,
                            hasStock = evmHasStock,
                            availableQuantity = product.StockQuantity,
                            allocatedQuantity = 0,
                            reservedQuantity = 0,
                            minimumStock = 0,
                            message = evmMessage,
                            isEVMStock = true // Đánh dấu đây là tồn kho EVM, chưa phân bổ
                        });
                    }
                    else
                    {
                        // Không tìm thấy sản phẩm
                        return new JsonResult(new { 
                            success = true,
                            hasStock = false,
                            availableQuantity = 0,
                            allocatedQuantity = 0,
                            reservedQuantity = 0,
                            minimumStock = 0,
                            message = "Không tìm thấy thông tin sản phẩm."
                        });
                    }
                }

                var inventory = result.Data;
                var hasStock = inventory.AvailableQuantity > 0;
                
                string message;
                if (hasStock)
                {
                    if (inventory.AvailableQuantity <= inventory.MinimumStock)
                    {
                        message = $"Tồn kho thấp! Chỉ còn {inventory.AvailableQuantity} xe (tối thiểu: {inventory.MinimumStock} xe)";
                    }
                    else
                    {
                        message = $"Có sẵn {inventory.AvailableQuantity} xe trong kho";
                    }
                }
                else
                {
                    message = "Không có xe trong kho. Vui lòng liên hệ EVM để đặt hàng.";
                }

                return new JsonResult(new
                {
                    success = true,
                    hasStock = hasStock,
                    availableQuantity = inventory.AvailableQuantity,
                    allocatedQuantity = inventory.AllocatedQuantity,
                    reservedQuantity = inventory.ReservedQuantity,
                    minimumStock = inventory.MinimumStock,
                    message = message
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { 
                    success = false, 
                    hasStock = false, 
                    message = $"Lỗi kiểm tra tồn kho: {ex.Message}" 
                });
            }
        }
    }
}
