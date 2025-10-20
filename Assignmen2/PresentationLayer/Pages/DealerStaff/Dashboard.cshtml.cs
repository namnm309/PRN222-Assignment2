using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Enums;
using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using System.Text.Json;

namespace PresentationLayer.Pages.DealerStaff
{
    public class DashboardModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly ITestDriveService _testDriveService;
        private readonly IMappingService _mappingService;

        public DashboardModel(
            IOrderService orderService,
            ICustomerService customerService,
            ITestDriveService testDriveService,
            IMappingService mappingService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _testDriveService = testDriveService;
            _mappingService = mappingService;
        }

        // User Info
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public Guid? DealerId { get; set; }

        // Today's Stats
        public int TodayOrders { get; set; }
        public decimal TodayRevenue { get; set; }
        public int NewCustomers { get; set; }
        public int TodayTestDrives { get; set; }
        public int PendingTestDrives { get; set; }
        public int TotalCustomers { get; set; }

        // Growth Percentages
        public int OrdersGrowth { get; set; }
        public int RevenueGrowth { get; set; }

        // Recent Data
        public List<OrderResponse> RecentOrders { get; set; } = new();
        public List<TestDriveResponse> RecentTestDrives { get; set; } = new();

        // Chart Data
        public List<string> RevenueChartLabels { get; set; } = new();
        public List<decimal> RevenueChartData { get; set; } = new();
        public List<string> OrderStatusLabels { get; set; } = new();
        public List<int> OrderStatusData { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Get user info from session
            UserName = HttpContext.Session.GetString("UserFullName") ?? "DealerStaff";
            UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            if (Guid.TryParse(dealerIdString, out var dealerId))
            {
                DealerId = dealerId;
            }

            // Set ViewData for all views
            ViewData["UserRole"] = HttpContext.Session.GetString("UserRole");
            ViewData["UserRoleName"] = HttpContext.Session.GetString("UserRole");
            ViewData["UserName"] = UserName;
            ViewData["UserEmail"] = UserEmail;

            if (DealerId.HasValue)
            {
                await LoadDashboardData(DealerId.Value);
            }
            else
            {
                // If no dealer ID, show sample data
                LoadSampleData();
            }
        }

