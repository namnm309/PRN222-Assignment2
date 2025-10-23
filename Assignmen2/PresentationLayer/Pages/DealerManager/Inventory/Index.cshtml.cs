using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.DealerManager.Inventory
{
    public class IndexModel : BaseDealerManagerPageModel
    {
        private readonly IInventoryManagementService _inventoryService;

        public IndexModel(
            IDealerService dealerService,
            IOrderService orderService,
            ITestDriveService testDriveService,
            ICustomerService customerService,
            IEVMReportService reportService,
            IDealerDebtService dealerDebtService,
            IAuthenService authenService,
            IPurchaseOrderService purchaseOrderService,
            IProductService productService,
            IBrandService brandService,
            IMappingService mappingService,
            IInventoryManagementService inventoryService)
            : base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService, mappingService)
        {
            _inventoryService = inventoryService;
        }

        public List<InventoryAllocationResponse> InventoryAllocations { get; set; } = new();
        public string? DealerName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var dealerId = GetCurrentDealerId();
            if (dealerId == null)
            {
                return RedirectToPage("/Dashboard/Index");
            }

            // Lấy tên đại lý
            var (ok, _, dealer) = await DealerService.GetByIdAsync(dealerId.Value);
            DealerName = dealer?.Name;

            // Lấy danh sách tồn kho đã phân bổ cho đại lý
            try
            {
                var allocations = await _inventoryService.GetInventoryAllocationsByDealerAsync(dealerId.Value);
                InventoryAllocations = allocations ?? new List<InventoryAllocationResponse>();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Không thể tải thông tin tồn kho: {ex.Message}";
                InventoryAllocations = new List<InventoryAllocationResponse>();
            }

            return Page();
        }
    }
}
