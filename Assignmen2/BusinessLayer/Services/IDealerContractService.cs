using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services
{
    public interface IDealerContractService
    {
        Task<(bool Success, string Error, DealerContract Data)> GetAsync(Guid id);
        Task<(bool Success, string Error, List<DealerContract> Data)> GetAllAsync(Guid? dealerId = null);
        Task<(bool Success, string Error, DealerContract Data)> CreateFromOrderAsync(
            Guid orderId, string contractNumber, string terms, string notes);
        
        // Contract Management
        Task<(bool Success, string Error, List<DealerContract> Data)> GetAllContractsAsync(Guid? dealerId = null, string status = null);
        Task<(bool Success, string Error)> UpdateContractStatusAsync(Guid contractId, string status);
    }
}

