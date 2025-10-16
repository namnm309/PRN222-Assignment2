using System;
using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.InventoryManagement
{
    public class IndexModel : BaseEVMStaffPageModel
    {
        private readonly IInventoryManagementService _inventoryService;
        private readonly IMappingService _mappingService;

        public IndexModel(
            IInventoryManagementService inventoryService,
            IMappingService mappingService)
        {
            _inventoryService = inventoryService;
            _mappingService = mappingService;
        }

        public List<InventoryAllocationResponse> Allocations { get; set; } = new();
        public List<InventoryTransactionResponse> Transactions { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? View { get; set; } = "allocations"; // "allocations" or "transactions"

        public async Task<IActionResult> OnGetAsync()
        {
            if (View == "transactions")
            {
                await LoadTransactionsAsync();
            }
            else
            {
                await LoadAllocationsAsync();
            }

            return Page();
        }

        private async Task LoadAllocationsAsync()
        {
            try
            {
                var allocations = await _inventoryService.GetAllInventoryAllocationsAsync();
                if (allocations != null)
                {
                    Allocations = allocations;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Không thể tải danh sách phân bổ: {ex.Message}";
            }
        }

        private async Task LoadTransactionsAsync()
        {
            try
            {
                var transactions = await _inventoryService.GetInventoryTransactionsAsync();
                if (transactions != null)
                {
                    Transactions = transactions;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Không thể tải lịch sử giao dịch: {ex.Message}";
            }
        }
    }
}

