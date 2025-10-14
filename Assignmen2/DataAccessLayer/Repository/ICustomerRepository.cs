using System;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Repository
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id);
        Task<bool> UpdateAsync(Customer entity);
        Task<bool> CreateAsync(Customer entity);
    }
}
