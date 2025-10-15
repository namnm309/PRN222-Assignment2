using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Category : BaseEntity
    {
       public string ModelName { get; set; }

        public string color { get; set; }

        public string varian { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
