using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class InventoryManagementRepository : IInventoryManagementRepository
    {
        private readonly AppDbContext _context;

        public InventoryManagementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventoryAllocation>> GetAllInventoryAllocationsAsync()
        {
            return await _context.InventoryAllocation
                .Include(ia => ia.Product)
                    .ThenInclude(p => p.Brand)
                .Include(ia => ia.Dealer)
                .Where(ia => ia.IsActive)
                .OrderBy(ia => ia.Dealer.Name)
                .ThenBy(ia => ia.Product.Name)
                .ToListAsync();
        }

        public async Task<List<InventoryAllocation>> GetInventoryAllocationsByDealerAsync(Guid dealerId)
        {
            return await _context.InventoryAllocation
                .Include(ia => ia.Product)
                    .ThenInclude(p => p.Brand)
                .Include(ia => ia.Dealer)
                .Where(ia => ia.DealerId == dealerId && ia.IsActive)
                .OrderBy(ia => ia.Product.Name)
                .ToListAsync();
        }

        public async Task<List<InventoryAllocation>> GetInventoryAllocationsByProductAsync(Guid productId)
        {
            return await _context.InventoryAllocation
                .Include(ia => ia.Product)
                    .ThenInclude(p => p.Brand)
                .Include(ia => ia.Dealer)
                .Where(ia => ia.ProductId == productId && ia.IsActive)
                .OrderBy(ia => ia.Dealer.Name)
                .ToListAsync();
        }

        public async Task<InventoryAllocation> GetInventoryAllocationAsync(Guid productId, Guid dealerId)
        {
            return await _context.InventoryAllocation
                .Include(ia => ia.Product)
                    .ThenInclude(p => p.Brand)
                .Include(ia => ia.Dealer)
                .FirstOrDefaultAsync(ia => ia.ProductId == productId && ia.DealerId == dealerId && ia.IsActive);
        }

        public async Task<bool> CreateInventoryAllocationAsync(InventoryAllocation allocation)
        {
            allocation.CreatedAt = DateTime.UtcNow;
            allocation.UpdatedAt = DateTime.UtcNow;
            
            await _context.InventoryAllocation.AddAsync(allocation);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateInventoryAllocationAsync(InventoryAllocation allocation)
        {
            allocation.UpdatedAt = DateTime.UtcNow;
            
            _context.InventoryAllocation.Update(allocation);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteInventoryAllocationAsync(Guid id)
        {
            var allocation = await _context.InventoryAllocation.FindAsync(id);
            if (allocation == null) return false;

            allocation.IsActive = false;
            allocation.UpdatedAt = DateTime.UtcNow;
            
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<InventoryAllocation>> GetLowStockAllocationsAsync()
        {
            return await _context.InventoryAllocation
                .Include(ia => ia.Product)
                    .ThenInclude(p => p.Brand)
                .Include(ia => ia.Dealer)
                .Where(ia => ia.IsActive && ia.AvailableQuantity <= ia.MinimumStock && ia.AvailableQuantity > 0)
                .OrderBy(ia => ia.AvailableQuantity)
                .ToListAsync();
        }

        public async Task<List<InventoryAllocation>> GetCriticalStockAllocationsAsync()
        {
            return await _context.InventoryAllocation
                .Include(ia => ia.Product)
                    .ThenInclude(p => p.Brand)
                .Include(ia => ia.Dealer)
                .Where(ia => ia.IsActive && ia.AvailableQuantity <= (ia.MinimumStock * 0.5) && ia.AvailableQuantity > 0)
                .OrderBy(ia => ia.AvailableQuantity)
                .ToListAsync();
        }

        public async Task<List<InventoryAllocation>> GetOutOfStockAllocationsAsync()
        {
            return await _context.InventoryAllocation
                .Include(ia => ia.Product)
                    .ThenInclude(p => p.Brand)
                .Include(ia => ia.Dealer)
                .Where(ia => ia.IsActive && ia.AvailableQuantity <= 0)
                .OrderBy(ia => ia.Dealer.Name)
                .ThenBy(ia => ia.Product.Name)
                .ToListAsync();
        }

        public async Task<List<InventoryTransaction>> GetInventoryTransactionsAsync(Guid? productId = null, Guid? dealerId = null, string transactionType = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.InventoryTransaction
                .Include(it => it.Product)
                    .ThenInclude(p => p.Brand)
                .Include(it => it.Dealer)
                .Include(it => it.ProcessedByUser)
                .AsQueryable();

            if (productId.HasValue)
                query = query.Where(it => it.ProductId == productId.Value);

            if (dealerId.HasValue)
                query = query.Where(it => it.DealerId == dealerId.Value);

            if (!string.IsNullOrEmpty(transactionType))
                query = query.Where(it => it.TransactionType == transactionType);

            if (fromDate.HasValue)
                query = query.Where(it => it.TransactionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(it => it.TransactionDate <= toDate.Value);

            return await query
                .OrderByDescending(it => it.TransactionDate)
                .ToListAsync();
        }

        public async Task<bool> CreateInventoryTransactionAsync(InventoryTransaction transaction)
        {
            transaction.CreatedAt = DateTime.UtcNow;
            transaction.UpdatedAt = DateTime.UtcNow;
            transaction.TransactionDate = DateTime.UtcNow;
            
            await _context.InventoryTransaction.AddAsync(transaction);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateInventoryTransactionAsync(InventoryTransaction transaction)
        {
            transaction.UpdatedAt = DateTime.UtcNow;
            
            _context.InventoryTransaction.Update(transaction);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> TransferStockAsync(Guid productId, Guid fromDealerId, Guid toDealerId, int quantity, string reason, Guid processedByUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get source allocation
                var fromAllocation = await GetInventoryAllocationAsync(productId, fromDealerId);
                if (fromAllocation == null || fromAllocation.AvailableQuantity < quantity)
                    return false;

                // Get or create destination allocation
                var toAllocation = await GetInventoryAllocationAsync(productId, toDealerId);
                if (toAllocation == null)
                {
                    toAllocation = new InventoryAllocation
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        DealerId = toDealerId,
                        AllocatedQuantity = 0,
                        ReservedQuantity = 0,
                        AvailableQuantity = 0,
                        MinimumStock = 5,
                        MaximumStock = 100,
                        LastRestockDate = DateTime.UtcNow,
                        Status = "Active",
                        Priority = "Normal",
                        IsActive = true
                    };
                    await CreateInventoryAllocationAsync(toAllocation);
                }

                // Update allocations
                fromAllocation.AvailableQuantity -= quantity;
                fromAllocation.AllocatedQuantity -= quantity;
                await UpdateInventoryAllocationAsync(fromAllocation);

                toAllocation.AvailableQuantity += quantity;
                toAllocation.AllocatedQuantity += quantity;
                toAllocation.LastRestockDate = DateTime.UtcNow;
                await UpdateInventoryAllocationAsync(toAllocation);

                // Create transactions
                var outTransaction = new InventoryTransaction
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    DealerId = fromDealerId,
                    TransactionType = "OUT",
                    Quantity = -quantity,
                    QuantityBefore = fromAllocation.AvailableQuantity + quantity,
                    QuantityAfter = fromAllocation.AvailableQuantity,
                    Reason = reason,
                    ReferenceNumber = $"TRF-{DateTime.Now:yyyyMMddHHmmss}",
                    ProcessedByUserId = processedByUserId,
                    Status = "Completed"
                };

                var inTransaction = new InventoryTransaction
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    DealerId = toDealerId,
                    TransactionType = "IN",
                    Quantity = quantity,
                    QuantityBefore = toAllocation.AvailableQuantity - quantity,
                    QuantityAfter = toAllocation.AvailableQuantity,
                    Reason = reason,
                    ReferenceNumber = outTransaction.ReferenceNumber,
                    ProcessedByUserId = processedByUserId,
                    Status = "Completed"
                };

                await CreateInventoryTransactionAsync(outTransaction);
                await CreateInventoryTransactionAsync(inTransaction);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> AdjustStockAsync(Guid productId, Guid dealerId, int quantity, string reason, Guid processedByUserId)
        {
            var allocation = await GetInventoryAllocationAsync(productId, dealerId);
            if (allocation == null) return false;

            var oldQuantity = allocation.AvailableQuantity;
            allocation.AvailableQuantity += quantity;
            allocation.AllocatedQuantity += quantity;
            
            if (quantity > 0)
                allocation.LastRestockDate = DateTime.UtcNow;

            await UpdateInventoryAllocationAsync(allocation);

            // Create transaction
            var adjustmentTransaction = new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                DealerId = dealerId,
                TransactionType = quantity > 0 ? "IN" : "OUT",
                Quantity = quantity,
                QuantityBefore = oldQuantity,
                QuantityAfter = allocation.AvailableQuantity,
                Reason = reason,
                ReferenceNumber = $"ADJ-{DateTime.Now:yyyyMMddHHmmss}",
                ProcessedByUserId = processedByUserId,
                Status = "Completed"
            };

            return await CreateInventoryTransactionAsync(adjustmentTransaction);
        }

        public async Task<List<InventoryAllocation>> GetInventoryReportAsync(Guid? dealerId = null, Guid? productId = null, string status = null)
        {
            var query = _context.InventoryAllocation
                .Include(ia => ia.Product)
                    .ThenInclude(p => p.Brand)
                .Include(ia => ia.Dealer)
                .Where(ia => ia.IsActive)
                .AsQueryable();

            if (dealerId.HasValue)
                query = query.Where(ia => ia.DealerId == dealerId.Value);

            if (productId.HasValue)
                query = query.Where(ia => ia.ProductId == productId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(ia => ia.Status == status);

            return await query
                .OrderBy(ia => ia.Dealer.Name)
                .ThenBy(ia => ia.Product.Name)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetStockSummaryAsync()
        {
            var summary = new Dictionary<string, int>();

            summary["TotalAllocations"] = await _context.InventoryAllocation.CountAsync(ia => ia.IsActive);
            summary["TotalStock"] = await _context.InventoryAllocation.Where(ia => ia.IsActive).SumAsync(ia => ia.AvailableQuantity);
            summary["LowStock"] = await _context.InventoryAllocation.CountAsync(ia => ia.IsActive && ia.AvailableQuantity <= ia.MinimumStock);
            summary["OutOfStock"] = await _context.InventoryAllocation.CountAsync(ia => ia.IsActive && ia.AvailableQuantity <= 0);
            summary["CriticalStock"] = await _context.InventoryAllocation.CountAsync(ia => ia.IsActive && ia.AvailableQuantity <= (ia.MinimumStock * 0.5));

            return summary;
        }

        public async Task<List<InventoryTransaction>> GetStockMovementReportAsync(DateTime fromDate, DateTime toDate, Guid? productId = null, Guid? dealerId = null)
        {
            var query = _context.InventoryTransaction
                .Include(it => it.Product)
                    .ThenInclude(p => p.Brand)
                .Include(it => it.Dealer)
                .Include(it => it.ProcessedByUser)
                .Where(it => it.TransactionDate >= fromDate && it.TransactionDate <= toDate)
                .AsQueryable();

            if (productId.HasValue)
                query = query.Where(it => it.ProductId == productId.Value);

            if (dealerId.HasValue)
                query = query.Where(it => it.DealerId == dealerId.Value);

            return await query
                .OrderByDescending(it => it.TransactionDate)
                .ToListAsync();
        }
    }
}
