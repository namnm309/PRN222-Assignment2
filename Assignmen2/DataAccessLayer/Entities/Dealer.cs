using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Dealer : BaseEntity
    {
        //Đại lý
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public Guid? RegionId { get; set; }
        public virtual Region Region { get; set; }
        
        public string DealerCode { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string LicenseNumber { get; set; }
        
        public decimal CreditLimit { get; set; }
        public decimal OutstandingDebt { get; set; }
        public string Status { get; set; } = "Active"; // Active, Suspended, Inactive
        
        public bool IsActive { get; set; } = true;
    }
}
