using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.DealerStaff.Promotions
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService; // Assuming we'll use ProductService for now

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Type { get; set; }

        public class PromotionVm
        {
            public Guid Id { get; set; }
            public string title { get; set; } = string.Empty;
            public string? description { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public List<PromotionVm> Promotions { get; set; } = new();

        public async Task OnGetAsync()
        {
            // TODO: Implement promotion service
            // For now, we'll create some sample data
            Promotions = GetSamplePromotions();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(Search))
            {
                var searchLower = Search.ToLower();
                Promotions = Promotions.Where(p => 
                    p.title.ToLower().Contains(searchLower) ||
                    (p.description != null && p.description.ToLower().Contains(searchLower))
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                Promotions = Promotions.Where(p => 
                    (Status == "Active" && p.IsActive) ||
                    (Status == "Inactive" && !p.IsActive)
                ).ToList();
            }

            // Note: Type filter removed since Promotion entity doesn't have Type property
        }

        private List<PromotionVm> GetSamplePromotions()
        {
            return new List<PromotionVm>
            {
                new PromotionVm
                {
                    Id = Guid.NewGuid(),
                    title = "Giảm giá 10% cho khách hàng mới",
                    description = "Áp dụng cho tất cả sản phẩm xe điện, giảm 10% cho khách hàng lần đầu mua hàng",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-30),
                    UpdatedAt = DateTime.Now.AddDays(-30)
                },
                new PromotionVm
                {
                    Id = Guid.NewGuid(),
                    title = "Tặng phụ kiện cao cấp",
                    description = "Mua xe điện được tặng bộ phụ kiện cao cấp trị giá 5 triệu đồng",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-15),
                    UpdatedAt = DateTime.Now.AddDays(-15)
                },
                new PromotionVm
                {
                    Id = Guid.NewGuid(),
                    title = "Trả góp 0% lãi suất",
                    description = "Trả góp trong 24 tháng với lãi suất 0% cho khách hàng có tín dụng tốt",
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-7),
                    UpdatedAt = DateTime.Now.AddDays(-7)
                },
                new PromotionVm
                {
                    Id = Guid.NewGuid(),
                    title = "Khuyến mãi cuối năm",
                    description = "Giảm giá đặc biệt cho dịp cuối năm, áp dụng cho tất cả sản phẩm",
                    IsActive = false,
                    CreatedAt = DateTime.Now.AddDays(-60),
                    UpdatedAt = DateTime.Now.AddDays(-1)
                }
            };
        }
    }
}
