using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public interface IInventoryManagementRepository
    {
        // Inventory Allocation Management
        Task<List<InventoryAllocation>> GetAllInventoryAllocationsAsync();
        Task<List<InventoryAllocation>> GetInventoryAllocationsByDealerAsync(Guid dealerId);
        Task<List<InventoryAllocation>> GetInventoryAllocationsByProductAsync(Guid productId);
        Task<InventoryAllocation> GetInventoryAllocationAsync(Guid productId, Guid dealerId);
        Task<bool> CreateInventoryAllocationAsync(InventoryAllocation allocation);
        Task<bool> UpdateInventoryAllocationAsync(InventoryAllocation allocation);
        Task<bool> DeleteInventoryAllocationAsync(Guid id);

        // Low Stock Alerts
        Task<List<InventoryAllocation>> GetLowStockAllocationsAsync();
        Task<List<InventoryAllocation>> GetCriticalStockAllocationsAsync();
        Task<List<InventoryAllocation>> GetOutOfStockAllocationsAsync();

        // Inventory Transactions
        Task<List<InventoryTransaction>> GetInventoryTransactionsAsync(Guid? productId = null, Guid? dealerId = null, string transactionType = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> CreateInventoryTransactionAsync(InventoryTransaction transaction);
        Task<bool> UpdateInventoryTransactionAsync(InventoryTransaction transaction);

        // Stock Transfer
        Task<bool> TransferStockAsync(Guid productId, Guid fromDealerId, Guid toDealerId, int quantity, string reason, Guid processedByUserId);
        Task<bool> AdjustStockAsync(Guid productId, Guid dealerId, int quantity, string reason, Guid processedByUserId);

        // Reports
        Task<List<InventoryAllocation>> GetInventoryReportAsync(Guid? dealerId = null, Guid? productId = null, string status = null);
        Task<Dictionary<string, int>> GetStockSummaryAsync();
        Task<List<InventoryTransaction>> GetStockMovementReportAsync(DateTime fromDate, DateTime toDate, Guid? productId = null, Guid? dealerId = null);
    }
}
