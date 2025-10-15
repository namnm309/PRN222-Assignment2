using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class SalesTarget : BaseEntity
    {
        public Guid DealerId { get; set; }
        public virtual Dealer Dealer { get; set; }
        
        public int Year { get; set; }
        public int Month { get; set; }
        public int Quarter { get; set; }
        
        public decimal TargetAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal AchievementRate { get; set; }
        
        public string Status { get; set; } = "Active"; // Active, Completed, Overdue
        public string Notes { get; set; }
    }
}
