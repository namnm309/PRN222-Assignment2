using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class DealerContractRepository : IDealerContractRepository
    {
        private readonly AppDbContext _db;
        public DealerContractRepository(AppDbContext db) => _db = db;

        public Task<DealerContract?> GetByIdAsync(Guid id)
            => _db.DealerContract
                .Include(c => c.Dealer)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<List<DealerContract>> GetAllAsync(Guid? dealerId = null)
        {
            var query = _db.DealerContract
                .Include(c => c.Dealer)
                .AsQueryable();

            if (dealerId.HasValue)
                query = query.Where(c => c.DealerId == dealerId.Value);

            return await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(DealerContract contract)
        {
            await _db.DealerContract.AddAsync(contract);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(DealerContract contract)
        {
            _db.DealerContract.Update(contract);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}

