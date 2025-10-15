using DataAccessLayer.Repository;
using DataAccessLayer.Data;
using BusinessLayer.Services;
using BusinessLayer.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using System;
using AutoMapper;

namespace PresentationLayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            // Cấu hình AutoMapper
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
            
            // Đăng ký MappingService
            builder.Services.AddScoped<IMappingService, MappingService>();


            // Đăng ký DbContext cho Repository DAL (chỉ cấu hình DI, không dùng trực tiếp ở Controller)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký Repository và Services
            builder.Services.AddScoped<IAuthen, Authen>();
            builder.Services.AddScoped<IAuthenService, AuthenService>();
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<ITestDriveRepository, TestDriveRepository>();
            builder.Services.AddScoped<ITestDriveService, TestDriveService>();
            builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IBrandRepository, BrandRepository>();
            builder.Services.AddScoped<IBrandService, BrandService>();            
            builder.Services.AddScoped<IDealerRepository, DealerRepository>();
            builder.Services.AddScoped<IDealerService, DealerService>();            
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();            
            builder.Services.AddScoped<IDealerContractRepository, DealerContractRepository>();
            builder.Services.AddScoped<IDealerContractService, DealerContractService>();
            builder.Services.AddScoped<IEVMRepository, EVMRepository>();
            builder.Services.AddScoped<IEVMReportService, EVMReportService>();
            builder.Services.AddScoped<IInventoryManagementRepository, InventoryManagementRepository>();
            builder.Services.AddScoped<IInventoryManagementService, InventoryManagementService>();
            builder.Services.AddScoped<IPricingManagementRepository, PricingManagementRepository>();
            builder.Services.AddScoped<IPricingManagementService, PricingManagementService>();
            builder.Services.AddScoped<IDealerDebtService, DealerDebtService>();


            // Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Seed dữ liệu mẫu khi khởi động (chỉ cho môi trường Development)
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Seeding nên chuyển sang BL Seeder (bỏ ở Presentation)
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Lỗi khi migrate/seed dữ liệu.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
