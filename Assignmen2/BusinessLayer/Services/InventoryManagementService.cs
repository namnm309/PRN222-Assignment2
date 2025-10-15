using DataAccessLayer.Repository;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTOs.Responses;
using BusinessLayer.DTOs.Requests;

namespace BusinessLayer.Services
{
    public class InventoryManagementService : IInventoryManagementService
    {
        private readonly IInventoryManagementRepository _inventoryRepository;
        private readonly IEVMRepository _evmRepository;
        private readonly IMappingService _mappingService;

        public InventoryManagementService(
            IInventoryManagementRepository inventoryRepository,
            IEVMRepository evmRepository,
            IMappingService mappingService)
        {
            _inventoryRepository = inventoryRepository;
            _evmRepository = evmRepository;
            _mappingService = mappingService;
        }

        public async Task<List<InventoryAllocationResponse>> GetAllInventoryAllocationsAsync()
        {
            var list = await _inventoryRepository.GetAllInventoryAllocationsAsync();
            return _mappingService.MapToInventoryAllocationViewModels(list);
        }

        public async Task<List<InventoryAllocationResponse>> GetInventoryAllocationsByDealerAsync(Guid dealerId)
        {
            var list = await _inventoryRepository.GetInventoryAllocationsByDealerAsync(dealerId);
            return _mappingService.MapToInventoryAllocationViewModels(list);
        }

        public async Task<List<InventoryAllocationResponse>> GetInventoryAllocationsByProductAsync(Guid productId)
        {
            var list = await _inventoryRepository.GetInventoryAllocationsByProductAsync(productId);
            return _mappingService.MapToInventoryAllocationViewModels(list);
        }

        public async Task<InventoryAllocationResponse?> GetInventoryAllocationAsync(Guid productId, Guid dealerId)
        {
            var entity = await _inventoryRepository.GetInventoryAllocationAsync(productId, dealerId);
            return entity == null ? null : _mappingService.MapToInventoryAllocationViewModel(entity);
        }

        public async Task<bool> CreateInventoryAllocationAsync(InventoryAllocationResponse allocation)
        {
            if (allocation.AllocatedQuantity < 0 || allocation.AvailableQuantity < 0)
                return false;

            if (allocation.MinimumStock < 0 || allocation.MaximumStock <= allocation.MinimumStock)
                return false;

            var entity = new InventoryAllocation
            {
                Id = allocation.Id,
                ProductId = allocation.ProductId,
                DealerId = allocation.DealerId,
                AllocatedQuantity = allocation.AllocatedQuantity,
                AvailableQuantity = allocation.AvailableQuantity,
                ReservedQuantity = allocation.ReservedQuantity,
                MinimumStock = allocation.MinimumStock,
                MaximumStock = allocation.MaximumStock,
                LastRestockDate = allocation.LastRestockDate == default ? DateTime.UtcNow : allocation.LastRestockDate,
                NextRestockDate = allocation.NextRestockDate,
                Status = allocation.Status,
                Priority = allocation.Priority,
                Notes = allocation.Notes ?? string.Empty,
                CreatedAt = allocation.CreatedAt,
                UpdatedAt = allocation.UpdatedAt ?? DateTime.UtcNow,
                IsActive = allocation.IsActive
            };
            return await _inventoryRepository.CreateInventoryAllocationAsync(entity);
        }

        public async Task<bool> UpdateInventoryAllocationAsync(InventoryAllocationResponse allocation)
        {
            if (allocation.AllocatedQuantity < 0 || allocation.AvailableQuantity < 0)
                return false;

            if (allocation.MinimumStock < 0 || allocation.MaximumStock <= allocation.MinimumStock)
                return false;

            var entity = new InventoryAllocation
            {
                Id = allocation.Id,
                ProductId = allocation.ProductId,
                DealerId = allocation.DealerId,
                AllocatedQuantity = allocation.AllocatedQuantity,
                AvailableQuantity = allocation.AvailableQuantity,
                ReservedQuantity = allocation.ReservedQuantity,
                MinimumStock = allocation.MinimumStock,
                MaximumStock = allocation.MaximumStock,
                LastRestockDate = allocation.LastRestockDate == default ? DateTime.UtcNow : allocation.LastRestockDate,
                NextRestockDate = allocation.NextRestockDate,
                Status = allocation.Status,
                Priority = allocation.Priority,
                Notes = allocation.Notes ?? string.Empty,
                CreatedAt = allocation.CreatedAt,
                UpdatedAt = allocation.UpdatedAt ?? DateTime.UtcNow,
                IsActive = allocation.IsActive
            };
            return await _inventoryRepository.UpdateInventoryAllocationAsync(entity);
        }

