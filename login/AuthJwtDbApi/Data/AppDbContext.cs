using AuthJwtDbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthJwtDbApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }


        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<AddressInfo> AddressInfo { get; set; }
        public DbSet<ClientProfile> ClientProfile { get; set; }

    }
}