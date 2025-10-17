using BusinessLayer.Profiles;
using BusinessLayer.Services;
using DataAccessLayer.Data;
using DataAccessLayer.Repository;
using Microsoft.EntityFrameworkCore;
using PresentationLayer.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PresentationLayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình AutoMapper
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            // Đăng ký MappingService và các dịch vụ BusinessLayer qua DI
            builder.Services.AddScoped<IMappingService, MappingService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IBrandService, BrandService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IDealerService, DealerService>();
            builder.Services.AddScoped<IDealerContractService, DealerContractService>();
            builder.Services.AddScoped<IDealerDebtService, DealerDebtService>();
            builder.Services.AddScoped<IEVMReportService, EVMReportService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<IInventoryManagementService, InventoryManagementService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IPricingManagementService, PricingManagementService>();
            builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            builder.Services.AddScoped<ITestDriveService, TestDriveService>();
            builder.Services.AddScoped<IAuthenService, AuthenService>();

            // Cấu hình DbContext (đăng ký DI trước khi build app)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            

            // Add services to the container.
            builder.Services.AddRazorPages(options =>
            {
                // Route "/" tới trang Home/Index
                options.Conventions.AddPageRoute("/Home/Index", "");
                // Thêm filter bảo vệ Dashboard
                options.Conventions.AddFolderApplicationModelConvention("/Dashboard", model =>
                {
                    model.Filters.Add(new DashboardAuthorizePageFilter());
                });

                // Thêm filter bảo vệ khu vực DealerManager (yêu cầu đã đăng nhập)
                options.Conventions.AddFolderApplicationModelConvention("/DealerManager", model =>
                {
                    model.Filters.Add(new DashboardAuthorizePageFilter());
                });
            });

            // Authentication - Cookie scheme (default)
            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/Login";
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                });

            // Session for auth
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Đăng ký Repository (DataAccessLayer)
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IBrandRepository, BrandRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IDealerRepository, DealerRepository>();
            builder.Services.AddScoped<IDealerContractRepository, DealerContractRepository>();
            //builder.Services.AddScoped<IDealerDebtRepository, DealerDebtRepository>();
            builder.Services.AddScoped<IEVMRepository, EVMRepository>();
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            builder.Services.AddScoped<IInventoryManagementRepository, InventoryManagementRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IPricingManagementRepository, PricingManagementRepository>();
            builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            builder.Services.AddScoped<ITestDriveRepository, TestDriveRepository>();
            builder.Services.AddScoped<IAuthen, Authen>();

            var app = builder.Build();

            // Seed dữ liệu mặc định cho tài khoản EVMStaff
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    
                    // Đảm bảo database đã được tạo
                    context.Database.Migrate();
                    
                    // Kiểm tra và tạo tài khoản EVMStaff mặc định
                    var evmStaffEmail = "evm.staff@vinfast.com";
                    var existingEVMStaff = context.Users.FirstOrDefault(u => u.Email == evmStaffEmail);
                    
                    if (existingEVMStaff == null)
                    {
                        var evmStaffUser = new DataAccessLayer.Entities.Users
                        {
                            Id = Guid.NewGuid(),
                            FullName = "EVM Staff",
                            Email = evmStaffEmail,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("evmstaff123"),
                            PhoneNumber = "0900000001",
                            Address = "VinFast HQ, Hanoi",
                            Role = DataAccessLayer.Enum.UserRole.EVMStaff,
                            DealerId = null, // EVMStaff không thuộc dealer nào
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        
                        context.Users.Add(evmStaffUser);
                        context.SaveChanges();
                        
                        logger.LogInformation("✅ Tạo tài khoản EVMStaff mặc định thành công: {Email}", evmStaffEmail);
                    }
                    else
                    {
                        logger.LogInformation("ℹ️ Tài khoản EVMStaff đã tồn tại: {Email}", evmStaffEmail);
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "❌ Lỗi khi seed tài khoản EVMStaff mặc định");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
