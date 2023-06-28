using JwtDbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtDbApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder
                .Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.ChildCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder
                .Entity<Category>()
                .HasMany(c => c.ChildCategories)
                .WithOne(p => p.ParentCategory)
                .HasForeignKey(f => f.ParentCategoryId);

            // modelBuilder.Entity<ProductVendor>().HasKey(pv => new { pv.ProductId, pv.VendorId });

            modelBuilder
                .Entity<ProductVendor>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.ProductVendors)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder
                .Entity<ProductVendor>()
                .HasOne(pv => pv.Vendor)
                .WithMany(v => v.ProductVendors)
                .HasForeignKey(pv => pv.VendorId)
                .OnDelete(DeleteBehavior.NoAction);

            // CartItem
            modelBuilder
                .Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<CartItem>()
                .HasOne(ci => ci.ProductVendor)
                .WithMany(pv => pv.CartItems)
                .HasForeignKey(ci => ci.ProductVendorId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVendor> ProductVendors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<QCRequest> QCRequests { get; set; }

        //public DbSet<CartItem> CartItems { get; set; }
    }
}
