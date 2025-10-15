using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository;

namespace BusinessLayer.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        public ProductService(IProductRepository repo) => _repo = repo;

        public async Task<(bool Success, string Error, Product Data)> GetAsync(Guid id)
        {
            var p = await _repo.GetByIdAsync(id);
            return p == null ? (false, "Không tìm thấy", null) : (true, null, p);
        }

        public Task<(bool Success, string Error, List<Product> Data)> SearchAsync(string? q, Guid? brandId, decimal? minPrice, decimal? maxPrice, bool? inStock, bool? isActive = true)
            => Execute(q, brandId, minPrice, maxPrice, inStock, isActive);

        private async Task<(bool Success, string Error, List<Product> Data)> Execute(string? q = null, Guid? brandId = null, decimal? minPrice = null, decimal? maxPrice = null, bool? inStock = null, bool? isActive = true)
        {
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                return (false, "Khoảng giá không hợp lệ", null);
            var list = await _repo.SearchAsync(q, brandId, minPrice, maxPrice, inStock, isActive);
            return (true, null, list);
        }

        public async Task<(bool Success, string Error)> UpdateStockAsync(Guid id, int newStockQuantity)
        {
            if (newStockQuantity < 0)
                return (false, "Số lượng tồn kho không hợp lệ");

            var product = await _repo.GetByIdAsync(id);
            if (product == null)
                return (false, "Không tìm thấy sản phẩm");

            var success = await _repo.UpdateStockAsync(id, newStockQuantity);
            if (!success)
                return (false, "Không thể cập nhật tồn kho");

            return (true, null);
        }

        public async Task<(bool Success, string Error)> CreateAsync(Product product)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(product.Sku))
                return (false, "SKU không được để trống");
            
            if (string.IsNullOrWhiteSpace(product.Name))
                return (false, "Tên sản phẩm không được để trống");
            
            if (product.Price <= 0)
                return (false, "Giá sản phẩm phải lớn hơn 0");
            
            if (product.StockQuantity < 0)
                return (false, "Số lượng tồn kho không hợp lệ");

            // Check SKU uniqueness
            if (await IsSkuExistsAsync(product.Sku))
                return (false, "SKU đã tồn tại trong hệ thống");

            var success = await _repo.CreateAsync(product);
            if (!success)
                return (false, "Không thể tạo sản phẩm mới");

            return (true, null);
        }

        public async Task<(bool Success, string Error)> UpdateAsync(Product product)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(product.Sku))
                return (false, "SKU không được để trống");
            
            if (string.IsNullOrWhiteSpace(product.Name))
                return (false, "Tên sản phẩm không được để trống");
            
            if (product.Price <= 0)
                return (false, "Giá sản phẩm phải lớn hơn 0");
            
            if (product.StockQuantity < 0)
                return (false, "Số lượng tồn kho không hợp lệ");

            // Check if product exists
            var existingProduct = await _repo.GetByIdAsync(product.Id);
            if (existingProduct == null)
                return (false, "Không tìm thấy sản phẩm");

            // Check SKU uniqueness (exclude current product)
            if (await IsSkuExistsAsync(product.Sku, product.Id))
                return (false, "SKU đã tồn tại trong hệ thống");

            var success = await _repo.UpdateAsync(product);
            if (!success)
                return (false, "Không thể cập nhật sản phẩm");

            return (true, null);
        }

        public async Task<(bool Success, string Error)> DeleteAsync(Guid id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null)
                return (false, "Không tìm thấy sản phẩm");

            var success = await _repo.DeleteAsync(id);
            if (!success)
                return (false, "Không thể xóa sản phẩm");

            return (true, null);
        }

        public async Task<bool> IsSkuExistsAsync(string sku, Guid? excludeId = null)
        {
            return await _repo.ExistsBySkuAsync(sku, excludeId);
        }
    }
}