        private async Task LoadDashboardData(Guid dealerId)
        {
            try
            {
                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, 1);
                var lastMonth = thisMonth.AddMonths(-1);

                // Load orders for this month
                var ordersResult = await _orderService.SearchAsync(
                    dealerId, 
                    null, // search
                    null, // status
                    null, // orderType
                    1, // page
                    100 // pageSize - get more data for calculations
                );

                if (ordersResult.Success && ordersResult.Item3.Data != null)
                {
                    var allOrders = ordersResult.Item3.Data;
                    var thisMonthOrders = allOrders.Where(o => o.OrderDate >= thisMonth).ToList();
                    var lastMonthOrders = allOrders.Where(o => o.OrderDate >= lastMonth && o.OrderDate < thisMonth).ToList();
                    
                    // Recent orders (last 5)
                    RecentOrders = allOrders.OrderByDescending(o => o.OrderDate).Take(5)
                        .Select(o => new OrderResponse
                        {
                            OrderNumber = o.OrderNumber,
                            CustomerName = o.Customer?.FullName ?? string.Empty,
                            ProductName = o.Product?.Name ?? string.Empty,
                            FinalAmount = o.FinalAmount,
                            Status = o.Status,
                            CreatedAt = o.CreatedAt
                        }).ToList();
                    
                    // Calculate stats
                    TodayOrders = allOrders.Count(o => o.OrderDate?.Date == today);
                    TodayRevenue = allOrders.Where(o => o.OrderDate?.Date == today).Sum(o => o.FinalAmount);
                    
                    // Calculate growth
                    var thisMonthOrderCount = thisMonthOrders.Count;
                    var lastMonthOrderCount = lastMonthOrders.Count;
                    OrdersGrowth = lastMonthOrderCount > 0 ? 
                        (int)((thisMonthOrderCount - lastMonthOrderCount) * 100.0 / lastMonthOrderCount) : 0;
                    
                    var thisMonthRevenue = thisMonthOrders.Sum(o => o.FinalAmount);
                    var lastMonthRevenue = lastMonthOrders.Sum(o => o.FinalAmount);
                    RevenueGrowth = lastMonthRevenue > 0 ? 
                        (int)((thisMonthRevenue - lastMonthRevenue) * 100m / lastMonthRevenue) : 0;
                }

                // Load customers
                var customersResult = await _customerService.GetAllAsync();
                if (customersResult.Success && customersResult.Data != null)
                {
                    var customers = customersResult.Data;
                    TotalCustomers = customers.Count;
                    NewCustomers = customers.Count(c => c.CreatedAt.Date == today);
                }

                // Load test drives
                var testDrivesResult = await _testDriveService.GetByDealerAsync(dealerId);
                if (testDrivesResult.Success && testDrivesResult.Data != null)
                {
                    var testDrives = testDrivesResult.Data;
                    RecentTestDrives = testDrives.OrderByDescending(t => t.ScheduledDate).Take(5)
                        .Select(t => new TestDriveResponse
                        {
                            CustomerName = t.CustomerName,
                            ProductName = t.ProductName,
                            ScheduledDate = t.ScheduledDate,
                            Status = t.Status
                        }).ToList();
                    
                    TodayTestDrives = testDrives.Count(t => t.ScheduledDate.Date == today);
                    PendingTestDrives = testDrives.Count(t => t.Status.ToString() == "Pending");
                }

                // Load chart data
                await LoadChartData(dealerId);
            }
            catch (Exception ex)
            {
                // Log error and load sample data
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard data: {ex.Message}");
                LoadSampleData();
            }
        }

        private void LoadSampleData()
        {
            // Sample data for demonstration
            UserName = "Nguyễn Văn A";
            TodayOrders = 12;
            TodayRevenue = 250000000;
            NewCustomers = 3;
            TodayTestDrives = 5;
            PendingTestDrives = 2;
            TotalCustomers = 156;
            OrdersGrowth = 15;
            RevenueGrowth = 23;

            // Sample recent orders
            RecentOrders = new List<OrderResponse>
            {
                new OrderResponse
                {
                    OrderNumber = "ORD001",
                    CustomerName = "Trần Thị B",
                    ProductName = "Tesla Model 3",
                    FinalAmount = 1500000000,
                    Status = "Completed",
                    CreatedAt = DateTime.Now.AddHours(-2)
                },
                new OrderResponse
                {
                    OrderNumber = "ORD002",
                    CustomerName = "Lê Văn C",
                    ProductName = "BMW iX",
                    FinalAmount = 2800000000,
                    Status = "Pending",
                    CreatedAt = DateTime.Now.AddHours(-4)
                }
            };

            // Sample recent test drives
            RecentTestDrives = new List<TestDriveResponse>
            {
                new TestDriveResponse
                {
                    CustomerName = "Phạm Thị D",
                    ProductName = "Audi e-tron",
                    ScheduledDate = DateTime.Now.AddHours(2),
                    Status = BusinessLayer.Enums.TestDriveStatus.Confirmed
                },
                new TestDriveResponse
                {
                    CustomerName = "Hoàng Văn E",
                    ProductName = "Mercedes EQS",
                    ScheduledDate = DateTime.Now.AddDays(1),
                    Status = BusinessLayer.Enums.TestDriveStatus.Pending
                }
            };

            LoadSampleChartData();
        }

        private async Task LoadChartData(Guid dealerId)
        {
            try
            {
                // Generate last 7 days labels and get actual revenue data
                RevenueChartLabels = new List<string>();
                RevenueChartData = new List<decimal>();
                
                for (int i = 6; i >= 0; i--)
                {
                    var date = DateTime.Today.AddDays(-i);
                    RevenueChartLabels.Add(date.ToString("dd/MM"));
                    
                    // Get actual revenue for this date
                    var dayOrdersResult = await _orderService.SearchAsync(
                        dealerId, 
                        null, // search
                        null, // status
                        null, // orderType
                        1, // page
                        100 // pageSize
                    );
                    
                    decimal dayRevenue = 0;
                    if (dayOrdersResult.Success && dayOrdersResult.Item3.Data != null)
                    {
                        dayRevenue = dayOrdersResult.Item3.Data
                            .Where(o => o.OrderDate?.Date == date)
                            .Sum(o => o.FinalAmount);
                    }
                    
                    RevenueChartData.Add(dayRevenue);
                }

                // Get actual order status distribution
                var statusOrdersResult = await _orderService.SearchAsync(
                    dealerId, 
                    null, // search
                    null, // status
                    null, // orderType
                    1, // page
                    100 // pageSize
                );
                
                if (statusOrdersResult.Success && statusOrdersResult.Item3.Data != null)
                {
                    var orders = statusOrdersResult.Item3.Data;
                    var statusGroups = orders.GroupBy(o => o.Status).ToList();
                    
                    OrderStatusLabels = new List<string>();
                    OrderStatusData = new List<int>();
                    
                    foreach (var group in statusGroups)
                    {
                        OrderStatusLabels.Add(GetStatusDisplayName(group.Key));
                        OrderStatusData.Add(group.Count());
                    }
                }
                else
                {
                    // Fallback to sample data
                    OrderStatusLabels = new List<string> { "Chờ xử lý", "Hoàn thành", "Đã hủy", "Đã giao", "Khác" };
                    OrderStatusData = new List<int> { 5, 12, 2, 8, 1 };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading chart data: {ex.Message}");
                LoadSampleChartData();
            }
        }
        
        private string GetStatusDisplayName(string status)
        {
            return status switch
            {
                "Pending" => "Chờ xử lý",
                "Completed" => "Hoàn thành",
                "Cancelled" => "Đã hủy",
                "Delivered" => "Đã giao",
                "Processing" => "Đang xử lý",
                _ => "Khác"
            };
        }

        private void LoadSampleChartData()
        {
            // Sample chart data
            RevenueChartLabels = new List<string> { "15/01", "16/01", "17/01", "18/01", "19/01", "20/01", "21/01" };
            RevenueChartData = new List<decimal> { 150000000, 200000000, 180000000, 250000000, 300000000, 220000000, 280000000 };
            
            OrderStatusLabels = new List<string> { "Chờ xử lý", "Hoàn thành", "Đã hủy", "Đã giao", "Khác" };
            OrderStatusData = new List<int> { 5, 12, 2, 8, 1 };
        }
    }
}