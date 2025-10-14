using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class InventoryAllocation : BaseEntity
    {
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public Guid DealerId { get; set; }
        public virtual Dealer Dealer { get; set; }

        public int AllocatedQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }

        public DateTime LastRestockDate { get; set; }
        public DateTime? NextRestockDate { get; set; }
        
        public string Status { get; set; } = "Active"; // Active, Suspended, OutOfStock
        public string Priority { get; set; } = "Normal"; // High, Normal, Low
        public string Notes { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
