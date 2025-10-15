using BusinessLayer.Services;
using DataAccessLayer.Entities;
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

        public CreateModel(
            IOrderService orderService,
            ICustomerService customerService,
            IProductService productService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<Customer> Customers { get; set; } = new();
        public List<Product> Products { get; set; } = new();

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
            // Load customers
            var customersResult = await _customerService.GetAllAsync();
            if (customersResult.Success)
            {
                Customers = customersResult.Data;
            }

            // Load products
            var productsResult = await _productService.SearchAsync(null, null, null, null, true, true);
            if (productsResult.Success)
            {
                Products = productsResult.Data;
            }

            // Pre-select if parameters provided
            if (customerId.HasValue)
            {
                Input.CustomerId = customerId.Value;
            }

            if (productId.HasValue)
            {
                Input.ProductId = productId.Value;
                // Load product price
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

            // Get current dealer ID (you'll need to implement this)
            var dealerId = GetCurrentDealerId();
            if (!dealerId.HasValue)
            {
                ModelState.AddModelError("", "Không xác định được đại lý hiện tại");
                await LoadDataAsync();
                return Page();
            }

            // Get current sales person ID (you'll need to implement this)
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
                    Input.Description ?? "",
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
            // Load customers
            var customersResult = await _customerService.GetAllAsync();
            if (customersResult.Success)
            {
                Customers = customersResult.Data;
            }

            // Load products
            var productsResult = await _productService.SearchAsync(null, null, null, null, true, true);
            if (productsResult.Success)
            {
                Products = productsResult.Data;
            }
        }

        private Guid? GetCurrentDealerId()
        {
            // TODO: Implement getting current dealer ID from session/authentication
            // For now, return a dummy ID - you'll need to implement this based on your authentication system
            return Guid.NewGuid();
        }

        private Guid? GetCurrentSalesPersonId()
        {
            // TODO: Implement getting current sales person ID from session/authentication
            // For now, return a dummy ID - you'll need to implement this based on your authentication system
            return Guid.NewGuid();
        }
    }
}
