using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Database migration sẽ được gọi ở Program.cs để tránh lỗi lúc design-time
        }
        // Constructor mặc định cho testing/migration
        public AppDbContext()
        {
        }
        // Khai báo các DbSet Entity - mỗi DbSet đại diện cho một bảng trong cơ sở dữ liệu 
        public DbSet<Users> Users { get; set; }
        
        public DbSet<Product> Product { get; set; }

        public DbSet<Brand> Brand { get; set; }

        public DbSet<Customer> Customer { get; set; }
        
        public DbSet<Dealer> Dealer { get; set; }
        
        public DbSet<Category> Categorie { get; set; }
        
        public DbSet<Order> Order { get; set; }
        
        public DbSet<Feedback> Feedback { get; set; }
        
        public DbSet<Promotion> Promotion { get; set; }
        
        public DbSet<TestDrive> TestDrive { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        
        public DbSet<Region> Region { get; set; }
        
        public DbSet<SalesTarget> SalesTarget { get; set; }
        
        public DbSet<DealerContract> DealerContract { get; set; }
        
        public DbSet<InventoryAllocation> InventoryAllocation { get; set; }
        
        public DbSet<PricingPolicy> PricingPolicy { get; set; }
        
        public DbSet<InventoryTransaction> InventoryTransaction { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình mối quan hệ Product - Brand (Many-to-One)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Order - Customer (Many-to-One)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Order - Dealer (Many-to-One)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Dealer)
                .WithMany()
                .HasForeignKey(o => o.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Order - Product (Many-to-One)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Product)
                .WithMany()
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Feedback - Customer (Many-to-One)
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Customer)
                .WithMany()
                .HasForeignKey(f => f.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Feedback - Product (Many-to-One)
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Product)
                .WithMany()
                .HasForeignKey(f => f.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ TestDrive - Customer (Many-to-One)
            modelBuilder.Entity<TestDrive>()
                .HasOne(t => t.Customer)
                .WithMany()
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ TestDrive - Product (Many-to-One)
            modelBuilder.Entity<TestDrive>()
                .HasOne(t => t.Product)
                .WithMany()
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ TestDrive - Dealer (Many-to-One)
            modelBuilder.Entity<TestDrive>()
                .HasOne(t => t.Dealer)
                .WithMany()
                .HasForeignKey(t => t.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PurchaseOrder properties
            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Reason).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.RejectReason).HasMaxLength(500);
            });

            // Configure PurchaseOrder relationships
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.Dealer)
                .WithMany()
                .HasForeignKey(p => p.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.RequestedBy)
                .WithMany()
                .HasForeignKey(p => p.RequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.ApprovedBy)
                .WithMany()
                .HasForeignKey(p => p.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Order - Region (Many-to-One)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Region)
                .WithMany()
                .HasForeignKey(o => o.RegionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Dealer - Region (Many-to-One)
            modelBuilder.Entity<Dealer>()
                .HasOne(d => d.Region)
                .WithMany()
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ SalesTarget - Dealer (Many-to-One)
            modelBuilder.Entity<SalesTarget>()
                .HasOne(st => st.Dealer)
                .WithMany()
                .HasForeignKey(st => st.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ DealerContract - Dealer (Many-to-One)
            modelBuilder.Entity<DealerContract>()
                .HasOne(dc => dc.Dealer)
                .WithMany()
                .HasForeignKey(dc => dc.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình các thuộc tính bắt buộc
            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(200);
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Country).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Sku).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(200);
            });

            modelBuilder.Entity<Dealer>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(200);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.ModelName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.color).HasMaxLength(50);
                entity.Property(e => e.varian).HasMaxLength(50);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Discount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasMaxLength(20);
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.Property(e => e.Comment).HasMaxLength(1000);
               
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.Property(e => e.title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.description).HasMaxLength(1000);
            });

            modelBuilder.Entity<TestDrive>(entity =>
            {
                entity.Property(e => e.Status).HasMaxLength(20);
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            modelBuilder.Entity<SalesTarget>(entity =>
            {
                entity.Property(e => e.TargetAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ActualAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AchievementRate).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Notes).HasMaxLength(1000);
            });

            modelBuilder.Entity<DealerContract>(entity =>
            {
                entity.Property(e => e.ContractNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CommissionRate).HasColumnType("decimal(5,2)");
                entity.Property(e => e.CreditLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OutstandingDebt).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Terms).HasMaxLength(2000);
                entity.Property(e => e.Notes).HasMaxLength(1000);
            });

            modelBuilder.Entity<Dealer>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.City).HasMaxLength(50);
                entity.Property(e => e.Province).HasMaxLength(50);
                entity.Property(e => e.DealerCode).HasMaxLength(20);
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.LicenseNumber).HasMaxLength(50);
                entity.Property(e => e.CreditLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OutstandingDebt).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasMaxLength(20);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderNumber).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Discount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FinalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.PaymentStatus).HasMaxLength(20);
                entity.Property(e => e.PaymentMethod).HasMaxLength(20);
                entity.Property(e => e.Notes).HasMaxLength(1000);
            });

            // Cấu hình mối quan hệ InventoryAllocation - Product (Many-to-One)
            modelBuilder.Entity<InventoryAllocation>()
                .HasOne(ia => ia.Product)
                .WithMany()
                .HasForeignKey(ia => ia.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ InventoryAllocation - Dealer (Many-to-One)
            modelBuilder.Entity<InventoryAllocation>()
                .HasOne(ia => ia.Dealer)
                .WithMany()
                .HasForeignKey(ia => ia.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ PricingPolicy - Product (Many-to-One)
            modelBuilder.Entity<PricingPolicy>()
                .HasOne(pp => pp.Product)
                .WithMany()
                .HasForeignKey(pp => pp.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ PricingPolicy - Dealer (Many-to-One)
            modelBuilder.Entity<PricingPolicy>()
                .HasOne(pp => pp.Dealer)
                .WithMany()
                .HasForeignKey(pp => pp.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ PricingPolicy - Region (Many-to-One)
            modelBuilder.Entity<PricingPolicy>()
                .HasOne(pp => pp.Region)
                .WithMany()
                .HasForeignKey(pp => pp.RegionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ InventoryTransaction - Product (Many-to-One)
            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Product)
                .WithMany()
                .HasForeignKey(it => it.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ InventoryTransaction - Dealer (Many-to-One)
            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Dealer)
                .WithMany()
                .HasForeignKey(it => it.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ InventoryTransaction - Order (Many-to-One)
            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Order)
                .WithMany()
                .HasForeignKey(it => it.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ InventoryTransaction - User (Many-to-One)
            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.ProcessedByUser)
                .WithMany()
                .HasForeignKey(it => it.ProcessedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryAllocation>(entity =>
            {
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Priority).HasMaxLength(20);
                entity.Property(e => e.Notes).HasMaxLength(1000);
            });

            modelBuilder.Entity<PricingPolicy>(entity =>
            {
                entity.Property(e => e.WholesalePrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.RetailPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountRate).HasColumnType("decimal(5,2)");
                entity.Property(e => e.MinimumPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PolicyType).HasMaxLength(50);
                entity.Property(e => e.ApplicableConditions).HasMaxLength(1000);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Notes).HasMaxLength(1000);
            });

            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.Property(e => e.TransactionType).HasMaxLength(20);
                entity.Property(e => e.Reason).HasMaxLength(100);
                entity.Property(e => e.ReferenceNumber).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Notes).HasMaxLength(1000);
            });
        }
        }
}
