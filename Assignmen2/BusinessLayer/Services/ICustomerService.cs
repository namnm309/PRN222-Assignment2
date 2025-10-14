using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services
{
    public interface ICustomerService
    {
        Task<(bool Success, string Error, Customer Data)> GetAsync(Guid id);
        Task<(bool Success, string Error, Customer Data)> UpdateProfileAsync(Customer updated);
        Task<(bool Success, string Error, Customer Data)> CreateAsync(string fullName, string email, string phoneNumber, string address);
        Task<(bool Success, string Error, List<Customer> Data)> GetAllByDealerAsync(Guid dealerId);
        Task<(bool Success, string Error, List<Customer> Data)> GetAllAsync();
    }
}
