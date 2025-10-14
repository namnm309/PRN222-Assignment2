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
    }
}
