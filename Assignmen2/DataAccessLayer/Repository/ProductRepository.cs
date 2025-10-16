using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;
        public ProductRepository(AppDbContext db) => _db = db;

        public Task<Product?> GetByIdAsync(Guid id)
            => _db.Product.Include(p => p.Brand).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<Product>> SearchAsync(string? q, Guid? brandId, decimal? minPrice, decimal? maxPrice, bool? inStock, bool? isActive)
        {
            var query = _db.Product.Include(p => p.Brand).AsNoTracking().AsQueryable();

            // Apply database filters first (more efficient)
            if (brandId.HasValue) query = query.Where(x => x.BrandId == brandId.Value);
            if (minPrice.HasValue) query = query.Where(x => x.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(x => x.Price <= maxPrice.Value);
            if (inStock == true) query = query.Where(x => x.StockQuantity > 0);
            if (isActive.HasValue) query = query.Where(x => x.IsActive == isActive);

            // Load to memory
            var results = await query.ToListAsync();

            // Apply text search with diacritics support in memory
            if (!string.IsNullOrWhiteSpace(q))
            {
                var normalizedSearch = NormalizeForSearch(q);
                results = results.Where(x =>
                    NormalizeForSearch(x.Name).Contains(normalizedSearch) ||
                    NormalizeForSearch(x.Sku).Contains(normalizedSearch) ||
                    NormalizeForSearch(x.Description ?? "").Contains(normalizedSearch)
                ).ToList();
            }

            return results.OrderBy(x => x.Name).ToList();
        }

        private static string NormalizeForSearch(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var normalized = input.ToLowerInvariant();
            normalized = RemoveDiacritics(normalized);
            return normalized.Trim();
        }

        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public async Task<bool> UpdateStockAsync(Guid id, int newStockQuantity)
        {
            try
            {
                var product = await _db.Product.FindAsync(id);
                if (product == null) return false;

                product.StockQuantity = newStockQuantity;
                product.UpdatedAt = DateTime.UtcNow;

                return await _db.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateAsync(Product product)
        {
            try
            {
                product.Id = Guid.NewGuid();
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;

                await _db.Product.AddAsync(product);
                return await _db.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            try
            {
                var existingProduct = await _db.Product.FindAsync(product.Id);
                if (existingProduct == null) return false;

                existingProduct.Sku = product.Sku;
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.IsActive = product.IsActive;
                existingProduct.BrandId = product.BrandId;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                return await _db.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var product = await _db.Product.FindAsync(id);
                if (product == null) return false;

                _db.Product.Remove(product);
                return await _db.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExistsBySkuAsync(string sku, Guid? excludeId = null)
        {
            var query = _db.Product.Where(p => p.Sku == sku);
            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);
            return await query.AnyAsync();
        }
    }
}
