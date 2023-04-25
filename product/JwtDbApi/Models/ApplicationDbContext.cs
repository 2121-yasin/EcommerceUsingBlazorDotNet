using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace JwtDbApi.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.ChildCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Category>()
            .HasMany(c => c.ChildCategories)
            .WithOne(p => p.ParentCategory)
            .HasForeignKey(f => f.ParentCategoryId);

            modelBuilder.Entity<ProductVendor>()
                .HasKey(pv => new { pv.ProductId, pv.VendorId });


            modelBuilder.Entity<ProductVendor>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.ProductVendors)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProductVendor>()
                .HasOne(pv => pv.Vendor)
                .WithMany(v => v.ProductVendors)
                .HasForeignKey(pv => pv.VendorId)
                .OnDelete(DeleteBehavior.NoAction);




        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<ProductVendor> ProductVendors { get; set; }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }

        //public DbSet<CartItem> CartItems { get; set; }
    }
}