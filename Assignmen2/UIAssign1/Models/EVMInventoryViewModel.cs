using System;
using System.Collections.Generic;

namespace PresentationLayer.Models
{
    public class EVMInventoryViewModel
    {
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public string BrandName { get; set; }
        public int CurrentStock { get; set; }
        public int MinStockLevel { get; set; }
        public int MaxStockLevel { get; set; }
        public decimal ConsumptionRate { get; set; } // Số lượng tiêu thụ/ngày
        public int DaysToStockOut { get; set; }
        public string StockStatus { get; set; } // Normal, Low, Critical, OutOfStock
        public decimal TurnoverRate { get; set; } // Tỷ lệ quay vòng kho
        public DateTime LastUpdated { get; set; }
    }

    public class EVMInventoryFilterViewModel
    {
        public string ProductId { get; set; }
        public string BrandId { get; set; }
        public string StockStatus { get; set; }
        public string Priority { get; set; }
        public int? MinStockLevel { get; set; }
        public int? MaxStockLevel { get; set; }
    }
}
