using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;

        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _context.Brand
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<Brand?> GetByIdAsync(Guid id)
        {
            return await _context.Brand.FindAsync(id);
        }

        public async Task<bool> CreateAsync(Brand brand)
        {
            try
            {
                brand.Id = Guid.NewGuid();
                brand.CreatedAt = DateTime.UtcNow;
                brand.UpdatedAt = DateTime.UtcNow;
                await _context.Brand.AddAsync(brand);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Brand brand)
        {
            try
            {
                _context.Brand.Update(brand);
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
                var brand = await GetByIdAsync(id);
                if (brand == null) return false;
                
                _context.Brand.Remove(brand);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Brand.AnyAsync(b => b.Id == id);
        }
    }
}


