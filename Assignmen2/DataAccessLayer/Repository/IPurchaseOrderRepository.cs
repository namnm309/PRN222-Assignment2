using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Enum;

namespace DataAccessLayer.Repository
{
    public interface IPurchaseOrderRepository
    {
        Task<PurchaseOrder?> GetByIdAsync(Guid id);
        Task<List<PurchaseOrder>> GetAllAsync(Guid? dealerId = null, PurchaseOrderStatus? status = null);
        Task<bool> CreateAsync(PurchaseOrder purchaseOrder);
        Task<bool> UpdateAsync(PurchaseOrder purchaseOrder);
        Task<bool> DeleteAsync(Guid id);
        Task<string> GenerateOrderNumberAsync();
    }
}
