using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class EVMRepository : IEVMRepository
    {
        private readonly AppDbContext _context;

        public EVMRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetSalesReportByRegionAsync(Guid? regionId = null, Guid? dealerId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null)
        {
            var query = _context.Order
                .Include(o => o.Region)
                .Include(o => o.Dealer)
                .Include(o => o.Product)
                .Include(o => o.Customer)
                .AsQueryable();

            // Apply filters
            if (regionId.HasValue)
                query = query.Where(o => o.RegionId == regionId.Value);

            if (dealerId.HasValue)
                query = query.Where(o => o.DealerId == dealerId.Value);

            // Apply date filters based on period
            if (year == 0) year = DateTime.Now.Year;

            switch (period?.ToLower())
            {
                case "monthly":
                    if (month.HasValue)
                        query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year && o.OrderDate.Value.Month == month.Value);
                    else
                        query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year);
                    break;
                case "quarterly":
                    if (quarter.HasValue)
                    {
                        var startMonth = (quarter.Value - 1) * 3 + 1;
                        var endMonth = quarter.Value * 3;
                        query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year && 
                                               o.OrderDate.Value.Month >= startMonth && o.OrderDate.Value.Month <= endMonth);
                    }
                    break;
                case "yearly":
                    query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year);
                    break;
            }

            return await query.ToListAsync();
        }

        public async Task<decimal> GetTotalSalesAsync(Guid? regionId = null, Guid? dealerId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null)
        {
            var orders = await GetSalesReportByRegionAsync(regionId, dealerId, period, year, month, quarter);
            return orders.Sum(o => o.FinalAmount);
        }

        public async Task<List<Product>> GetInventoryReportAsync(Guid? brandId = null, string priority = null)
        {
            var query = _context.Product
                .Include(p => p.Brand)
                .AsQueryable();

            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == brandId.Value);

            return await query.ToListAsync();
        }

        public async Task<List<Product>> GetLowStockProductsAsync()
        {
            return await _context.Product
                .Include(p => p.Brand)
                .Where(p => p.StockQuantity <= 10) // Assuming low stock threshold is 10
                .ToListAsync();
        }

        public async Task<List<Product>> GetCriticalStockProductsAsync()
        {
            return await _context.Product
                .Include(p => p.Brand)
                .Where(p => p.StockQuantity <= 5) // Assuming critical stock threshold is 5
                .ToListAsync();
        }

        public async Task<List<Product>> GetDemandForecastAsync(int forecastPeriod = 6, Guid? productId = null, string priority = null)
        {
            var query = _context.Product
                .Include(p => p.Brand)
                .AsQueryable();

            if (productId.HasValue)
                query = query.Where(p => p.Id == productId.Value);

            return await query.ToListAsync();
        }

        public async Task<List<Product>> GetHighPriorityForecastsAsync()
        {
            return await _context.Product
                .Include(p => p.Brand)
                .Where(p => p.StockQuantity <= 20) // High priority based on stock level
                .ToListAsync();
        }

        public async Task<List<DealerContract>> GetContractManagementReportAsync(Guid? dealerId = null, string status = null, string riskLevel = null)
        {
            var query = _context.DealerContract
                .Include(dc => dc.Dealer)
                .ThenInclude(d => d.Region)
                .AsQueryable();

            if (dealerId.HasValue)
                query = query.Where(dc => dc.DealerId == dealerId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(dc => dc.Status == status);

            return await query.ToListAsync();
        }

        public async Task<List<DealerContract>> GetExpiringContractsAsync(int daysAhead = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(daysAhead);
            return await _context.DealerContract
                .Include(dc => dc.Dealer)
                .ThenInclude(d => d.Region)
                .Where(dc => dc.EndDate <= cutoffDate && dc.Status == "Active")
                .ToListAsync();
        }

        public async Task<List<DealerContract>> GetHighRiskContractsAsync()
        {
            return await _context.DealerContract
                .Include(dc => dc.Dealer)
                .ThenInclude(d => d.Region)
                .Where(dc => dc.OutstandingDebt > dc.CreditLimit * 0.8m) // High risk if debt > 80% of credit limit
                .ToListAsync();
        }

        public async Task<List<Region>> GetAllRegionsAsync()
        {
            return await _context.Region.Where(r => r.IsActive).ToListAsync();
        }

        public async Task<List<Dealer>> GetAllDealersAsync()
        {
            return await _context.Dealer
                .Include(d => d.Region)
                .Where(d => d.IsActive)
                .ToListAsync();
        }

        public async Task<List<Brand>> GetAllBrandsAsync()
        {
            return await _context.Brand.Where(b => b.IsActive).ToListAsync();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Product
                .Include(p => p.Brand)
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<List<Order>> GetSalesReportByStaffAsync(Guid? salesPersonId = null, Guid? dealerId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null)
        {
            var query = _context.Order
                .Include(o => o.SalesPerson)
                .Include(o => o.Dealer)
                .Include(o => o.Product)
                .Include(o => o.Customer)
                .AsQueryable();

            // Apply filters
            if (salesPersonId.HasValue)
                query = query.Where(o => o.SalesPersonId == salesPersonId.Value);

            if (dealerId.HasValue)
                query = query.Where(o => o.DealerId == dealerId.Value);

            // Apply date filters based on period
            if (year == 0) year = DateTime.Now.Year;

            switch (period?.ToLower())
            {
                case "monthly":
                    if (month.HasValue)
                        query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year && o.OrderDate.Value.Month == month.Value);
                    else
                        query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year);
                    break;
                case "quarterly":
                    if (quarter.HasValue)
                    {
                        var startMonth = (quarter.Value - 1) * 3 + 1;
                        var endMonth = quarter.Value * 3;
                        query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year && 
                                               o.OrderDate.Value.Month >= startMonth && o.OrderDate.Value.Month <= endMonth);
                    }
                    break;
                case "yearly":
                    query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year);
                    break;
            }

            return await query.ToListAsync();
        }

        public async Task<List<Users>> GetAllSalesStaffAsync()
        {
            return await _context.Users
                .Where(u => u.Role == DataAccessLayer.Enum.UserRole.DealerStaff || u.Role == DataAccessLayer.Enum.UserRole.DealerManager)
                .Where(u => u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<List<Order>> GetCustomerDebtReportAsync(Guid? customerId = null, string paymentStatus = null)
        {
            var query = _context.Order
                .Include(o => o.Customer)
                .Include(o => o.Dealer)
                .Include(o => o.Product)
                .Where(o => o.PaymentStatus != "Paid") // Chỉ lấy các đơn chưa thanh toán hết
                .AsQueryable();

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(o => o.PaymentStatus == paymentStatus);

            return await query
                .OrderBy(o => o.PaymentDueDate)
                .ToListAsync();
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customer
                .Where(c => c.IsActive)
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<List<Users>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Dealer)
                .OrderBy(u => u.Role)
                .ThenBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<List<Users>> GetUsersByDealerAsync(Guid dealerId)
        {
            return await _context.Users
                .Include(u => u.Dealer)
                .Where(u => u.DealerId == dealerId && u.Role == UserRole.DealerStaff)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<Users> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Dealer)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> UpdateUserAsync(Users user)
        {
            try
            {
                _context.Users.Update(user);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                _context.Users.Remove(user);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
