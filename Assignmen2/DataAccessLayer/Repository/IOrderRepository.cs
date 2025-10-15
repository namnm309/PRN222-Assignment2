namespace DataAccessLayer.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccessLayer.Entities;

    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task<List<Order>> GetAllAsync(Guid? dealerId = null, string? status = null);
        Task<bool> CreateAsync(Order order);
        Task<bool> UpdateAsync(Order order);
    }
}

