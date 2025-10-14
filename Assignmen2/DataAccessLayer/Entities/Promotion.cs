using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Promotion : BaseEntity
    {
        public string title { get; set; }

        public string description { get; set; }

        public bool IsActive { get; set; } = true;

    }
}
