﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Brand : BaseEntity
    {
        public string Name { get; set; }

        public string Country { get; set; }

        public bool IsActive { get; set; } = true;

        public string Description { get; set; }


    }
}
