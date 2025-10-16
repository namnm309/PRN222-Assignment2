using System;
using System.Linq;
using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.PricingManagement
{
    public class IndexModel : BaseEVMStaffPageModel
    {
        private readonly IPricingManagementService _pricingService;
        private readonly IMappingService _mappingService;

        public IndexModel(
            IPricingManagementService pricingService,
            IMappingService mappingService)
        {
            _pricingService = pricingService;
            _mappingService = mappingService;
        }

        public List<PricingPolicyResponse> PricingPolicies { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public Guid? ProductId { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? DealerId { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? RegionId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var policies = await _pricingService.GetAllPricingPoliciesAsync();
                
                if (policies != null)
                {
                    PricingPolicies = _mappingService.MapToPricingPolicyViewModels(policies);
                    
                    // Apply filters
                    if (ProductId.HasValue)
                    {
                        PricingPolicies = PricingPolicies.Where(p => p.ProductId == ProductId.Value).ToList();
                    }
                    
                    if (DealerId.HasValue)
                    {
                        PricingPolicies = PricingPolicies.Where(p => p.DealerId == DealerId.Value).ToList();
                    }
                    
                    if (RegionId.HasValue)
                    {
                        PricingPolicies = PricingPolicies.Where(p => p.RegionId == RegionId.Value).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Không thể tải danh sách chính sách giá: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            try
            {
                var success = await _pricingService.DeletePricingPolicyAsync(id);

                if (success)
                {
                    TempData["Success"] = "Xóa chính sách giá thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể xóa chính sách giá";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa chính sách giá: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}

