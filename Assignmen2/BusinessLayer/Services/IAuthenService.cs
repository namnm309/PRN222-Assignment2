using System;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using BusinessLayer.Enums;

namespace BusinessLayer.Services
{
    public interface IAuthenService
    {
        Task<(bool Success, string Error, Users User)> LoginAsync(string email, string password);
        Task<(bool Success, string Error, Users User)> RegisterAsync(string fullName, string email, string password, string phoneNumber, string address, UserRole role, Guid? dealerId = null);
        Task<(Users User, string Error)> GetUserByEmailAsync(string email);
        
        // User Management
        Task<(bool Success, string Error, List<Users> Data)> GetAllUsersAsync();
        Task<(bool Success, string Error, Users Data)> GetUserByIdAsync(Guid id);
        Task<(bool Success, string Error)> UpdateUserAsync(Users user);
        Task<(bool Success, string Error)> UpdateUserWithBusinessRoleAsync(Users user, UserRole role);
        Task<(bool Success, string Error)> ToggleUserStatusAsync(Guid id, bool isActive);
        Task<(bool Success, string Error)> DeleteUserAsync(Guid id);
    }
}
