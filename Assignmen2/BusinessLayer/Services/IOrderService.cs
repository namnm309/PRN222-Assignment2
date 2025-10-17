using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services
{
    public interface IOrderService
    {
        Task<(bool Success, string Error, Order Data)> GetAsync(Guid id);
        Task<(bool Success, string Error, Order Data)> GetByIdAsync(Guid id);
        Task<(bool Success, string Error, List<Order> Data)> GetAllAsync(Guid? dealerId = null, string? status = null);
        Task<(bool Success, string Error, (List<Order> Data, int TotalPages))> SearchAsync(Guid dealerId, string? search, string? status, string? orderType, int page, int pageSize);
        Task<(bool Success, string Error, Order Data)> CreateQuotationAsync(Guid productId, Guid customerId, Guid dealerId, Guid? salesPersonId, decimal price, decimal discount, string orderType, string notes);
        Task<(bool Success, string Error, Order Data)> ConfirmOrderAsync(Guid orderId);
        Task<(bool Success, string Error, Order Data)> UpdatePaymentAsync(Guid orderId, string paymentStatus, string? paymentMethod, DateTime? paymentDueDate);
        Task<(bool Success, string Error, Order Data)> DeliverOrderAsync(Guid orderId, DateTime deliveryDate);
        Task<(bool Success, string Error, Order Data)> CancelOrderAsync(Guid orderId);
        Task<(bool Success, string Error, Order Data)> UpdateOrderAsync(Order order);
        Task<(bool Success, string Error)> DeleteOrderAsync(Guid orderId);
    }
}