        public async Task<bool> DeleteInventoryAllocationAsync(Guid id)
        {
            return await _inventoryRepository.DeleteInventoryAllocationAsync(id);
        }

        public async Task<List<InventoryAllocationResponse>> GetLowStockAllocationsAsync()
        {
            var list = await _inventoryRepository.GetLowStockAllocationsAsync();
            return _mappingService.MapToInventoryAllocationViewModels(list);
        }

        public async Task<List<InventoryAllocationResponse>> GetCriticalStockAllocationsAsync()
        {
            var list = await _inventoryRepository.GetCriticalStockAllocationsAsync();
            return _mappingService.MapToInventoryAllocationViewModels(list);
        }

        public async Task<List<InventoryAllocationResponse>> GetOutOfStockAllocationsAsync()
        {
            var list = await _inventoryRepository.GetOutOfStockAllocationsAsync();
            return _mappingService.MapToInventoryAllocationViewModels(list);
        }

        public async Task<List<InventoryTransactionResponse>> GetInventoryTransactionsAsync(Guid? productId = null, Guid? dealerId = null, string transactionType = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var list = await _inventoryRepository.GetInventoryTransactionsAsync(productId, dealerId, transactionType, fromDate, toDate);
            return _mappingService.MapToInventoryTransactionViewModels(list);
        }

        public async Task<bool> CreateInventoryTransactionAsync(InventoryTransactionCreateRequest transaction)
        {
            var entity = new InventoryTransaction
            {
                ProductId = transaction.ProductId,
                DealerId = transaction.DealerId,
                Quantity = transaction.Quantity,
                TransactionType = transaction.TransactionType,
                Reason = transaction.Reason,
                ProcessedByUserId = transaction.ProcessedByUserId,
                TransactionDate = DateTime.UtcNow,
                Status = "Completed",
                Notes = string.Empty
            };
            return await _inventoryRepository.CreateInventoryTransactionAsync(entity);
        }

        public async Task<bool> TransferStockAsync(Guid productId, Guid fromDealerId, Guid toDealerId, int quantity, string reason, Guid processedByUserId)
        {
            if (quantity <= 0) return false;
            if (fromDealerId == toDealerId) return false;

            return await _inventoryRepository.TransferStockAsync(productId, fromDealerId, toDealerId, quantity, reason, processedByUserId);
        }

        public async Task<bool> AdjustStockAsync(Guid productId, Guid dealerId, int quantity, string reason, Guid processedByUserId)
        {
            if (quantity == 0) return false;

            return await _inventoryRepository.AdjustStockAsync(productId, dealerId, quantity, reason, processedByUserId);
        }

        public async Task<List<InventoryAllocationResponse>> GetInventoryReportAsync(Guid? dealerId = null, Guid? productId = null, string status = null)
        {
            var list = await _inventoryRepository.GetInventoryReportAsync(dealerId, productId, status);
            return _mappingService.MapToInventoryAllocationViewModels(list);
        }

        public async Task<Dictionary<string, int>> GetStockSummaryAsync()
        {
            return await _inventoryRepository.GetStockSummaryAsync();
        }

        public async Task<List<InventoryTransactionResponse>> GetStockMovementReportAsync(DateTime fromDate, DateTime toDate, Guid? productId = null, Guid? dealerId = null)
        {
            var list = await _inventoryRepository.GetStockMovementReportAsync(fromDate, toDate, productId, dealerId);
            return _mappingService.MapToInventoryTransactionViewModels(list);
        }

        public async Task<List<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _evmRepository.GetAllProductsAsync();
            return _mappingService.MapToProductViewModels(products);
        }

        public async Task<List<DealerResponse>> GetAllDealersAsync()
        {
            var dealers = await _evmRepository.GetAllDealersAsync();
            return _mappingService.MapToDealerViewModels(dealers);
        }
    }
}
