using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class DealerContract : BaseEntity
    {
        public Guid DealerId { get; set; }
        public virtual Dealer Dealer { get; set; }
        
        public string ContractNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        
        public decimal CommissionRate { get; set; } // Tỷ lệ hoa hồng
        public decimal CreditLimit { get; set; } // Hạn mức tín dụng
        public decimal OutstandingDebt { get; set; } // Công nợ hiện tại
        
        public string Status { get; set; } = "Active"; // Active, Suspended, Terminated
        public string Terms { get; set; } // Điều khoản hợp đồng
        public string Notes { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
