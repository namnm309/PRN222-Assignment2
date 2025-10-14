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
    }
}
