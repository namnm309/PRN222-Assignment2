using BusinessLayer.Profiles;
using BusinessLayer.Services;
using DataAccessLayer.Data;
using DataAccessLayer.Repository;
using Microsoft.EntityFrameworkCore;

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

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
