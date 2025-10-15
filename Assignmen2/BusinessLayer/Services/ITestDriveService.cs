using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services
{
    public interface ITestDriveService
    {
        Task<(bool Success, string Error, TestDrive Data)> CreateAsync(Guid customerId, Guid productId, Guid dealerId, DateTime scheduledDate);
        Task<(bool Success, string Error, TestDrive Data)> CreatePublicAsync(string customerName, string customerPhone, string customerEmail, string? notes, Guid productId, Guid dealerId, DateTime scheduledDate);
        Task<(bool Success, string Error, TestDrive Data)> GetAsync(Guid id);
        Task<(bool Success, string Error, List<TestDrive> Data)> GetAllAsync(Guid? dealerId = null, string? status = null);
        Task<(bool Success, string Error, List<TestDrive> Data)> GetByCustomerAsync(Guid customerId);
        Task<(bool Success, string Error, List<TestDrive> Data)> GetByDealerAsync(Guid dealerId);
        Task<(bool Success, string Error, TestDrive Data)> ConfirmAsync(Guid id);
        Task<(bool Success, string Error, TestDrive Data)> CompleteAsync(Guid id, bool success);
        Task<(bool Success, string Error, TestDrive Data)> CancelAsync(Guid id);
    }
}
