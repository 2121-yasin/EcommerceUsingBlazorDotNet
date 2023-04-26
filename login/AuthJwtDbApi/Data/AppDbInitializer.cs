using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthJwtDbApi.Models;

namespace AuthJwtDbApi.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();

                if (!context.UserInfo.Any())
                {
                    context.UserInfo.AddRange(new List<UserInfo>()
                    {
                        new UserInfo()
                        {
                            UserId=1,
                            UserName="Aaron",
                            Email="aaron@gmail.com",
                            Password= BCrypt.Net.BCrypt.HashPassword("Aaron@123"),
                            Role="Admin",
                            AddressId=1
                        },
                        new UserInfo()
                        {
                            UserId=2,
                            UserName="Suraj",
                            Email="suraj@gmail.com",
                            Password= BCrypt.Net.BCrypt.HashPassword("Suraj@123"),
                            Role="Vendor",
                            AddressId=1

                        },
                        new UserInfo()
                        {
                            UserId=3,
                            UserName="Yasin",
                            Email="yasin@gmail.com",
                            Password= BCrypt.Net.BCrypt.HashPassword("Yasin@123"),
                            Role="User",
                            AddressId=1

                        }
                    });
                    context.SaveChanges();
                }




                if (!context.AddressInfo.Any())
                {
                    context.AddressInfo.AddRange(new List<AddressInfo>()
                    {
                        new AddressInfo()
                        {
                            AddressId=1,
                            Street="abc",
                            City="panaji",
                            State="Goa",
                            Pincode="403005"

                        },
                        new AddressInfo()
                        {
                            AddressId=2,
                            Street="abc",
                            City="panaji",
                            State="Goa",
                            Pincode="403005"
                        },
                        new AddressInfo()
                        {
                            AddressId=3,
                            Street="abc",
                            City="panaji",
                            State="Goa",
                            Pincode="403005"
                        }

                    });
                    context.SaveChanges();
                }




                //ClientProfile
                if (!context.ClientProfile.Any())
                {
                    context.ClientProfile.AddRange(new List<ClientProfile>()
                    {
                        new ClientProfile()
                        {
                            Id=1,
                            Name="Admin",
                            ClientId = new Guid("b6782e13-5669-4156-82a8-1850883214e4"),
                            RedirectUrl="http://localhost:3000"

                        },
                        new ClientProfile()
                        {
                            Id=2,
                            Name="Vendor",
                            ClientId = new Guid("ff84a00f-99ab-4f81-9f52-26df485a9dcf"),
                            RedirectUrl="http://localhost:3002"

                        },
                        new ClientProfile()
                        {
                            Id=3,
                            Name="User",
                            ClientId = new Guid("A0D0B3A2-EFA4-47CA-B193-45BDBD950F3A"),
                            RedirectUrl="https://localhost:7042"

                        },

                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
