using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly AppDbContext _db;
        public FeedbackRepository(AppDbContext db) => _db = db;

        public Task<Feedback?> GetByIdAsync(Guid id)
            => _db.Feedback.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);

        public Task<List<Feedback>> GetByProductAsync(Guid productId)
            => _db.Feedback.Where(f => f.ProductId == productId)
                           .OrderByDescending(f => f.CreatedAt)
                           .AsNoTracking().ToListAsync();

        public Task<List<Feedback>> GetByCustomerAsync(Guid customerId)
            => _db.Feedback.Where(f => f.CustomerId == customerId)
                           .OrderByDescending(f => f.CreatedAt)
                           .AsNoTracking().ToListAsync();

        public Task<List<Feedback>> GetAllAsync()
            => _db.Feedback.Include(f => f.Customer)
                           .Include(f => f.Product)
                           .OrderByDescending(f => f.CreatedAt)
                           .AsNoTracking().ToListAsync();

        public async Task<bool> CreateAsync(Feedback feedback)
        {
            await _db.Feedback.AddAsync(feedback);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> ReplyToFeedbackAsync(Guid feedbackId, string replyMessage, Guid repliedByUserId)
        {
            try
            {
                var feedback = await _db.Feedback.FindAsync(feedbackId);
                if (feedback == null) return false;

                // Cập nhật thông tin trả lời (có thể thêm field ReplyMessage vào entity nếu cần)
                feedback.UpdatedAt = DateTime.UtcNow;
                
                return await _db.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var fb = await _db.Feedback.FindAsync(id);
            if (fb == null) return false;
            _db.Feedback.Remove(fb);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
