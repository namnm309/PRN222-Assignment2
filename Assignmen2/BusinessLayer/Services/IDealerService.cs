using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IDealerService
    {
        Task<(bool Success, string Error, List<Dealer> Data)> GetAllAsync();
        Task<(bool Success, string Error, Dealer Data)> GetByIdAsync(Guid id);
        Task<(bool Success, string Error, Dealer Data)> CreateAsync(
            string name, string phone, string address, string city, string province,
            Guid? regionId, string dealerCode, string contactPerson, string email,
            string licenseNumber, decimal creditLimit);
        Task<(bool Success, string Error)> UpdateAsync(
            Guid id, string name, string phone, string address, string city, string province,
            Guid? regionId, string dealerCode, string contactPerson, string email,
            string licenseNumber, decimal creditLimit, decimal outstandingDebt, string status, bool isActive);
        Task<(bool Success, string Error)> DeleteAsync(Guid id);
    }
}

