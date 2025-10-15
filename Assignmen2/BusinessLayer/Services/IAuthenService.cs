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
    }
}
