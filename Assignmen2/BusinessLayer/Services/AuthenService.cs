using System;
using System.Threading.Tasks;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using BusinessLayer.Enums;
using DataAccessLayer.Repository;

namespace BusinessLayer.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly IAuthen _authRepository;

        public AuthenService(IAuthen authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<(bool Success, string Error, Users User)> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return (false, "Email và mật khẩu là bắt buộc", null);
            }

            var user = await _authRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return (false, "Email không tồn tại", null);
            }

            var ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!ok)
            {
                return (false, "Mật khẩu không đúng", null);
            }

            if (!user.IsActive)
            {
                return (false, "Tài khoản đã bị khóa", null);
            }

            return (true, null, user);
        }

        public async Task<(bool Success, string Error, Users User)> RegisterAsync(
            string fullName, string email, string password, string phoneNumber, 
            string address, UserRole role, Guid? dealerId = null)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return (false, "Họ tên, email và mật khẩu là bắt buộc", null);
            }

            // Kiểm tra email đã tồn tại chưa
            var existingUser = await _authRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                return (false, "Email này đã được sử dụng", null);
            }

            // Validate dealerId cho Dealer Manager và Dealer Staff
            if (role == UserRole.DealerManager || role == UserRole.DealerStaff)
            {
                if (!dealerId.HasValue || dealerId.Value == Guid.Empty)
                {
                    return (false, "Dealer Manager và Dealer Staff phải được gán vào một đại lý", null);
                }
            }

            var user = new Users
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                PhoneNumber = phoneNumber,
                Address = address,
                Role = (DataAccessLayer.Enum.UserRole)role,
                DealerId = dealerId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var success = await _authRepository.CreateAsync(user);
            if (!success)
            {
                return (false, "Không thể tạo tài khoản", null);
            }

            return (true, null, user);
        }

        public async Task<(Users User, string Error)> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return (null, "Email không hợp lệ");
            }

            var user = await _authRepository.GetByEmailAsync(email);
            return (user, null);
        }

    }
}
