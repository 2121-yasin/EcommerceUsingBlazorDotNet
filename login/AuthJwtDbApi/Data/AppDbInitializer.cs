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
                           RedirectUrl="http://localhost:3000"

                        },
                        new ClientProfile()
                        {
                            Id=2,
                            Name="Vendor",
                           RedirectUrl="http://localhost:3002"

                        }

                    });
                    context.SaveChanges();
                }

            }

        }

        // public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        // {
        //     using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        //     {

        //         //Roles
        //         var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        //         if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
        //             await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        //         if (!await roleManager.RoleExistsAsync(UserRoles.User))
        //             await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        //         //Users
        //         var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        //         string adminUserEmail = "admin@etickets.com";

        //         var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
        //         if (adminUser == null)
        //         {
        //             var newAdminUser = new ApplicationUser()
        //             {
        //                 FullName = "Admin User",
        //                 UserName = "admin-user",
        //                 Email = adminUserEmail,
        //                 EmailConfirmed = true
        //             };
        //             await userManager.CreateAsync(newAdminUser, "Coding@1234?");
        //             await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
        //         }


        //         string appUserEmail = "user@etickets.com";

        //         var appUser = await userManager.FindByEmailAsync(appUserEmail);
        //         if (appUser == null)
        //         {
        //             var newAppUser = new ApplicationUser()
        //             {
        //                 FullName = "Application User",
        //                 UserName = "app-user",
        //                 Email = appUserEmail,
        //                 EmailConfirmed = true
        //             };
        //             await userManager.CreateAsync(newAppUser, "Coding@1234?");
        //             await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
        //         }
        //     }
        // }
    }
}
