using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class DealerDebtService : IDealerDebtService
    {
        private readonly AppDbContext _context;

        public DealerDebtService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Order> Orders, decimal TotalDebt)> GetDebtReportAsync(
            Guid dealerId, 
            Guid? customerId = null, 
            string? paymentStatus = null)
        {
            var query = _context.Order
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .Include(o => o.SalesPerson)
                .Where(o => o.DealerId == dealerId && o.PaymentStatus != "Paid");

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(o => o.PaymentStatus == paymentStatus);

            var orders = await query
                .OrderByDescending(o => o.PaymentDueDate)
                .ToListAsync();

            var totalDebt = orders.Sum(o => o.FinalAmount);

            return (orders, totalDebt);
        }

        public async Task<List<Customer>> GetDealerCustomersAsync(Guid dealerId)
        {
            var customerIds = await _context.Order
                .Where(o => o.DealerId == dealerId)
                .Select(o => o.CustomerId)
                .Distinct()
                .ToListAsync();

            var customers = await _context.Customer
                .Where(c => customerIds.Contains(c.Id))
                .OrderBy(c => c.FullName)
                .ToListAsync();

            return customers;
        }
    }
}
