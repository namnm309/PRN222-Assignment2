using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Feedback : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; } 
    }
}
