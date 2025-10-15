using DataAccessLayer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Users : BaseEntity
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public UserRole Role { get; set; } 

        public bool IsActive { get; set; } = true;

        // Dealer relationship for Dealer Staff/Manager
        public Guid? DealerId { get; set; }
        public virtual Dealer? Dealer { get; set; }

    }
}
