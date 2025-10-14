using System;
using System.Collections.Generic;

namespace PresentationLayer.Models
{
    public class EVMDemandForecastViewModel
    {
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public string BrandName { get; set; }
        public string RegionName { get; set; }
        public int CurrentDemand { get; set; }
        public int PredictedDemand { get; set; }
        public decimal ConfidenceLevel { get; set; } // Mức độ tin cậy của dự đoán
        public string Trend { get; set; } // Increasing, Decreasing, Stable
        public decimal GrowthRate { get; set; }
        public int RecommendedProduction { get; set; }
        public int RecommendedDistribution { get; set; }
        public DateTime ForecastDate { get; set; }
        public string Priority { get; set; } // High, Medium, Low
    }

    public class EVMDemandForecastFilterViewModel
    {
        public string ProductId { get; set; }
        public string BrandId { get; set; }
        public string RegionId { get; set; }
        public string Priority { get; set; }
        public int ForecastPeriod { get; set; } // Số tháng dự đoán
        public string Trend { get; set; }
    }
}
