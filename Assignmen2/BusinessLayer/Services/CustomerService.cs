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

        public async Task<(bool Success, string Error)> UpdateAsync(Guid id, string fullName, string email, string phoneNumber, string address)
        {
            try
            {
                var (ok, err, customer) = await GetAsync(id);
                if (!ok || customer == null)
                {
                    return (false, err ?? "Không tìm thấy khách hàng");
                }

                customer.FullName = fullName;
                customer.Name = fullName; // Duplicate for backward compatibility
                customer.Email = email;
                customer.PhoneNumber = phoneNumber;
                customer.Address = address;
                customer.UpdatedAt = DateTime.UtcNow;

                var success = await _repo.UpdateAsync(customer);
                return success ? (true, null) : (false, "Không thể cập nhật khách hàng");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Error, Customer Data)> GetByEmailAsync(string email)
        {
            try
            {
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.Email == email && c.IsActive);
                return customer == null ? (false, "Không tìm thấy", null) : (true, null, customer);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Error, Customer Data)> GetByPhoneAsync(string phoneNumber)
        {
            try
            {
                var customer = await _context.Customer
                    .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber && c.IsActive);
                return customer == null ? (false, "Không tìm thấy", null) : (true, null, customer);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
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

        public async Task<(bool Success, string Error)> UpdateStatusAsync(Guid id, bool isActive)
        {
            try
            {
                var customer = await _repo.GetByIdAsync(id);
                if (customer == null)
                    return (false, "Không tìm thấy khách hàng");

                customer.IsActive = isActive;
                customer.UpdatedAt = DateTime.UtcNow;

                var result = await _repo.UpdateAsync(customer);
                return result ? (true, null) : (false, "Không thể cập nhật trạng thái khách hàng");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }
    }
}
