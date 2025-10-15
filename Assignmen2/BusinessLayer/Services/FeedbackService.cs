using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository;

namespace BusinessLayer.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _repo;
        public FeedbackService(IFeedbackRepository repo) => _repo = repo;

        public async Task<(bool Success, string Error, Feedback Data)> CreateAsync(Guid customerId, Guid productId, string comment, int rating)
        {
            if (customerId == Guid.Empty || productId == Guid.Empty)
                return (false, "Thiếu thông tin", null);

            if (string.IsNullOrWhiteSpace(comment) || comment.Length < 5)
                return (false, "Nội dung quá ngắn", null);

            if (rating < 0 || rating > 5)
                return (false, "Điểm đánh giá phải từ 0 đến 5", null);

            var fb = new Feedback
            {
                CustomerId = customerId,
                ProductId = productId,
                Comment = comment.Trim(),
                Rating = rating,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var ok = await _repo.CreateAsync(fb);
            return ok ? (true, null, fb) : (false, "Không lưu được feedback", null);
        }

        public async Task<(bool Success, string Error, List<Feedback> Data)> GetByProductAsync(Guid productId)
        {
            var list = await _repo.GetByProductAsync(productId);
            return (true, null, list);
        }

        public async Task<(bool Success, string Error, List<Feedback> Data)> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return (true, null, list);
        }

        public async Task<(bool Success, string Error, Feedback Data)> GetByIdAsync(Guid id)
        {
            var feedback = await _repo.GetByIdAsync(id);
            return feedback != null ? (true, null, feedback) : (false, "Không tìm thấy phản hồi", null);
        }

        public async Task<(bool Success, string Error)> ReplyToFeedbackAsync(Guid feedbackId, string replyMessage, Guid repliedByUserId)
        {
            if (string.IsNullOrWhiteSpace(replyMessage))
                return (false, "Nội dung trả lời không được để trống");

            var success = await _repo.ReplyToFeedbackAsync(feedbackId, replyMessage, repliedByUserId);
            return success ? (true, null) : (false, "Không thể trả lời phản hồi");
        }

        public async Task<(bool Success, string Error)> DeleteAsync(Guid id)
        {
            var ok = await _repo.DeleteAsync(id);
            return ok ? (true, null) : (false, "Không xóa được feedback");
        }
    }
}
