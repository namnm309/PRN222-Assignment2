using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Entities;

namespace PresentationLayer.Pages.DealerStaff.Orders
{
    public class PricingInfo
    {
        public decimal WholesalePrice { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal MinimumPrice { get; set; }
        public string PolicyType { get; set; } = string.Empty;
        public bool HasPolicy { get; set; }
    }

    public class CreateModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IMappingService _mappingService;
        private readonly IInventoryManagementService _inventoryService;
        private readonly IPricingManagementService _pricingService;
        private readonly IDealerService _dealerService;

        public CreateModel(
            IOrderService orderService,
            ICustomerService customerService,
            IProductService productService,
            IMappingService mappingService,
            IInventoryManagementService inventoryService,
            IPricingManagementService pricingService,
            IDealerService dealerService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
            _mappingService = mappingService;
            _inventoryService = inventoryService;
            _pricingService = pricingService;
            _dealerService = dealerService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<CustomerResponse> Customers { get; set; } = new();
        public List<ProductResponse> Products { get; set; } = new();
        public Dictionary<Guid, PricingInfo> ProductPricing { get; set; } = new();

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

            [Required]
            public string PriceType { get; set; } = "Wholesale"; // Wholesale or Retail

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

                // Load pricing information for each product
                var dealerId = GetCurrentDealerId();
                if (dealerId.HasValue)
                {
                    var (dealerOk, _, dealer) = await _dealerService.GetByIdAsync(dealerId.Value);
                    if (dealerOk && dealer != null)
                    {
                        foreach (var product in productsResult.Data)
                        {
                            var pricingPolicy = await _pricingService.GetActivePricingPolicyAsync(product.Id, dealerId.Value, dealer.RegionId);
                            
                            if (pricingPolicy != null)
                            {
                                ProductPricing[product.Id] = new PricingInfo
                                {
                                    WholesalePrice = pricingPolicy.WholesalePrice,
                                    RetailPrice = pricingPolicy.RetailPrice,
                                    DiscountRate = pricingPolicy.DiscountRate,
                                    MinimumPrice = pricingPolicy.MinimumPrice,
                                    PolicyType = pricingPolicy.PolicyType,
                                    HasPolicy = true
                                };
                            }
                            else
                            {
                                // Fallback to product base price
                                ProductPricing[product.Id] = new PricingInfo
                                {
                                    WholesalePrice = product.Price,
                                    RetailPrice = product.Price,
                                    DiscountRate = 0,
                                    MinimumPrice = product.Price,
                                    PolicyType = "Standard",
                                    HasPolicy = false
                                };
                            }
                        }
                    }
                }
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

            // Kiểm tra tồn kho trước khi tạo đơn hàng
            var inventoryResult = await _inventoryService.GetInventoryByDealerAndProductAsync(dealerId.Value, Input.ProductId);
            if (!inventoryResult.Success || inventoryResult.Data == null || inventoryResult.Data.AvailableQuantity <= 0)
            {
                ModelState.AddModelError("", "Không có xe trong kho. Vui lòng liên hệ EVM để đặt hàng hoặc chọn sản phẩm khác.");
                await LoadDataAsync();
                return Page();
            }

            try
            {
                // Tính giá theo PriceType
                decimal finalPrice = Input.Price;
                if (ProductPricing.ContainsKey(Input.ProductId))
                {
                    var pricing = ProductPricing[Input.ProductId];
                    finalPrice = Input.PriceType == "Retail" ? pricing.RetailPrice : pricing.WholesalePrice;
                    
                    // Apply discount nếu có
                    if (pricing.DiscountRate > 0)
                    {
                        finalPrice = finalPrice * (1 - pricing.DiscountRate / 100);
                        finalPrice = Math.Max(finalPrice, pricing.MinimumPrice);
                    }
                }

                var result = await _orderService.CreateQuotationAsync(
                    Input.ProductId,
                    Input.CustomerId,
                    dealerId.Value,
                    salesPersonId,
                    finalPrice,
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
                    // Không có allocation cho đại lý này
                    return new JsonResult(new { 
                        success = true,
                        hasStock = false,
                        availableQuantity = 0,
                        allocatedQuantity = 0,
                        reservedQuantity = 0,
                        minimumStock = 0,
                        message = "Chưa có xe được phân bổ cho đại lý này. Vui lòng liên hệ EVM để đặt hàng.",
                        isEVMStock = false
                    });
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
