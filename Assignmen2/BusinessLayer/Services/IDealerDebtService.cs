using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services
{
    public interface IDealerDebtService
    {
        Task<(List<Order> Orders, decimal TotalDebt)> GetDebtReportAsync(
            Guid dealerId, 
            Guid? customerId = null, 
            string? paymentStatus = null);
        
        Task<List<Customer>> GetDealerCustomersAsync(Guid dealerId);

        // Debt operations
        Task<(bool Success, string? Error)> ProcessPaymentAsync(Guid orderId, decimal amount, string method, string? note = null);
        Task<(bool Success, string? Error)> ExtendPaymentAsync(Guid orderId, DateTime newDueDate, string? reason = null);
    }
}
