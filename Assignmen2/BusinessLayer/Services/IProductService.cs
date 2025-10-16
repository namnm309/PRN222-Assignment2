using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services
{
    public interface IProductService
    {
        Task<(bool Success, string Error, Product Data)> GetAsync(Guid id);
        Task<(bool Success, string Error, Product Data)> GetByIdAsync(Guid id); // Alias for GetAsync
        Task<(bool Success, string Error, List<Product> Data)> SearchAsync(string? q, Guid? brandId, decimal? minPrice, decimal? maxPrice, bool? inStock, bool? isActive = true);
        Task<(bool Success, string Error)> UpdateStockAsync(Guid id, int newStockQuantity);
        Task<(bool Success, string Error)> CreateAsync(Product product);
        Task<(bool Success, string Error)> UpdateAsync(Product product);
        Task<(bool Success, string Error)> DeleteAsync(Guid id);
        Task<bool> IsSkuExistsAsync(string sku, Guid? excludeId = null);
    }
}
