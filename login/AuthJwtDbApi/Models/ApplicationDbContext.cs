using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AuthJwtDbApi.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<AddressInfo> AddressInfo { get; set; }
        public DbSet<ClientProfile> ClientProfile { get; set; }

    }
}