using System;

namespace BusinessLayer.ViewModels
{
    public class DealerContractViewModel
    {
        public Guid Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public Guid DealerId { get; set; }
        public Guid RegionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal CommissionRate { get; set; }
        public string Terms { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string DealerName { get; set; } = string.Empty;
        public string RegionName { get; set; } = string.Empty;
    }
}
