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

        public async Task<(bool Success, string Error, List<Users> Data)> GetAllUsersAsync()
        {
            try
            {
                var users = await _authRepository.GetAllAsync();
                return (true, null, users);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi tải danh sách người dùng: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Error, Users Data)> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _authRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return (false, "Không tìm thấy người dùng", null);
                }
                return (true, null, user);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi tải thông tin người dùng: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Error)> ToggleUserStatusAsync(Guid id, bool isActive)
        {
            try
            {
                var user = await _authRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return (false, "Không tìm thấy người dùng");
                }

                user.IsActive = isActive;
                var success = await _authRepository.UpdateAsync(user);
                
                if (!success)
                {
                    return (false, "Không thể cập nhật trạng thái người dùng");
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi cập nhật trạng thái: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Error)> UpdateUserAsync(Users user)
        {
            try
            {
                var success = await _authRepository.UpdateAsync(user);
                
                if (!success)
                {
                    return (false, "Không thể cập nhật thông tin người dùng");
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi cập nhật người dùng: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Error)> UpdateUserWithBusinessRoleAsync(Users user, UserRole role)
        {
            try
            {
                // Convert BusinessLayer enum to DataAccessLayer enum
                user.Role = (DataAccessLayer.Enum.UserRole)role;
                
                var success = await _authRepository.UpdateAsync(user);
                
                if (!success)
                {
                    return (false, "Không thể cập nhật thông tin người dùng");
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi cập nhật người dùng: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Error)> DeleteUserAsync(Guid id)
        {
            try
            {
                var user = await _authRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return (false, "Không tìm thấy người dùng");
                }

                var success = await _authRepository.DeleteAsync(id);
                
                if (!success)
                {
                    return (false, "Không thể xóa người dùng");
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi xóa người dùng: {ex.Message}");
            }
        }

    }
}
