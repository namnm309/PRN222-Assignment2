using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class DealerRepository : IDealerRepository
    {
        private readonly AppDbContext _context;

        public DealerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Dealer>> GetAllAsync()
        {
            return await _context.Dealer
                .Include(d => d.Region)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<Dealer?> GetByIdAsync(Guid id)
        {
            return await _context.Dealer
                .Include(d => d.Region)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> CreateAsync(Dealer dealer)
        {
            try
            {
                dealer.Id = Guid.NewGuid();
                dealer.CreatedAt = DateTime.UtcNow;
                dealer.UpdatedAt = DateTime.UtcNow;
                await _context.Dealer.AddAsync(dealer);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Dealer dealer)
        {
            try
            {
                dealer.UpdatedAt = DateTime.UtcNow;
                _context.Dealer.Update(dealer);
                return await _context.SaveChangesAsync() > 0;
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
                var dealer = await GetByIdAsync(id);
                if (dealer == null) return false;
                
                _context.Dealer.Remove(dealer);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Dealer.AnyAsync(d => d.Id == id);
        }
    }
}

