using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IBrandService
    {
        Task<(bool Success, string Error, List<Brand> Data)> GetAllAsync();
        Task<(bool Success, string Error, Brand Data)> GetByIdAsync(Guid id);
        Task<(bool Success, string Error, Brand Data)> CreateAsync(string name, string country, string description);
        Task<(bool Success, string Error)> UpdateAsync(Guid id, string name, string country, string description, bool isActive);
        Task<(bool Success, string Error)> DeleteAsync(Guid id);
    }
}


