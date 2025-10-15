using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;
        private readonly AppDbContext _context;
        
        public CustomerService(ICustomerRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<(bool Success, string Error, Customer Data)> GetAsync(Guid id)
        {
            var c = await _repo.GetByIdAsync(id);
            return c == null ? (false, "Không tìm thấy", null) : (true, null, c);
        }

        public async Task<(bool Success, string Error, Customer Data)> UpdateProfileAsync(Customer updated)
        {
            if (updated == null || updated.Id == Guid.Empty) return (false, "Thiếu dữ liệu", null);
            updated.UpdatedAt = DateTime.UtcNow;
            var ok = await _repo.UpdateAsync(updated);
            return ok ? (true, null, updated) : (false, "Cập nhật thất bại", null);
        }

        public async Task<(bool Success, string Error, Customer Data)> CreateAsync(string fullName, string email, string phoneNumber, string address)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return (false, "Vui lòng nhập họ tên", null);
            if (string.IsNullOrWhiteSpace(phoneNumber)) return (false, "Vui lòng nhập số điện thoại", null);
            if (string.IsNullOrWhiteSpace(email)) return (false, "Vui lòng nhập email", null);

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Name = fullName, // Duplicate for backward compatibility
                Email = email,
                PhoneNumber = phoneNumber,
                Address = address ?? "",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var ok = await _repo.CreateAsync(customer);
            return ok ? (true, null, customer) : (false, "Không thể tạo khách hàng", null);
        }

        public async Task<(bool Success, string Error, List<Customer> Data)> GetAllByDealerAsync(Guid dealerId)
        {
            try
            {
                // Dealer có thể xem tất cả khách hàng trong hệ thống
                // Vì khách hàng mới tạo chưa có Order/TestDrive thì sẽ không hiển thị
                // Giải pháp: Hiển thị tất cả khách hàng active
                var customers = await _context.Customer
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                return (true, null, customers);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", new List<Customer>());
            }
        }

        public async Task<(bool Success, string Error, List<Customer> Data)> GetAllAsync()
        {
            try
            {
                var customers = await _context.Customer
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                return (true, null, customers);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", new List<Customer>());
            }
        }
    }
}
