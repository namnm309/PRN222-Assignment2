using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Repository
{
    public interface IFeedbackRepository
    {
        Task<Feedback?> GetByIdAsync(Guid id);
        Task<List<Feedback>> GetByProductAsync(Guid productId);
        Task<List<Feedback>> GetByCustomerAsync(Guid customerId);
        Task<List<Feedback>> GetAllAsync();
        Task<bool> CreateAsync(Feedback feedback);
        Task<bool> ReplyToFeedbackAsync(Guid feedbackId, string replyMessage, Guid repliedByUserId);
        Task<bool> DeleteAsync(Guid id);
    }
}
