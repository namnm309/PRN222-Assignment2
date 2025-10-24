using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.DTOs.Responses;
using DataAccessLayer.Entities;

namespace PresentationLayer.Pages.DealerStaff.PurchaseOrders
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
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IProductService _productService;
        private readonly IPricingManagementService _pricingService;
        private readonly IDealerService _dealerService;

        public CreateModel(
            IPurchaseOrderService purchaseOrderService, 
            IProductService productService,
            IPricingManagementService pricingService,
            IDealerService dealerService)
        {
            _purchaseOrderService = purchaseOrderService;
            _productService = productService;
            _pricingService = pricingService;
            _dealerService = dealerService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<ProductResponse> Products { get; set; } = new();
        public Dictionary<Guid, PricingInfo> ProductPricing { get; set; } = new();

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

            [Required]
            public string PriceType { get; set; } = "Wholesale"; // Wholesale or Retail
        }

        public async Task OnGetAsync(Guid? productId = null)
        {
            // Load available products
            var productsResult = await _productService.SearchAsync(null, null, null, null, true, true);
            if (productsResult.Success)
            {
                Products = productsResult.Data.Select(p => new ProductResponse { Id = p.Id, Name = p.Name, Price = p.Price, StockQuantity = p.StockQuantity, BrandId = p.BrandId, BrandName = p.Brand?.Name ?? string.Empty }).ToList();

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

            // Pre-select product if provided
            if (productId.HasValue)
            {
                Input.ProductId = productId.Value;
                // Load product price from pricing policy
                if (ProductPricing.ContainsKey(productId.Value))
                {
                    var pricing = ProductPricing[productId.Value];
                    Input.UnitPrice = pricing.WholesalePrice; // Default to wholesale
                }
                else
                {
                    var productResult = await _productService.GetAsync(productId.Value);
                    if (productResult.Success && productResult.Data != null)
                    {
                        Input.UnitPrice = productResult.Data.Price;
                    }
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

        private Guid? GetCurrentDealerId()
        {
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            if (Guid.TryParse(dealerIdString, out var dealerId))
            {
                return dealerId;
            }
            return null;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload products if validation fails
                var productsResult = await _productService.SearchAsync(null, null, null, null, true, true);
                if (productsResult.Success)
                {
                    Products = productsResult.Data
                        .Select(p => new ProductResponse
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            StockQuantity = p.StockQuantity,
                            BrandId = p.BrandId,
                            BrandName = p.Brand?.Name ?? string.Empty
                        }).ToList();
                }
                return Page();
            }

            // Get current dealer ID
            var dealerId = GetCurrentDealerId();
            if (!dealerId.HasValue)
            {
                ModelState.AddModelError("", "Không xác định được đại lý hiện tại");
                return Page();
            }

            // Calculate final price based on PriceType
            decimal finalUnitPrice = Input.UnitPrice;
            if (ProductPricing.ContainsKey(Input.ProductId))
            {
                var pricing = ProductPricing[Input.ProductId];
                finalUnitPrice = Input.PriceType == "Retail" ? pricing.RetailPrice : pricing.WholesalePrice;
                
                // Apply discount if any
                if (pricing.DiscountRate > 0)
                {
                    finalUnitPrice = finalUnitPrice * (1 - pricing.DiscountRate / 100);
                    finalUnitPrice = Math.Max(finalUnitPrice, pricing.MinimumPrice);
                }
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
                    finalUnitPrice, // Use finalUnitPrice instead of Input.UnitPrice
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
                var dealerId = GetCurrentDealerId();
                if (!dealerId.HasValue)
                {
                    return new JsonResult(new { success = false, message = "Không xác định được đại lý hiện tại" });
                }

                // Kiểm tra tồn kho đã phân bổ cho đại lý
                var inventoryService = HttpContext.RequestServices.GetRequiredService<IInventoryManagementService>();
                var inventoryResult = await inventoryService.GetInventoryByDealerAndProductAsync(dealerId.Value, productId);
                
                if (inventoryResult.Success && inventoryResult.Data != null)
                {
                    // Có allocation cho đại lý
                    var inventory = inventoryResult.Data;
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
                        message = "Không có xe trong kho. Vui lòng đặt hàng từ EVM.";
                    }

                    return new JsonResult(new
                    {
                        success = true,
                        hasStock = hasStock,
                        availableQuantity = inventory.AvailableQuantity,
                        allocatedQuantity = inventory.AllocatedQuantity,
                        reservedQuantity = inventory.ReservedQuantity,
                        minimumStock = inventory.MinimumStock,
                        message = message,
                        isEVMStock = false
                    });
                }
                else
                {
                    // Không có allocation cho đại lý này
                    return new JsonResult(new { 
                        success = true,
                        hasStock = false,
                        availableQuantity = 0,
                        allocatedQuantity = 0,
                        reservedQuantity = 0,
                        minimumStock = 0,
                        message = "Chưa có xe được phân bổ cho đại lý này. Vui lòng đặt hàng từ EVM.",
                        isEVMStock = false
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}
