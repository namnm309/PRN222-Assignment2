using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.Enums;
using DataAccessLayer.Entities;

namespace PresentationLayer.Controllers
{
    public class PricingManagementController : BaseDashboardController
    {
        private readonly IPricingManagementService _pricingService;

        public PricingManagementController(IPricingManagementService pricingService)
        {
            _pricingService = pricingService;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Title = "Quản lý giá sỉ";
            
            // Get summary data
            var summary = await _pricingService.GetPricingSummaryAsync();
            ViewBag.Summary = summary;

            var policies = await _pricingService.GetAllPricingPoliciesAsync();
            return View(policies);
        }

        [HttpGet]
        public async Task<IActionResult> Policies(Guid? productId = null, Guid? dealerId = null, Guid? regionId = null, string status = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var policies = await _pricingService.GetPricingReportAsync(productId, dealerId, regionId, status);
            
            ViewBag.ProductId = productId;
            ViewBag.DealerId = dealerId;
            ViewBag.RegionId = regionId;
            ViewBag.Status = status;

            // Get dropdown data
            ViewBag.Products = await _pricingService.GetAllProductsAsync();
            ViewBag.Dealers = await _pricingService.GetAllDealersAsync();
            ViewBag.Regions = await _pricingService.GetAllRegionsAsync();

            return View(policies);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePolicy()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Products = await _pricingService.GetAllProductsAsync();
            ViewBag.Dealers = await _pricingService.GetAllDealersAsync();
            ViewBag.Regions = await _pricingService.GetAllRegionsAsync();

            var policy = new PricingPolicy
            {
                EffectiveDate = DateTime.Now,
                PolicyType = "Standard",
                Status = "Active",
                MinimumQuantity = 1,
                MaximumQuantity = 999
            };

            return View(policy);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePolicy(PricingPolicy policy)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _pricingService.GetAllProductsAsync();
                ViewBag.Dealers = await _pricingService.GetAllDealersAsync();
                ViewBag.Regions = await _pricingService.GetAllRegionsAsync();
                return View(policy);
            }

            var result = await _pricingService.CreatePricingPolicyAsync(policy);
            if (result)
            {
                TempData["Success"] = "Chính sách giá đã được tạo thành công.";
                return RedirectToAction("Policies");
            }

            TempData["Error"] = "Có lỗi xảy ra khi tạo chính sách giá.";
            ViewBag.Products = await _pricingService.GetAllProductsAsync();
            ViewBag.Dealers = await _pricingService.GetAllDealersAsync();
            ViewBag.Regions = await _pricingService.GetAllRegionsAsync();
            return View(policy);
        }

        [HttpGet]
        public async Task<IActionResult> EditPolicy(Guid id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var policy = await _pricingService.GetPricingPolicyAsync(id);
            if (policy == null)
            {
                TempData["Error"] = "Không tìm thấy chính sách giá.";
                return RedirectToAction("Policies");
            }

            ViewBag.Products = await _pricingService.GetAllProductsAsync();
            ViewBag.Dealers = await _pricingService.GetAllDealersAsync();
            ViewBag.Regions = await _pricingService.GetAllRegionsAsync();

            return View(policy);
        }

