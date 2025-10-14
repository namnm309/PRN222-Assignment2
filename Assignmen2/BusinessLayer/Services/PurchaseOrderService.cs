using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository;
using BusinessLayer.Enums;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _repo;
        private readonly IProductService _productService;
        private readonly AppDbContext _dbContext;

        public PurchaseOrderService(IPurchaseOrderRepository repo, IProductService productService, AppDbContext dbContext)
        {
            _repo = repo;
            _productService = productService;
            _dbContext = dbContext;
        }

        public async Task<(bool Success, string Error, PurchaseOrder Data)> GetAsync(Guid id)
        {
            if (id == Guid.Empty)
                return (false, "ID không hợp lệ", null);

            var purchaseOrder = await _repo.GetByIdAsync(id);
            if (purchaseOrder == null)
                return (false, "Không tìm thấy đơn đặt hàng", null);

            return (true, null, purchaseOrder);
        }

        public async Task<(bool Success, string Error, List<PurchaseOrder> Data)> GetAllAsync(Guid? dealerId = null, PurchaseOrderStatus? status = null)
        {
            var dalStatus = status.HasValue ? (DataAccessLayer.Enum.PurchaseOrderStatus?)status.Value : null;
            var purchaseOrders = await _repo.GetAllAsync(dealerId, dalStatus);
            return (true, null, purchaseOrders);
        }

        public async Task<(bool Success, string Error, PurchaseOrder Data)> CreateAsync(
            Guid dealerId, Guid productId, Guid requestedById, int quantity, decimal unitPrice, 
            string reason, string notes, DateTime? expectedDeliveryDate = null)
        {
            // Validation
            if (dealerId == Guid.Empty)
                return (false, "Dealer ID không hợp lệ", null);

            if (productId == Guid.Empty)
                return (false, "Product ID không hợp lệ", null);

            if (requestedById == Guid.Empty)
                return (false, "Requested By ID không hợp lệ", null);

            if (quantity <= 0)
                return (false, "Số lượng phải lớn hơn 0", null);

            if (unitPrice <= 0)
                return (false, "Đơn giá phải lớn hơn 0", null);

            if (string.IsNullOrWhiteSpace(reason))
                return (false, "Lý do đặt hàng không được để trống", null);

            // Kiểm tra sản phẩm có tồn tại không
            var (productExists, productErr, product) = await _productService.GetAsync(productId);
            if (!productExists)
                return (false, productErr ?? "Sản phẩm không tồn tại", null);

            // Tạo số đơn hàng
            var orderNumber = await _repo.GenerateOrderNumberAsync();

            // Chuyển ExpectedDeliveryDate sang UTC nếu có
            DateTime? expectedDeliveryUtc = null;
            if (expectedDeliveryDate.HasValue)
            {
                expectedDeliveryUtc = expectedDeliveryDate.Value.Kind switch
                {
                    DateTimeKind.Utc => expectedDeliveryDate.Value,
                    DateTimeKind.Local => expectedDeliveryDate.Value.ToUniversalTime(),
                    _ => DateTime.SpecifyKind(expectedDeliveryDate.Value, DateTimeKind.Local).ToUniversalTime()
                };
            }

            // Tạo đơn đặt hàng
            var purchaseOrder = new PurchaseOrder
            {
                DealerId = dealerId,
                ProductId = productId,
                RequestedById = requestedById,
                OrderNumber = orderNumber,
                RequestedQuantity = quantity,
                UnitPrice = unitPrice,
                TotalAmount = quantity * unitPrice,
                Reason = reason.Trim(),
                Notes = notes?.Trim() ?? string.Empty,
                RequestedDate = DateTime.UtcNow,
                ExpectedDeliveryDate = expectedDeliveryUtc,
                Status = (DataAccessLayer.Enum.PurchaseOrderStatus)PurchaseOrderStatus.Pending
            };

            try
            {
                var success = await _repo.CreateAsync(purchaseOrder);
                if (!success)
                    return (false, "Không thể tạo đơn đặt hàng - Repository CreateAsync failed", null);

                return (true, null, purchaseOrder);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi tạo đơn đặt hàng: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Error, PurchaseOrder Data)> ApproveAsync(
            Guid id, Guid approvedById, DateTime? expectedDeliveryDate = null, string notes = "")
        {
            var (exists, err, purchaseOrder) = await GetAsync(id);
            if (!exists)
                return (false, err, null);

            if (purchaseOrder.Status != DataAccessLayer.Enum.PurchaseOrderStatus.Pending)
                return (false, "Chỉ có thể duyệt đơn hàng đang chờ duyệt", null);

            if (approvedById == Guid.Empty)
                return (false, "Approved By ID không hợp lệ", null);

            purchaseOrder.Status = DataAccessLayer.Enum.PurchaseOrderStatus.Approved;
            purchaseOrder.ApprovedById = approvedById;
            purchaseOrder.ApprovedDate = DateTime.UtcNow;
            if (expectedDeliveryDate.HasValue)
            {
                purchaseOrder.ExpectedDeliveryDate = expectedDeliveryDate.Value.Kind switch
                {
                    DateTimeKind.Utc => expectedDeliveryDate.Value,
                    DateTimeKind.Local => expectedDeliveryDate.Value.ToUniversalTime(),
                    _ => DateTime.SpecifyKind(expectedDeliveryDate.Value, DateTimeKind.Local).ToUniversalTime()
                };
            }
            
            if (!string.IsNullOrWhiteSpace(notes))
            {
                purchaseOrder.Notes = string.IsNullOrWhiteSpace(purchaseOrder.Notes) 
                    ? notes.Trim() 
                    : $"{purchaseOrder.Notes}\n[Duyệt]: {notes.Trim()}";
            }

            var success = await _repo.UpdateAsync(purchaseOrder);
            if (!success)
                return (false, "Không thể duyệt đơn đặt hàng", null);

            return (true, null, purchaseOrder);
        }

        public async Task<(bool Success, string Error, PurchaseOrder Data)> RejectAsync(
            Guid id, Guid rejectedById, string rejectReason)
        {
            var (exists, err, purchaseOrder) = await GetAsync(id);
            if (!exists)
                return (false, err, null);

            if (purchaseOrder.Status != DataAccessLayer.Enum.PurchaseOrderStatus.Pending)
                return (false, "Chỉ có thể từ chối đơn hàng đang chờ duyệt", null);

            if (rejectedById == Guid.Empty)
                return (false, "Rejected By ID không hợp lệ", null);

            if (string.IsNullOrWhiteSpace(rejectReason))
                return (false, "Lý do từ chối không được để trống", null);

            purchaseOrder.Status = DataAccessLayer.Enum.PurchaseOrderStatus.Rejected;
            purchaseOrder.ApprovedById = rejectedById;
            purchaseOrder.ApprovedDate = DateTime.UtcNow;
            purchaseOrder.RejectReason = rejectReason.Trim();

            var success = await _repo.UpdateAsync(purchaseOrder);
            if (!success)
                return (false, "Không thể từ chối đơn đặt hàng", null);

            return (true, null, purchaseOrder);
        }

        public async Task<(bool Success, string Error, PurchaseOrder Data)> UpdateStatusAsync(
            Guid id, PurchaseOrderStatus status, DateTime? actualDeliveryDate = null)
        {
            var (exists, err, purchaseOrder) = await GetAsync(id);
            if (!exists)
                return (false, err, null);

            // Validation theo trạng thái
            switch (status)
            {
                case PurchaseOrderStatus.InTransit:
                    if (purchaseOrder.Status != DataAccessLayer.Enum.PurchaseOrderStatus.Approved)
                        return (false, "Chỉ có thể chuyển trạng thái 'Đang vận chuyển' từ trạng thái 'Đã duyệt'", null);
                    break;

                case PurchaseOrderStatus.Delivered:
                    if (purchaseOrder.Status != DataAccessLayer.Enum.PurchaseOrderStatus.InTransit)
                        return (false, "Chỉ có thể chuyển trạng thái 'Đã giao' từ trạng thái 'Đang vận chuyển'", null);
                    
                    if (actualDeliveryDate.HasValue)
                    {
                        purchaseOrder.ActualDeliveryDate = actualDeliveryDate.Value.Kind switch
                        {
                            DateTimeKind.Utc => actualDeliveryDate.Value,
                            DateTimeKind.Local => actualDeliveryDate.Value.ToUniversalTime(),
                            _ => DateTime.SpecifyKind(actualDeliveryDate.Value, DateTimeKind.Local).ToUniversalTime()
                        };
                    }
                    else
                    {
                        purchaseOrder.ActualDeliveryDate = DateTime.UtcNow;
                    }

                    // 🔥 CẬP NHẬT TỒN KHO ĐẠI LÝ KHI GIAO HÀNG
                    try
                    {
                        Console.WriteLine($"[PurchaseOrder Delivered] Updating inventory for Dealer={purchaseOrder.DealerId}, Product={purchaseOrder.ProductId}, Quantity={purchaseOrder.RequestedQuantity}");
                        
                        var inventory = await _dbContext.InventoryAllocation
                            .FirstOrDefaultAsync(i => i.DealerId == purchaseOrder.DealerId 
                                                   && i.ProductId == purchaseOrder.ProductId 
                                                   && i.IsActive);

                        if (inventory == null)
                        {
                            // Tạo mới InventoryAllocation nếu chưa có
                            inventory = new InventoryAllocation
                            {
                                Id = Guid.NewGuid(),
                                DealerId = purchaseOrder.DealerId,
                                ProductId = purchaseOrder.ProductId,
                                AllocatedQuantity = purchaseOrder.RequestedQuantity,
                                ReservedQuantity = 0,
                                AvailableQuantity = purchaseOrder.RequestedQuantity,
                                MinimumStock = 5,
                                MaximumStock = 50,
                                LastRestockDate = DateTime.UtcNow,
                                Status = "Active",
                                Priority = "Normal",
                                Notes = $"Tạo tự động từ PurchaseOrder #{purchaseOrder.OrderNumber}",
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };
                            await _dbContext.InventoryAllocation.AddAsync(inventory);
                            Console.WriteLine($"[PurchaseOrder Delivered] Created new InventoryAllocation: {inventory.Id}");
                        }
                        else
                        {
                            // Cập nhật tồn kho hiện có
                            inventory.AllocatedQuantity += purchaseOrder.RequestedQuantity;
                            inventory.AvailableQuantity += purchaseOrder.RequestedQuantity;
                            inventory.LastRestockDate = DateTime.UtcNow;
                            inventory.UpdatedAt = DateTime.UtcNow;
                            inventory.Notes = string.IsNullOrWhiteSpace(inventory.Notes)
                                ? $"Nhập hàng từ PurchaseOrder #{purchaseOrder.OrderNumber}"
                                : $"{inventory.Notes}\n[{DateTime.Now:dd/MM/yyyy HH:mm}] Nhập {purchaseOrder.RequestedQuantity} xe từ PO #{purchaseOrder.OrderNumber}";
                            _dbContext.InventoryAllocation.Update(inventory);
                            Console.WriteLine($"[PurchaseOrder Delivered] Updated InventoryAllocation: AllocatedQty={inventory.AllocatedQuantity}, AvailableQty={inventory.AvailableQuantity}");
                        }

                        await _dbContext.SaveChangesAsync();
                        Console.WriteLine($"[PurchaseOrder Delivered] Inventory updated successfully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[PurchaseOrder Delivered] ERROR updating inventory: {ex.Message}");
                        return (false, $"Lỗi cập nhật tồn kho: {ex.Message}", null);
                    }
                    break;

                case PurchaseOrderStatus.Cancelled:
                    if (purchaseOrder.Status == DataAccessLayer.Enum.PurchaseOrderStatus.Delivered)
                        return (false, "Không thể hủy đơn hàng đã giao", null);
                    break;
            }

            purchaseOrder.Status = (DataAccessLayer.Enum.PurchaseOrderStatus)status;

            var success = await _repo.UpdateAsync(purchaseOrder);
            if (!success)
                return (false, "Không thể cập nhật trạng thái đơn đặt hàng", null);

            return (true, null, purchaseOrder);
        }

        public async Task<(bool Success, string Error, PurchaseOrder Data)> CancelAsync(Guid id)
        {
            return await UpdateStatusAsync(id, PurchaseOrderStatus.Cancelled);
        }
    }
}
