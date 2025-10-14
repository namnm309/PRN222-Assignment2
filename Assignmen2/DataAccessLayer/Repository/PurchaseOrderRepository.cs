using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Enum;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly AppDbContext _db;

        public PurchaseOrderRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<PurchaseOrder?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _db.PurchaseOrder
                    .Include(p => p.Dealer)
                    .Include(p => p.Product)
                        .ThenInclude(p => p.Brand)
                    .Include(p => p.RequestedBy)
                    .Include(p => p.ApprovedBy)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<PurchaseOrder>> GetAllAsync(Guid? dealerId = null, PurchaseOrderStatus? status = null)
        {
            try
            {
                var query = _db.PurchaseOrder
                    .Include(p => p.Dealer)
                    .Include(p => p.Product)
                        .ThenInclude(p => p.Brand)
                    .Include(p => p.RequestedBy)
                    .Include(p => p.ApprovedBy)
                    .AsQueryable();

                if (dealerId.HasValue)
                {
                    query = query.Where(p => p.DealerId == dealerId.Value);
                }

                if (status.HasValue)
                {
                    query = query.Where(p => p.Status == status.Value);
                }

                return await query
                    .OrderByDescending(p => p.RequestedDate)
                    .ToListAsync();
            }
            catch
            {
                return new List<PurchaseOrder>();
            }
        }

        public async Task<bool> CreateAsync(PurchaseOrder purchaseOrder)
        {
            try
            {
                purchaseOrder.Id = Guid.NewGuid();
                purchaseOrder.CreatedAt = DateTime.UtcNow;
                purchaseOrder.UpdatedAt = DateTime.UtcNow;

                await _db.PurchaseOrder.AddAsync(purchaseOrder);
                var result = await _db.SaveChangesAsync();
                Console.WriteLine($"[DEBUG] PurchaseOrder CreateAsync: SaveChanges result = {result}");
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] PurchaseOrder CreateAsync: {ex.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[ERROR] Inner: {ex.InnerException.Message}");
                    Console.WriteLine($"[ERROR] Inner StackTrace: {ex.InnerException.StackTrace}");
                }
                return false;
            }
        }

        public async Task<bool> UpdateAsync(PurchaseOrder purchaseOrder)
        {
            try
            {
                purchaseOrder.UpdatedAt = DateTime.UtcNow;
                _db.PurchaseOrder.Update(purchaseOrder);
                return await _db.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var purchaseOrder = await _db.PurchaseOrder.FindAsync(id);
                if (purchaseOrder == null) return false;

                _db.PurchaseOrder.Remove(purchaseOrder);
                return await _db.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            try
            {
                var today = DateTime.UtcNow;
                var prefix = $"PO{today:yyyyMM}";
                
                var lastOrder = await _db.PurchaseOrder
                    .Where(p => p.OrderNumber.StartsWith(prefix))
                    .OrderByDescending(p => p.OrderNumber)
                    .FirstOrDefaultAsync();

                if (lastOrder == null)
                {
                    return $"{prefix}001";
                }

                var lastNumberStr = lastOrder.OrderNumber.Substring(prefix.Length);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    return $"{prefix}{(lastNumber + 1):D3}";
                }

                return $"{prefix}001";
            }
            catch
            {
                return $"PO{DateTime.UtcNow:yyyyMMddHHmmss}";
            }
        }
    }
}
