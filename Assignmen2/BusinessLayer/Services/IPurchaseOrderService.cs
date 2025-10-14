using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using BusinessLayer.Enums;

namespace BusinessLayer.Services
{
    public interface IPurchaseOrderService
    {
        Task<(bool Success, string Error, PurchaseOrder Data)> GetAsync(Guid id);
        Task<(bool Success, string Error, List<PurchaseOrder> Data)> GetAllAsync(Guid? dealerId = null, PurchaseOrderStatus? status = null);
        Task<(bool Success, string Error, PurchaseOrder Data)> CreateAsync(Guid dealerId, Guid productId, Guid requestedById, int quantity, decimal unitPrice, string reason, string notes, DateTime? expectedDeliveryDate = null);
        Task<(bool Success, string Error, PurchaseOrder Data)> ApproveAsync(Guid id, Guid approvedById, DateTime? expectedDeliveryDate = null, string notes = "");
        Task<(bool Success, string Error, PurchaseOrder Data)> RejectAsync(Guid id, Guid rejectedById, string rejectReason);
        Task<(bool Success, string Error, PurchaseOrder Data)> UpdateStatusAsync(Guid id, PurchaseOrderStatus status, DateTime? actualDeliveryDate = null);
        Task<(bool Success, string Error, PurchaseOrder Data)> CancelAsync(Guid id);
    }
}
