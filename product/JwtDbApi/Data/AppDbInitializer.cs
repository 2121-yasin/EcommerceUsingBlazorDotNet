using JwtDbApi.Models;

namespace JwtDbApi.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

                context.Database.EnsureCreated();

                // Vendors
                if (!context.Vendors.Any())
                {
                    context.Vendors.AddRange(new List<Vendor>()
                    {
                        new Vendor()
                        {
                            UserId=2,
                        },
                        new Vendor()
                        {
                            GSTIN="30AAAAA0000A1Z5",
                            DeliveryPinCode=403002,
                            UserId=3,
                        }
                    });
                    context.SaveChanges();
                }

                // Categories
                var parentCategory = new Category();
                var mobileCategory = new Category();
                var chargerCategory = new Category();

                if (!context.Categories.Any())
                {
                    // Create parent category
                    parentCategory.Name = "Mobiles & Accessories";
                    parentCategory.Description = "Category for mobiles and accessories";
                    context.Categories.Add(parentCategory);

                    // Create child category - Mobile
                    mobileCategory.Name = "Mobile";
                    mobileCategory.Description = "Category for mobile phones";
                    mobileCategory.ParentCategory = parentCategory;
                    context.Categories.Add(mobileCategory);

                    // Create child category - Charger
                    chargerCategory.Name = "Charger";
                    chargerCategory.Description = "Category for mobile phone chargers";
                    chargerCategory.ParentCategory = parentCategory;
                    context.Categories.Add(chargerCategory);

                    context.SaveChanges();
                }

                // Products
                if (!context.Products.Any())
                {
                    context.Products.AddRange(new List<Product>()
                    {
                        new Product()
                        {
                            ProdName="Charger",
                            Description="Charger for Samsung",
                            Price=450,
                            ImageURL="https://m.media-amazon.com/images/W/IMAGERENDERING_521856-T1/images/I/51G529B1p1L._SL1200_.jpg",
                            CategoryId=chargerCategory.CategoryId
                        },
                        new Product()
                        {
                            ProdName="Samsung",
                            Description="Samsung",
                            Price=10000,
                            ImageURL="https://images.pexels.com/photos/47261/pexels-photo-47261.jpeg?auto=compress&cs=tinysrgb&dpr=1&w=500",
                            CategoryId=mobileCategory.CategoryId
                        },
                        new Product()
                        {
                            ProdName="Apple",
                            Description="Apple",
                            Price=20000,
                            ImageURL="https://thumbs.dreamstime.com/b/iphone-most-recent-isolated-white-background-46056944.jpg",
                            CategoryId=mobileCategory.CategoryId
                        },
                        new Product()
                        {
                            ProdName="Xiaomi",
                            Description="Xiaomi",
                            Price=15000,
                            ImageURL="https://st4.depositphotos.com/11698096/22999/i/600/depositphotos_229999768-stock-photo-kyiv-ukraine-july-2018-xiaomi.jpg",
                            CategoryId=mobileCategory.CategoryId
                        },

                    });
                    context.SaveChanges();
                }

                // ProductVendors
                if (!context.ProductVendors.Any())
                {
                    context.ProductVendors.AddRange(new List<ProductVendor>()
                    {
                        new ProductVendor()
                        {
                            ProductId=1,
                            VendorId=2
                        },
                        new ProductVendor()
                        {
                            ProductId=2,
                            VendorId=2
                        },
                        new ProductVendor()
                        {
                            ProductId=4,
                            VendorId=2
                        },
                        new ProductVendor()
                        {
                            ProductId=3,
                            VendorId=1
                        },
                    });
                    context.SaveChanges();
                }

                // Admins
                if (!context.Admins.Any())
                {
                    var admin = new Admin();
                    admin.UserId = 1;
                    context.Admins.Add(admin);
                    context.SaveChanges();
                }
            }
        }
    }
}
