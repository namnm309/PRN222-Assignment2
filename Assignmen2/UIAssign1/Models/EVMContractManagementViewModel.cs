using System;
using System.Collections.Generic;

namespace PresentationLayer.Models
{
    public class EVMContractManagementViewModel
    {
        public string ContractNumber { get; set; }
        public string DealerName { get; set; }
        public string DealerCode { get; set; }
        public string RegionName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal OutstandingDebt { get; set; }
        public decimal CreditUtilization { get; set; } // Tỷ lệ sử dụng hạn mức
        public decimal SalesTarget { get; set; }
        public decimal ActualSales { get; set; }
        public decimal AchievementRate { get; set; }
        public int DaysToExpiry { get; set; }
        public string RiskLevel { get; set; } // Low, Medium, High
        public DateTime LastUpdated { get; set; }
    }

    public class EVMContractFilterViewModel
    {
        public string DealerId { get; set; }
        public string RegionId { get; set; }
        public string Status { get; set; }
        public string RiskLevel { get; set; }
        public string Priority { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
    }
}
