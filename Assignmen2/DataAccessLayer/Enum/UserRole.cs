using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Enum
{
    public enum UserRole
    {
        DealerStaff, //Nhân viên đại lý
        DealerManager,//Quản lý đại lý
        EVMStaff,//Nhân viên EVM ( nv của hãng xe ) 
        Admin
    }
}
