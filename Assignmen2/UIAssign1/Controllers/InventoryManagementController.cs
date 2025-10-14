using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.Enums;
using BusinessLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    public class InventoryManagementController : BaseDashboardController
    {
        private readonly IInventoryManagementService _inventoryService;

        public InventoryManagementController(IInventoryManagementService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Title = "Quản lý tồn kho";
            
            // Get summary data
            var summary = await _inventoryService.GetStockSummaryAsync();
            ViewBag.Summary = summary;

            var allocations = await _inventoryService.GetAllInventoryAllocationsAsync();
            return View(allocations);
        }

        [HttpGet]
        public async Task<IActionResult> Allocations(Guid? dealerId = null, Guid? productId = null, string status = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var allocations = await _inventoryService.GetInventoryReportAsync(dealerId, productId, status);
            
            ViewBag.DealerId = dealerId;
            ViewBag.ProductId = productId;
            ViewBag.Status = status;

            // Get dropdown data
            ViewBag.Dealers = await _inventoryService.GetAllDealersAsync();
            ViewBag.Products = await _inventoryService.GetAllProductsAsync();

            return View(allocations);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAllocation()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Dealers = await _inventoryService.GetAllDealersAsync();
            ViewBag.Products = await _inventoryService.GetAllProductsAsync();

            return View(new InventoryAllocationViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateAllocation(InventoryAllocationViewModel allocation)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Dealers = await _inventoryService.GetAllDealersAsync();
                ViewBag.Products = await _inventoryService.GetAllProductsAsync();
                return View(allocation);
            }

            allocation.Id = Guid.NewGuid();
            allocation.CreatedAt = DateTime.UtcNow;
            allocation.UpdatedAt = DateTime.UtcNow;

            var result = await _inventoryService.CreateInventoryAllocationAsync(allocation);
            if (result)
            {
                TempData["Success"] = "Phân bổ tồn kho đã được tạo thành công.";
                return RedirectToAction("Allocations");
            }

            TempData["Error"] = "Có lỗi xảy ra khi tạo phân bổ tồn kho.";
            ViewBag.Dealers = await _inventoryService.GetAllDealersAsync();
            ViewBag.Products = await _inventoryService.GetAllProductsAsync();
            return View(allocation);
        }

        [HttpGet]
        public async Task<IActionResult> EditAllocation(Guid productId, Guid dealerId)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var allocation = await _inventoryService.GetInventoryAllocationAsync(productId, dealerId);
            if (allocation == null)
            {
                TempData["Error"] = "Không tìm thấy phân bổ tồn kho.";
                return RedirectToAction("Allocations");
            }

            ViewBag.Dealers = await _inventoryService.GetAllDealersAsync();
            ViewBag.Products = await _inventoryService.GetAllProductsAsync();

            return View(allocation);
        }

        [HttpPost]
        public async Task<IActionResult> EditAllocation(InventoryAllocationViewModel allocation)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Dealers = await _inventoryService.GetAllDealersAsync();
                ViewBag.Products = await _inventoryService.GetAllProductsAsync();
                return View(allocation);
            }

            var result = await _inventoryService.UpdateInventoryAllocationAsync(allocation);
            if (result)
            {
                TempData["Success"] = "Phân bổ tồn kho đã được cập nhật thành công.";
                return RedirectToAction("Allocations");
            }

            TempData["Error"] = "Có lỗi xảy ra khi cập nhật phân bổ tồn kho.";
            ViewBag.Dealers = await _inventoryService.GetAllDealersAsync();
            ViewBag.Products = await _inventoryService.GetAllProductsAsync();
            return View(allocation);
        }

        [HttpGet]
        public async Task<IActionResult> StockAlerts()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var lowStock = await _inventoryService.GetLowStockAllocationsAsync();
            var criticalStock = await _inventoryService.GetCriticalStockAllocationsAsync();
            var outOfStock = await _inventoryService.GetOutOfStockAllocationsAsync();

            ViewBag.LowStock = lowStock;
            ViewBag.CriticalStock = criticalStock;
            ViewBag.OutOfStock = outOfStock;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StockTransfer()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Dealers = await _inventoryService.GetAllDealersAsync();
            ViewBag.Products = await _inventoryService.GetAllProductsAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StockTransfer(Guid productId, Guid fromDealerId, Guid toDealerId, int quantity, string reason)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var currentUserId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            
            var result = await _inventoryService.TransferStockAsync(productId, fromDealerId, toDealerId, quantity, reason, currentUserId);
            
            if (result)
            {
                TempData["Success"] = "Chuyển kho thành công.";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi chuyển kho. Vui lòng kiểm tra số lượng tồn kho.";
            }

            return RedirectToAction("StockTransfer");
        }

        [HttpGet]
        public async Task<IActionResult> StockAdjustment()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Dealers = await _inventoryService.GetAllDealersAsync();
            ViewBag.Products = await _inventoryService.GetAllProductsAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StockAdjustment(Guid productId, Guid dealerId, int quantity, string reason)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var currentUserId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            
            var result = await _inventoryService.AdjustStockAsync(productId, dealerId, quantity, reason, currentUserId);
            
            if (result)
            {
                TempData["Success"] = "Điều chỉnh tồn kho thành công.";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi điều chỉnh tồn kho.";
            }

            return RedirectToAction("StockAdjustment");
        }

        [HttpGet]
        public async Task<IActionResult> Transactions(Guid? productId = null, Guid? dealerId = null, string transactionType = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Set default date range if not provided
            if (!fromDate.HasValue)
                fromDate = DateTime.Now.AddDays(-30);
            if (!toDate.HasValue)
                toDate = DateTime.Now;

            var transactions = await _inventoryService.GetInventoryTransactionsAsync(productId, dealerId, transactionType, fromDate, toDate);

            ViewBag.ProductId = productId;
            ViewBag.DealerId = dealerId;
            ViewBag.TransactionType = transactionType;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            // Get dropdown data
            ViewBag.Dealers = await _inventoryService.GetAllDealersAsync();
            ViewBag.Products = await _inventoryService.GetAllProductsAsync();

            return View(transactions);
        }

        // API endpoints for AJAX calls
        [HttpGet]
        public async Task<JsonResult> GetStockSummary()
        {
            if (!IsAdmin())
                return Json(new { error = "Unauthorized" });

            var summary = await _inventoryService.GetStockSummaryAsync();
            return Json(summary);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllocationsByDealer(Guid dealerId)
        {
            if (!IsAdmin())
                return Json(new { error = "Unauthorized" });

            var allocations = await _inventoryService.GetInventoryAllocationsByDealerAsync(dealerId);
            return Json(allocations);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllocationsByProduct(Guid productId)
        {
            if (!IsAdmin())
                return Json(new { error = "Unauthorized" });

            var allocations = await _inventoryService.GetInventoryAllocationsByProductAsync(productId);
            return Json(allocations);
        }
    }
}
