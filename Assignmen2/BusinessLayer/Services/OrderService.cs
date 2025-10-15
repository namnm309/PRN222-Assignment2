using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly AppDbContext _dbContext;
        
        public OrderService(IOrderRepository repo, AppDbContext dbContext)
        {
            _repo = repo;
            _dbContext = dbContext;
        }

        public async Task<(bool Success, string Error, Order Data)> GetAsync(Guid id)
        {
            var order = await _repo.GetByIdAsync(id);
            return order == null ? (false, "Không tìm thấy đơn hàng", null) : (true, null, order);
        }

        public async Task<(bool Success, string Error, List<Order> Data)> GetAllAsync(Guid? dealerId = null, string? status = null)
        {
            var list = await _repo.GetAllAsync(dealerId, status);
            return (true, null, list);
        }

        public async Task<(bool Success, string Error, (List<Order> Data, int TotalPages))> SearchAsync(
            Guid dealerId, string? search, string? status, string? orderType, int page, int pageSize)
        {
            try
            {
                var query = _dbContext.Order
                    .Include(o => o.Customer)
                    .Include(o => o.Product)
                    .ThenInclude(p => p.Brand)
                    .Where(o => o.DealerId == dealerId);

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var searchTerm = search.Trim().ToLower();
                    query = query.Where(o => 
                        o.OrderNumber.ToLower().Contains(searchTerm) ||
                        o.Customer.FullName.ToLower().Contains(searchTerm) ||
                        o.Product.Name.ToLower().Contains(searchTerm));
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(o => o.Status == status);
                }

                // Apply order type filter (using Description field)
                if (!string.IsNullOrWhiteSpace(orderType))
                {
                    query = query.Where(o => o.Description == orderType);
                }

                // Get total count
                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Apply pagination
                var orders = await query
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (true, null, (orders, totalPages));
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi tìm kiếm: {ex.Message}", (new List<Order>(), 0));
            }
        }

        public async Task<(bool Success, string Error, Order Data)> CreateQuotationAsync(
            Guid productId, Guid customerId, Guid dealerId, Guid? salesPersonId,
            decimal price, decimal discount, string description, string notes)
        {
            if (productId == Guid.Empty || customerId == Guid.Empty || dealerId == Guid.Empty)
                return (false, "Thiếu thông tin bắt buộc", null);

            if (price <= 0)
                return (false, "Giá không hợp lệ", null);

            var finalAmount = price - discount;
            if (finalAmount < 0)
                return (false, "Giảm giá không hợp lệ", null);

            var order = new Order
            {
                OrderNumber = $"QT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                ProductId = productId,
                CustomerId = customerId,
                DealerId = dealerId,
                SalesPersonId = salesPersonId,
                Price = price,
                Discount = discount,
                FinalAmount = finalAmount,
                Description = description ?? "",
                Notes = notes ?? "",
                Status = "Draft", // Quotation status
                PaymentStatus = "Unpaid",
                PaymentMethod = "",
                OrderDate = null,
                DeliveryDate = null,
                PaymentDueDate = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var ok = await _repo.CreateAsync(order);
            return ok ? (true, null, order) : (false, "Không thể tạo báo giá", null);
        }

        public async Task<(bool Success, string Error, Order Data)> ConfirmOrderAsync(Guid orderId)
        {
            var order = await _repo.GetByIdAsync(orderId);
            if (order == null) return (false, "Không tìm thấy đơn hàng", null);

            if (order.Status != "Draft")
                return (false, "Chỉ có thể xác nhận báo giá ở trạng thái Draft", null);

            order.Status = "Confirmed";
            order.OrderDate = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            var ok = await _repo.UpdateAsync(order);
            return ok ? (true, null, order) : (false, "Không thể xác nhận đơn hàng", null);
        }

        public async Task<(bool Success, string Error, Order Data)> UpdatePaymentAsync(
            Guid orderId, string paymentStatus, string paymentMethod, DateTime? paymentDueDate)
        {
            var order = await _repo.GetByIdAsync(orderId);
            if (order == null) return (false, "Không tìm thấy đơn hàng", null);

            order.PaymentStatus = paymentStatus ?? "Unpaid";
            order.PaymentMethod = paymentMethod ?? "";
            order.PaymentDueDate = paymentDueDate.HasValue 
                ? (paymentDueDate.Value.Kind == DateTimeKind.Utc 
                    ? paymentDueDate.Value 
                    : paymentDueDate.Value.ToUniversalTime())
                : null;
            order.UpdatedAt = DateTime.UtcNow;

            if (paymentStatus == "Paid")
                order.Status = "Paid";

            var ok = await _repo.UpdateAsync(order);
            return ok ? (true, null, order) : (false, "Không thể cập nhật thanh toán", null);
        }

        public async Task<(bool Success, string Error, Order Data)> DeliverOrderAsync(Guid orderId, DateTime deliveryDate)
        {
            var order = await _repo.GetByIdAsync(orderId);
            if (order == null) return (false, "Không tìm thấy đơn hàng", null);

            if (order.Status != "Paid" && order.Status != "Confirmed")
                return (false, "Chỉ có thể giao hàng khi đã xác nhận hoặc thanh toán", null);

            var deliveryUtc = deliveryDate.Kind switch
            {
                DateTimeKind.Utc => deliveryDate,
                DateTimeKind.Local => deliveryDate.ToUniversalTime(),
                _ => DateTime.SpecifyKind(deliveryDate, DateTimeKind.Local).ToUniversalTime()
            };

            order.DeliveryDate = deliveryUtc;
            order.Status = "Delivered";
            order.UpdatedAt = DateTime.UtcNow;

            // 🔥 GIẢM TỒN KHO ĐẠI LÝ KHI GIAO HÀNG CHO KHÁCH
            try
            {
                Console.WriteLine($"[Order Delivered] Reducing inventory for Dealer={order.DealerId}, Product={order.ProductId}");
                
                var inventory = await _dbContext.InventoryAllocation
                    .FirstOrDefaultAsync(i => i.DealerId == order.DealerId 
                                           && i.ProductId == order.ProductId 
                                           && i.IsActive);

                if (inventory != null)
                {
                    if (inventory.AvailableQuantity > 0)
                    {
                        inventory.AvailableQuantity -= 1; // Giảm 1 xe
                        inventory.UpdatedAt = DateTime.UtcNow;
                        inventory.Notes = string.IsNullOrWhiteSpace(inventory.Notes)
                            ? $"Bán xe cho khách - Order #{order.OrderNumber}"
                            : $"{inventory.Notes}\n[{DateTime.Now:dd/MM/yyyy HH:mm}] Bán 1 xe - Order #{order.OrderNumber}";
                        
                        // Cập nhật status nếu hết hàng
                        if (inventory.AvailableQuantity <= 0)
                        {
                            inventory.Status = "OutOfStock";
                        }
                        else if (inventory.AvailableQuantity <= inventory.MinimumStock)
                        {
                            inventory.Status = "LowStock";
                        }

                        _dbContext.InventoryAllocation.Update(inventory);
                        await _dbContext.SaveChangesAsync();
                        Console.WriteLine($"[Order Delivered] Inventory reduced successfully! AvailableQty={inventory.AvailableQuantity}");
                    }
                    else
                    {
                        Console.WriteLine($"[Order Delivered] WARNING: AvailableQuantity is 0, cannot reduce!");
                    }
                }
                else
                {
                    Console.WriteLine($"[Order Delivered] WARNING: No inventory allocation found for this dealer/product!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Order Delivered] ERROR reducing inventory: {ex.Message}");
                // Không return lỗi, chỉ ghi log - vẫn cho phép giao hàng
            }

            var ok = await _repo.UpdateAsync(order);
            return ok ? (true, null, order) : (false, "Không thể cập nhật giao hàng", null);
        }

        public async Task<(bool Success, string Error, Order Data)> CancelOrderAsync(Guid orderId)
        {
            var order = await _repo.GetByIdAsync(orderId);
            if (order == null) return (false, "Không tìm thấy đơn hàng", null);

            if (order.Status == "Delivered")
                return (false, "Không thể hủy đơn đã giao", null);

            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.UtcNow;

            var ok = await _repo.UpdateAsync(order);
            return ok ? (true, null, order) : (false, "Không thể hủy đơn hàng", null);
        }
    }
}

