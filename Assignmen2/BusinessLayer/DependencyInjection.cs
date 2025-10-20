using BusinessLayer.Profiles;
using BusinessLayer.Services;
using DataAccessLayer.Data;
using DataAccessLayer.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services, string connectionString)
        {
            // AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile));

            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Repositories (DAL)
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IDealerRepository, DealerRepository>();
            services.AddScoped<IDealerContractRepository, DealerContractRepository>();
            services.AddScoped<IEVMRepository, EVMRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IInventoryManagementRepository, InventoryManagementRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPricingManagementRepository, PricingManagementRepository>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<ITestDriveRepository, TestDriveRepository>();
            services.AddScoped<IAuthen, Authen>();

            // Business services
            services.AddScoped<IMappingService, MappingService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IDealerService, DealerService>();
            services.AddScoped<IDealerContractService, DealerContractService>();
            services.AddScoped<IDealerDebtService, DealerDebtService>();
            services.AddScoped<IEVMReportService, EVMReportService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IInventoryManagementService, InventoryManagementService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPricingManagementService, PricingManagementService>();
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            services.AddScoped<ITestDriveService, TestDriveService>();
            services.AddScoped<IAuthenService, AuthenService>();

            return services;
        }
    }
}


