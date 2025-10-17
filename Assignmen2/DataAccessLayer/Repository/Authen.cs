using System;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class Authen : IAuthen
    {
        private readonly AppDbContext _dbContext;

        public Authen(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Users> GetByEmailAsync(string email)
        {
            var normalized = email?.Trim().ToLower();
            return await _dbContext.Users
                .Include(u => u.Dealer)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalized);
        }

        public async Task<bool> CreateAsync(Users user)
        {
            try
            {
                await _dbContext.Users.AddAsync(user);
                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        // Legacy signatures, kept for compatibility
        public bool Login(string username, string password)
        {
            var user = GetByEmailAsync(username).GetAwaiter().GetResult();
            if (user == null) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public async Task<List<Users>> GetAllAsync()
        {
            return await _dbContext.Users
                .Include(u => u.Dealer)
                .ToListAsync();
        }

        public async Task<Users> GetByIdAsync(Guid id)
        {
            return await _dbContext.Users
                .Include(u => u.Dealer)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> UpdateAsync(Users user)
        {
            try
            {
                var existingUser = await _dbContext.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    return false;
                }

                // Update only the fields that should be updated
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Role = user.Role;
                existingUser.IsActive = user.IsActive;
                existingUser.UpdatedAt = DateTime.UtcNow;

                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(id);
                if (user == null) return false;

                _dbContext.Users.Remove(user);
                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
