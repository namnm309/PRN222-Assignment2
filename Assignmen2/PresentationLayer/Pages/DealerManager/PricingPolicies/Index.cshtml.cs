using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace PresentationLayer.Pages.DealerManager.PricingPolicies
{
    public class IndexModel : BaseDealerManagerPageModel
    {
        private readonly IPricingManagementService _pricingService;
        private readonly IMappingService _mappingService;

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
            IPricingManagementService pricingService)
            : base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService, mappingService)
        {
            _pricingService = pricingService;
            _mappingService = mappingService;
        }

        public List<PricingPolicyResponse> PricingPolicies { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var dealerId = GetCurrentDealerId();
            if (dealerId == null) return RedirectToPage("/Dashboard/Index");

            try
            {
                var (dealerOk, _, dealer) = await DealerService.GetByIdAsync(dealerId.Value);
                if (!dealerOk || dealer == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin đại lý.";
                    return Page();
                }

                // Lấy chính sách giá cho dealer này
                var policies = await _pricingService.GetActivePricingPoliciesAsync(dealerId.Value, dealer.RegionId);
                if (policies != null)
                {
                    PricingPolicies = _mappingService.MapToPricingPolicyViewModels(policies);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Không thể tải danh sách chính sách giá: {ex.Message}";
            }

            return Page();
        }
    }
}