using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public interface IDealerRepository
    {
        Task<List<Dealer>> GetAllAsync();
        Task<Dealer?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(Dealer dealer);
        Task<bool> UpdateAsync(Dealer dealer);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}


