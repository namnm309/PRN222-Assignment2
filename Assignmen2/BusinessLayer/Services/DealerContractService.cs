using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository;

namespace BusinessLayer.Services
{
    public class DealerContractService : IDealerContractService
    {
        private readonly IDealerContractRepository _repo;
        private readonly IOrderRepository _orderRepo;

        public DealerContractService(IDealerContractRepository repo, IOrderRepository orderRepo)
        {
            _repo = repo;
            _orderRepo = orderRepo;
        }

        public async Task<(bool Success, string Error, DealerContract Data)> GetAsync(Guid id)
        {
            var contract = await _repo.GetByIdAsync(id);
            return contract == null ? (false, "Không tìm thấy hợp đồng", null) : (true, null, contract);
        }

        public async Task<(bool Success, string Error, List<DealerContract> Data)> GetAllAsync(Guid? dealerId = null)
        {
            var list = await _repo.GetAllAsync(dealerId);
            return (true, null, list);
        }

        public async Task<(bool Success, string Error, DealerContract Data)> CreateFromOrderAsync(
            Guid orderId, string contractNumber, string terms, string notes)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return (false, "Không tìm thấy đơn hàng", null);

            if (order.Status != "Delivered")
                return (false, "Chỉ có thể tạo hợp đồng khi đã giao xe", null);

            if (string.IsNullOrWhiteSpace(contractNumber))
                return (false, "Số hợp đồng không được để trống", null);

            var startDate = DateTime.UtcNow;
            var endDate = DateTime.UtcNow.AddYears(1); // Default 1 year contract

            var contract = new DealerContract
            {
                DealerId = order.DealerId,
                ContractNumber = contractNumber,
                StartDate = startDate,
                EndDate = endDate,
                CommissionRate = 5.0m, // Default 5%
                CreditLimit = 100000000, // Default 100M VND
                OutstandingDebt = 0, // Initial
                Status = "Active",
                Terms = terms ?? "",
                Notes = notes ?? "",
                RenewalDate = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var ok = await _repo.CreateAsync(contract);
            return ok ? (true, null, contract) : (false, "Không thể tạo hợp đồng", null);
        }
    }
}

