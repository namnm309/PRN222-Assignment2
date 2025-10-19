using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTOs.Responses;

namespace BusinessLayer.Services
{
    public interface ITestDriveService
    {
        Task<(bool Success, string Error, TestDriveResponse Data)> CreateAsync(Guid customerId, Guid productId, Guid dealerId, DateTime scheduledDate);
        Task<(bool Success, string Error, TestDriveResponse Data)> CreatePublicAsync(string customerName, string customerPhone, string customerEmail, string? notes, Guid productId, Guid dealerId, DateTime scheduledDate);
        Task<(bool Success, string Error, TestDriveResponse Data)> GetAsync(Guid id);
        Task<(bool Success, string Error, List<TestDriveResponse> Data)> GetAllAsync(Guid? dealerId = null, string? status = null);
        Task<(bool Success, string Error, List<TestDriveResponse> Data)> GetByCustomerAsync(Guid customerId);
        Task<(bool Success, string Error, List<TestDriveResponse> Data)> GetByDealerAsync(Guid dealerId);
        Task<(bool Success, string Error, TestDriveResponse Data)> ConfirmAsync(Guid id);
        Task<(bool Success, string Error, TestDriveResponse Data)> CompleteAsync(Guid id, bool success);
        Task<(bool Success, string Error, TestDriveResponse Data)> CancelAsync(Guid id);
        Task<(bool Success, string Error)> UpdateCustomerAsync(Guid testDriveId, Guid customerId);
        Task<(bool Success, string Error, List<DateTime> Data)> GetScheduledInRangeAsync(Guid dealerId, Guid productId, DateTime from, DateTime to);
    }
}
