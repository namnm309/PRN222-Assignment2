using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Order : BaseEntity
    {
        //Khóa ngoại cho bảng Customer, Dealer, Product
        public Guid CustomerId { get; set; }
        public Guid DealerId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? SalesPersonId { get; set; } // Nhân viên bán hàng

        public virtual Customer Customer { get; set; } //Trong order chứa thông tin khách hàng 
        public virtual Dealer Dealer { get; set; } //Trong order chứa Id của đại lý 
        public virtual Product Product { get; set; } //Trong order chứa Id của sản phẩm
        public virtual Region Region { get; set; } //Khu vực bán hàng
        public virtual Users SalesPerson { get; set; } //Nhân viên bán hàng

        public string OrderNumber { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Status { get; set; } = "Pending"; //Pending, Completed, Canceled, Delivered
        public string PaymentStatus { get; set; } = "Pending"; //Pending, Paid, Partial, Overdue
        public string PaymentMethod { get; set; } //Cash, Installment, BankTransfer
        
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        
        public string Notes { get; set; }
    }

}
