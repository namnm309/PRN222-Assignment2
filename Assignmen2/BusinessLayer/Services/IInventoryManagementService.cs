using BusinessLayer.DTOs.Responses;
using BusinessLayer.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IInventoryManagementService
    {
        // Inventory Allocation Management
        Task<List<InventoryAllocationResponse>> GetAllInventoryAllocationsAsync();
        Task<List<InventoryAllocationResponse>> GetInventoryAllocationsByDealerAsync(Guid dealerId);
        Task<List<InventoryAllocationResponse>> GetInventoryAllocationsByProductAsync(Guid productId);
        Task<InventoryAllocationResponse?> GetInventoryAllocationAsync(Guid productId, Guid dealerId);
        Task<(bool Success, string Error, InventoryAllocationResponse Data)> GetInventoryByDealerAndProductAsync(Guid dealerId, Guid productId);
        Task<bool> CreateInventoryAllocationAsync(InventoryAllocationResponse allocation);
        Task<bool> UpdateInventoryAllocationAsync(InventoryAllocationResponse allocation);
        Task<bool> DeleteInventoryAllocationAsync(Guid id);

        // Stock Alerts
        Task<List<InventoryAllocationResponse>> GetLowStockAllocationsAsync();
        Task<List<InventoryAllocationResponse>> GetCriticalStockAllocationsAsync();
        Task<List<InventoryAllocationResponse>> GetOutOfStockAllocationsAsync();

        // Inventory Transactions
        Task<List<InventoryTransactionResponse>> GetInventoryTransactionsAsync(Guid? productId = null, Guid? dealerId = null, string transactionType = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> CreateInventoryTransactionAsync(InventoryTransactionCreateRequest transaction);

        // Stock Operations
        Task<bool> TransferStockAsync(Guid productId, Guid fromDealerId, Guid toDealerId, int quantity, string reason, Guid processedByUserId);
        Task<bool> AdjustStockAsync(Guid productId, Guid dealerId, int quantity, string reason, Guid processedByUserId);

        // Reports
        Task<List<InventoryAllocationResponse>> GetInventoryReportAsync(Guid? dealerId = null, Guid? productId = null, string status = null);
        Task<Dictionary<string, int>> GetStockSummaryAsync();
        Task<List<InventoryTransactionResponse>> GetStockMovementReportAsync(DateTime fromDate, DateTime toDate, Guid? productId = null, Guid? dealerId = null);

        // Support methods
        Task<List<ProductResponse>> GetAllProductsAsync();
        Task<List<DealerResponse>> GetAllDealersAsync();
    }
}
