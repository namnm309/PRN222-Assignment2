using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class InventoryTransaction : BaseEntity
    {
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public Guid? DealerId { get; set; }
        public virtual Dealer Dealer { get; set; }

        public Guid? OrderId { get; set; }
        public virtual Order Order { get; set; }

        public string TransactionType { get; set; } // IN, OUT, TRANSFER, ADJUSTMENT
        public int Quantity { get; set; }
        public int QuantityBefore { get; set; }
        public int QuantityAfter { get; set; }
        
        public string Reason { get; set; } // Sale, Purchase, Transfer, Damage, Return
        public string ReferenceNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        
        public Guid? ProcessedByUserId { get; set; }
        public virtual Users ProcessedByUser { get; set; }
        
        public string Status { get; set; } = "Completed"; // Pending, Completed, Cancelled
        public string Notes { get; set; }
    }
}
