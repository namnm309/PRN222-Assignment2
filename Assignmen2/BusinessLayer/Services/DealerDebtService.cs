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
                .Where(o => o.DealerId == dealerId);

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(o => o.PaymentStatus == paymentStatus);

            var orders = await query
                .OrderByDescending(o => o.PaymentDueDate)
                .ToListAsync();

            // Tính tổng công nợ chỉ cho những đơn chưa thanh toán đầy đủ
            var totalDebt = orders
                .Where(o => o.PaymentStatus != "Paid")
                .Sum(o => o.FinalAmount);

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

        public async Task<(bool Success, string? Error)> ProcessPaymentAsync(Guid orderId, decimal amount, string method, string? note = null)
        {
            if (amount <= 0)
            {
                return (false, "Số tiền thanh toán phải lớn hơn 0");
            }

            var order = await _context.Order.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return (false, "Không tìm thấy đơn hàng");
            }

            // Hiện tại chưa có bảng Payment, tạm thời cập nhật PaymentStatus theo số tiền thanh toán
            // Giả định FinalAmount là tổng nợ (chưa trừ), nếu amount >= FinalAmount => Paid, else Partial
            if (amount >= order.FinalAmount)
            {
                order.PaymentStatus = "Paid";
                order.PaymentMethod = method;
            }
            else
            {
                order.PaymentStatus = "Partial";
                order.PaymentMethod = method;
            }

            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ExtendPaymentAsync(Guid orderId, DateTime newDueDate, string? reason = null)
        {
            var order = await _context.Order.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return (false, "Không tìm thấy đơn hàng");
            }

            if (newDueDate.Date <= DateTime.UtcNow.Date)
            {
                return (false, "Ngày gia hạn phải sau ngày hiện tại");
            }

            order.PaymentDueDate = newDueDate;
            if (order.PaymentStatus == "Overdue")
            {
                order.PaymentStatus = "Pending"; // reset về chờ thanh toán sau khi gia hạn
            }
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}
