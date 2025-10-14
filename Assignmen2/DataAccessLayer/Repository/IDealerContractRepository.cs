using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Repository
{
    public interface IDealerContractRepository
    {
        Task<DealerContract?> GetByIdAsync(Guid id);
        Task<List<DealerContract>> GetAllAsync(Guid? dealerId = null);
        Task<bool> CreateAsync(DealerContract contract);
        Task<bool> UpdateAsync(DealerContract contract);
    }
}


