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
                user.UpdatedAt = DateTime.UtcNow;
                _dbContext.Users.Update(user);
                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
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
