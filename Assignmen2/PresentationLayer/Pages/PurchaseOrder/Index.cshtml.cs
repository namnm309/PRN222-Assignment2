using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using BusinessLayer.Enums;
using BusinessLayer.Helpers;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using System.Linq;

namespace PresentationLayer.Pages.EVMStaff.PurchaseOrderManagement
{
    public class IndexModel : BaseEVMStaffPageModel
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IMappingService _mappingService;

        public IndexModel(
            IPurchaseOrderService purchaseOrderService,
            IMappingService mappingService)
        {
            _purchaseOrderService = purchaseOrderService;
            _mappingService = mappingService;
        }

        public List<PurchaseOrderResponse> PurchaseOrders { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? Status { get; set; } // 0=Pending, 1=Approved, 2=Rejected, 3=InTransit, 4=Delivered, 5=Cancelled

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            PurchaseOrderStatus? statusFilter = null;
            if (Status.HasValue)
            {
                statusFilter = (PurchaseOrderStatus)Status.Value;
            }

            var result = await _purchaseOrderService.GetAllAsync(null, statusFilter);

            if (result.Success && result.Data != null)
            {
                PurchaseOrders = _mappingService.MapToPurchaseOrderViewModels(result.Data);

                // Apply search filter (case-insensitive, diacritics-insensitive)
                if (!string.IsNullOrWhiteSpace(Search))
                {
                    PurchaseOrders = PurchaseOrders.Where(p =>
                        SearchHelper.Contains(p.OrderNumber, Search) ||
                        SearchHelper.Contains(p.DealerName, Search) ||
                        SearchHelper.Contains(p.ProductName, Search)
                    ).ToList();
                }
            }
            else
            {
                TempData["Error"] = result.Error ?? "Không thể tải danh sách đơn hàng";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(Guid id)
        {
            if (!CurrentUserId.HasValue)
            {
                TempData["Error"] = "Không xác định được người dùng";
                return RedirectToPage();
            }

            var result = await _purchaseOrderService.ApproveAsync(id, CurrentUserId.Value);

            if (result.Success)
            {
                TempData["Success"] = "Duyệt đơn hàng thành công!";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Không thể duyệt đơn hàng";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(Guid id, string rejectReason)
        {
            if (!CurrentUserId.HasValue)
            {
                TempData["Error"] = "Không xác định được người dùng";
                return RedirectToPage();
            }

            var result = await _purchaseOrderService.RejectAsync(id, CurrentUserId.Value, rejectReason);

            if (result.Success)
            {
                TempData["Success"] = "Từ chối đơn hàng thành công!";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Không thể từ chối đơn hàng";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(Guid id, int status)
        {
            var result = await _purchaseOrderService.UpdateStatusAsync(id, (PurchaseOrderStatus)status);

            if (result.Success)
            {
                TempData["Success"] = "Cập nhật trạng thái thành công!";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Không thể cập nhật trạng thái";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCancelAsync(Guid id)
        {
            var result = await _purchaseOrderService.CancelAsync(id);

            if (result.Success)
            {
                TempData["Success"] = "Hủy đơn hàng thành công!";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Không thể hủy đơn hàng";
            }

            return RedirectToPage();
        }
    }
}

