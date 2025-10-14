using DataAccessLayer.Entities;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class DealerService : IDealerService
    {
        private readonly IDealerRepository _repository;

        public DealerService(IDealerRepository repository)
        {
            _repository = repository;
        }

        public async Task<(bool Success, string Error, List<Dealer> Data)> GetAllAsync()
        {
            var dealers = await _repository.GetAllAsync();
            return (true, null, dealers);
        }

        public async Task<(bool Success, string Error, Dealer Data)> GetByIdAsync(Guid id)
        {
            var dealer = await _repository.GetByIdAsync(id);
            if (dealer == null)
                return (false, "Không tìm thấy đại lý", null);
            return (true, null, dealer);
        }

        public async Task<(bool Success, string Error, Dealer Data)> CreateAsync(
            string name, string phone, string address, string city, string province,
            Guid? regionId, string dealerCode, string contactPerson, string email,
            string licenseNumber, decimal creditLimit)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (false, "Tên đại lý không được để trống", null);

            var dealer = new Dealer
            {
                Name = name,
                Phone = phone ?? "",
                Address = address ?? "",
                City = city ?? "",
                Province = province ?? "",
                RegionId = regionId,
                DealerCode = dealerCode ?? "",
                ContactPerson = contactPerson ?? "",
                Email = email ?? "",
                LicenseNumber = licenseNumber ?? "",
                CreditLimit = creditLimit,
                OutstandingDebt = 0,
                Status = "Active",
                IsActive = true
            };

            var success = await _repository.CreateAsync(dealer);
            if (!success)
                return (false, "Không thể tạo đại lý", null);

            return (true, null, dealer);
        }

        public async Task<(bool Success, string Error)> UpdateAsync(
            Guid id, string name, string phone, string address, string city, string province,
            Guid? regionId, string dealerCode, string contactPerson, string email,
            string licenseNumber, decimal creditLimit, decimal outstandingDebt, string status, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (false, "Tên đại lý không được để trống");

            var dealer = await _repository.GetByIdAsync(id);
            if (dealer == null)
                return (false, "Không tìm thấy đại lý");

            dealer.Name = name;
            dealer.Phone = phone ?? "";
            dealer.Address = address ?? "";
            dealer.City = city ?? "";
            dealer.Province = province ?? "";
            dealer.RegionId = regionId;
            dealer.DealerCode = dealerCode ?? "";
            dealer.ContactPerson = contactPerson ?? "";
            dealer.Email = email ?? "";
            dealer.LicenseNumber = licenseNumber ?? "";
            dealer.CreditLimit = creditLimit;
            dealer.OutstandingDebt = outstandingDebt;
            dealer.Status = status ?? "Active";
            dealer.IsActive = isActive;

            var success = await _repository.UpdateAsync(dealer);
            if (!success)
                return (false, "Không thể cập nhật đại lý");

            return (true, null);
        }

        public async Task<(bool Success, string Error)> DeleteAsync(Guid id)
        {
            var exists = await _repository.ExistsAsync(id);
            if (!exists)
                return (false, "Không tìm thấy đại lý");

            var success = await _repository.DeleteAsync(id);
            if (!success)
                return (false, "Không thể xóa đại lý");

            return (true, null);
        }
    }
}

