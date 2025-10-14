using BusinessLayer.ViewModels;
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
        Task<List<InventoryAllocationViewModel>> GetAllInventoryAllocationsAsync();
        Task<List<InventoryAllocationViewModel>> GetInventoryAllocationsByDealerAsync(Guid dealerId);
        Task<List<InventoryAllocationViewModel>> GetInventoryAllocationsByProductAsync(Guid productId);
        Task<InventoryAllocationViewModel?> GetInventoryAllocationAsync(Guid productId, Guid dealerId);
        Task<bool> CreateInventoryAllocationAsync(InventoryAllocationViewModel allocation);
        Task<bool> UpdateInventoryAllocationAsync(InventoryAllocationViewModel allocation);
        Task<bool> DeleteInventoryAllocationAsync(Guid id);

        // Stock Alerts
        Task<List<InventoryAllocationViewModel>> GetLowStockAllocationsAsync();
        Task<List<InventoryAllocationViewModel>> GetCriticalStockAllocationsAsync();
        Task<List<InventoryAllocationViewModel>> GetOutOfStockAllocationsAsync();

        // Inventory Transactions
        Task<List<InventoryTransactionViewModel>> GetInventoryTransactionsAsync(Guid? productId = null, Guid? dealerId = null, string transactionType = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> CreateInventoryTransactionAsync(InventoryTransactionViewModel transaction);

        // Stock Operations
        Task<bool> TransferStockAsync(Guid productId, Guid fromDealerId, Guid toDealerId, int quantity, string reason, Guid processedByUserId);
        Task<bool> AdjustStockAsync(Guid productId, Guid dealerId, int quantity, string reason, Guid processedByUserId);

        // Reports
        Task<List<InventoryAllocationViewModel>> GetInventoryReportAsync(Guid? dealerId = null, Guid? productId = null, string status = null);
        Task<Dictionary<string, int>> GetStockSummaryAsync();
        Task<List<InventoryTransactionViewModel>> GetStockMovementReportAsync(DateTime fromDate, DateTime toDate, Guid? productId = null, Guid? dealerId = null);

        // Support methods
        Task<List<ProductViewModel>> GetAllProductsAsync();
        Task<List<DealerViewModel>> GetAllDealersAsync();
    }
}