        [HttpPost]
        public async Task<IActionResult> EditPolicy(PricingPolicy policy)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _pricingService.GetAllProductsAsync();
                ViewBag.Dealers = await _pricingService.GetAllDealersAsync();
                ViewBag.Regions = await _pricingService.GetAllRegionsAsync();
                return View(policy);
            }

            var result = await _pricingService.UpdatePricingPolicyAsync(policy);
            if (result)
            {
                TempData["Success"] = "Chính sách giá đã được cập nhật thành công.";
                return RedirectToAction("Policies");
            }

            TempData["Error"] = "Có lỗi xảy ra khi cập nhật chính sách giá.";
            ViewBag.Products = await _pricingService.GetAllProductsAsync();
            ViewBag.Dealers = await _pricingService.GetAllDealersAsync();
            ViewBag.Regions = await _pricingService.GetAllRegionsAsync();
            return View(policy);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePolicy(Guid id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var result = await _pricingService.DeletePricingPolicyAsync(id);
            if (result)
            {
                TempData["Success"] = "Chính sách giá đã được xóa thành công.";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa chính sách giá.";
            }

            return RedirectToAction("Policies");
        }

        [HttpGet]
        public async Task<IActionResult> ActivePolicies(Guid? dealerId = null, Guid? regionId = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var policies = await _pricingService.GetActivePricingPoliciesAsync(dealerId, regionId);
            
            ViewBag.DealerId = dealerId;
            ViewBag.RegionId = regionId;

            // Get dropdown data
            ViewBag.Dealers = await _pricingService.GetAllDealersAsync();
            ViewBag.Regions = await _pricingService.GetAllRegionsAsync();

            return View(policies);
        }

        [HttpGet]
        public async Task<IActionResult> DiscountPolicies(Guid? dealerId = null, string policyType = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var policies = await _pricingService.GetDiscountPoliciesAsync(dealerId, policyType);
            
            ViewBag.DealerId = dealerId;
            ViewBag.PolicyType = policyType;

            // Get dropdown data
            ViewBag.Dealers = await _pricingService.GetAllDealersAsync();

            return View(policies);
        }

        [HttpGet]
        public async Task<IActionResult> ExpiringPolicies(int daysAhead = 30)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var expiringPolicies = await _pricingService.GetExpiringPoliciesAsync(daysAhead);
            var expiredPolicies = await _pricingService.GetExpiredPoliciesAsync();
            
            ViewBag.DaysAhead = daysAhead;
            ViewBag.ExpiringPolicies = expiringPolicies;
            ViewBag.ExpiredPolicies = expiredPolicies;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateExpiredPolicies()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var result = await _pricingService.DeactivateExpiredPoliciesAsync();
            if (result)
            {
                TempData["Success"] = "Đã vô hiệu hóa các chính sách giá hết hạn.";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi vô hiệu hóa chính sách giá.";
            }

            return RedirectToAction("ExpiringPolicies");
        }

        [HttpGet]
        public async Task<IActionResult> PriceCalculator()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Products = await _pricingService.GetAllProductsAsync();
            ViewBag.Dealers = await _pricingService.GetAllDealersAsync();
            ViewBag.Regions = await _pricingService.GetAllRegionsAsync();

            return View();
        }

        // API endpoints for AJAX calls
        [HttpGet]
        public async Task<JsonResult> GetPricingSummary()
        {
            if (!IsAdmin())
                return Json(new { error = "Unauthorized" });

            var summary = await _pricingService.GetPricingSummaryAsync();
            return Json(summary);
        }

        [HttpGet]
        public async Task<JsonResult> GetWholesalePrice(Guid productId, Guid? dealerId = null, Guid? regionId = null, int quantity = 1)
        {
            if (!IsAdmin())
                return Json(new { error = "Unauthorized" });

            var price = await _pricingService.GetWholesalePriceAsync(productId, dealerId, regionId, quantity);
            return Json(new { price = price });
        }

        [HttpGet]
        public async Task<JsonResult> GetRetailPrice(Guid productId, Guid? dealerId = null, Guid? regionId = null)
        {
            if (!IsAdmin())
                return Json(new { error = "Unauthorized" });

            var price = await _pricingService.GetRetailPriceAsync(productId, dealerId, regionId);
            return Json(new { price = price });
        }

        [HttpGet]
        public async Task<JsonResult> CalculateDiscount(Guid productId, Guid? dealerId = null, Guid? regionId = null, int quantity = 1)
        {
            if (!IsAdmin())
                return Json(new { error = "Unauthorized" });

            var discount = await _pricingService.CalculateDiscountAsync(productId, dealerId, regionId, quantity);
            return Json(new { discount = discount });
        }

        [HttpGet]
        public async Task<JsonResult> GetActivePricingPolicy(Guid productId, Guid? dealerId = null, Guid? regionId = null)
        {
            if (!IsAdmin())
                return Json(new { error = "Unauthorized" });

            var policy = await _pricingService.GetActivePricingPolicyAsync(productId, dealerId, regionId);
            return Json(policy);
        }
    }
}
