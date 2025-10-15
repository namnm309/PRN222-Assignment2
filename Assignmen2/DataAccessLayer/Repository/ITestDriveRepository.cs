using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Repository
{
    public interface ITestDriveRepository
    {
        Task<TestDrive?> GetByIdAsync(Guid id);
        Task<List<TestDrive>> GetAllAsync(Guid? dealerId = null, string? status = null);
        Task<List<TestDrive>> GetByCustomerAsync(Guid customerId);
        Task<List<TestDrive>> GetByDealerAsync(Guid dealerId);
        Task<List<TestDrive>> GetByDealerAndProductInRangeAsync(Guid dealerId, Guid productId, DateTime from, DateTime to);
        Task<bool> CreateAsync(TestDrive td);
        Task<bool> UpdateAsync(TestDrive td);
    }
}
