using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;
        public OrderRepository(AppDbContext db) => _db = db;

        public Task<Order?> GetByIdAsync(Guid id)
            => _db.Order
                .Include(o => o.Product)
                .Include(o => o.Customer)
                .Include(o => o.Dealer)
                .Include(o => o.Region)
                .Include(o => o.SalesPerson)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<List<Order>> GetAllAsync(Guid? dealerId = null, string? status = null)
        {
            var query = _db.Order
                .Include(o => o.Product)
                .Include(o => o.Customer)
                .Include(o => o.Dealer)
                .Include(o => o.Region)
                .Include(o => o.SalesPerson)
                .AsQueryable();

            if (dealerId.HasValue)
                query = query.Where(o => o.DealerId == dealerId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            return await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(Order order)
        {
            await _db.Order.AddAsync(order);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            _db.Order.Update(order);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}


