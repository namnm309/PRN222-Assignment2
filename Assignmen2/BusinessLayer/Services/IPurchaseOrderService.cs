using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTOs.Responses;
using BusinessLayer.Enums;

namespace BusinessLayer.Services
{
    public interface IPurchaseOrderService
    {
        Task<(bool Success, string Error, PurchaseOrderResponse Data)> GetAsync(Guid id);
        Task<(bool Success, string Error, List<PurchaseOrderResponse> Data)> GetAllAsync(Guid? dealerId = null, PurchaseOrderStatus? status = null);
        Task<(bool Success, string Error, PurchaseOrderResponse Data)> CreateAsync(Guid dealerId, Guid productId, Guid requestedById, int quantity, decimal unitPrice, string reason, string notes, DateTime? expectedDeliveryDate = null);
        Task<(bool Success, string Error, PurchaseOrderResponse Data)> ApproveAsync(Guid id, Guid approvedById, DateTime? expectedDeliveryDate = null, string notes = "");
        Task<(bool Success, string Error, PurchaseOrderResponse Data)> RejectAsync(Guid id, Guid rejectedById, string rejectReason);
        Task<(bool Success, string Error, PurchaseOrderResponse Data)> UpdateStatusAsync(Guid id, PurchaseOrderStatus status, DateTime? actualDeliveryDate = null);
        Task<(bool Success, string Error, PurchaseOrderResponse Data)> CancelAsync(Guid id);
    }
}
