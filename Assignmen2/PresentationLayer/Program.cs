using BusinessLayer.Profiles;
using BusinessLayer.Services;
using PresentationLayer.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using BusinessLayer.Helpers;

namespace PresentationLayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Đăng ký BusinessLayer (bao gồm AutoMapper, DbContext, Repositories và Services)
            builder.Services.AddBusinessLayer(builder.Configuration.GetConnectionString("DefaultConnection"));

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

            // SignalR
            builder.Services.AddSignalR();

            var app = builder.Build();

            // Seed mặc định có thể được thực hiện thông qua BusinessLayer (di chuyển vào BL)

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

            // Map hubs
            app.MapHub<PresentationLayer.Hubs.TestDriveHub>("/testDriveHub");

            app.MapRazorPages();

            app.Run();
        }
    }
}
