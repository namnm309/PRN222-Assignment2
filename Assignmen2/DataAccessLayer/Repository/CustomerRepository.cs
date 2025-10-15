using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
namespace DataAccessLayer.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _db;
        public CustomerRepository(AppDbContext db) => _db = db;

        public Task<Customer?> GetByIdAsync(Guid id)
            => _db.Set<Customer>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<bool> UpdateAsync(Customer entity)
        {
            _db.Set<Customer>().Update(entity);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateAsync(Customer entity)
        {
            await _db.Set<Customer>().AddAsync(entity);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
