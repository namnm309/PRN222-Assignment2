using DataAccessLayer.Entities;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _repository;

        public BrandService(IBrandRepository repository)
        {
            _repository = repository;
        }

        public async Task<(bool Success, string Error, List<Brand> Data)> GetAllAsync()
        {
            var brands = await _repository.GetAllAsync();
            return (true, null, brands);
        }

        public async Task<(bool Success, string Error, Brand Data)> GetByIdAsync(Guid id)
        {
            var brand = await _repository.GetByIdAsync(id);
            if (brand == null)
                return (false, "Không tìm thấy thương hiệu", null);
            return (true, null, brand);
        }

        public async Task<(bool Success, string Error, Brand Data)> CreateAsync(string name, string country, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (false, "Tên thương hiệu không được để trống", null);

            var brand = new Brand
            {
                Name = name,
                Country = country ?? "",
                Description = description ?? "",
                IsActive = true
            };

            var success = await _repository.CreateAsync(brand);
            if (!success)
                return (false, "Không thể tạo thương hiệu", null);

            return (true, null, brand);
        }

        public async Task<(bool Success, string Error)> UpdateAsync(Guid id, string name, string country, string description, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (false, "Tên thương hiệu không được để trống");

            var brand = await _repository.GetByIdAsync(id);
            if (brand == null)
                return (false, "Không tìm thấy thương hiệu");

            // Update properties
            brand.Name = name;
            brand.Country = country ?? "";
            brand.Description = description ?? "";
            brand.IsActive = isActive;
            brand.UpdatedAt = DateTime.UtcNow;

            var success = await _repository.UpdateAsync(brand);
            if (!success)
                return (false, "Không thể cập nhật thương hiệu");

            return (true, null);
        }

        public async Task<(bool Success, string Error)> DeleteAsync(Guid id)
        {
            var exists = await _repository.ExistsAsync(id);
            if (!exists)
                return (false, "Không tìm thấy thương hiệu");

            var success = await _repository.DeleteAsync(id);
            if (!success)
                return (false, "Không thể xóa thương hiệu");

            return (true, null);
        }
    }
}


