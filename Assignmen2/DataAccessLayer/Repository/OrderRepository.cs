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
            try
            {
                Console.WriteLine($"[DEBUG] OrderRepository.UpdateAsync called for order: {order.Id}, Status: {order.Status}, PaymentStatus: {order.PaymentStatus}");
                
                // Use raw SQL to update the order - only update non-null fields
                var sql = @"
                    UPDATE ""Order"" 
                    SET ""Status"" = COALESCE(@status, ""Status""),
                        ""OrderDate"" = COALESCE(@orderDate, ""OrderDate""),
                        ""UpdatedAt"" = @updatedAt,
                        ""PaymentStatus"" = COALESCE(@paymentStatus, ""PaymentStatus""),
                        ""PaymentMethod"" = COALESCE(@paymentMethod, ""PaymentMethod""),
                        ""PaymentDueDate"" = COALESCE(@paymentDueDate, ""PaymentDueDate""),
                        ""DeliveryDate"" = COALESCE(@deliveryDate, ""DeliveryDate""),
                        ""Notes"" = COALESCE(@notes, ""Notes"")
                    WHERE ""Id"" = @id";
                
                var parameters = new[]
                {
                    new Npgsql.NpgsqlParameter("@id", order.Id),
                    new Npgsql.NpgsqlParameter("@status", order.Status ?? (object)DBNull.Value),
                    new Npgsql.NpgsqlParameter("@orderDate", order.OrderDate ?? (object)DBNull.Value),
                    new Npgsql.NpgsqlParameter("@updatedAt", order.UpdatedAt),
                    new Npgsql.NpgsqlParameter("@paymentStatus", order.PaymentStatus ?? (object)DBNull.Value),
                    new Npgsql.NpgsqlParameter("@paymentMethod", order.PaymentMethod ?? (object)DBNull.Value),
                    new Npgsql.NpgsqlParameter("@paymentDueDate", order.PaymentDueDate ?? (object)DBNull.Value),
                    new Npgsql.NpgsqlParameter("@deliveryDate", order.DeliveryDate ?? (object)DBNull.Value),
                    new Npgsql.NpgsqlParameter("@notes", order.Notes ?? (object)DBNull.Value)
                };
                
                Console.WriteLine($"[DEBUG] Executing SQL update for order: {order.Id}");
                Console.WriteLine($"[DEBUG] Status to update: {order.Status}");
                Console.WriteLine($"[DEBUG] PaymentStatus to update: {order.PaymentStatus}");
                
                var result = await _db.Database.ExecuteSqlRawAsync(sql, parameters);
                
                Console.WriteLine($"[DEBUG] SQL update result: {result}");
                
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Exception in OrderRepository.UpdateAsync: {ex.Message}");
                Console.WriteLine($"[DEBUG] StackTrace: {ex.StackTrace}");
                return false;
            }
        }
    }
}


