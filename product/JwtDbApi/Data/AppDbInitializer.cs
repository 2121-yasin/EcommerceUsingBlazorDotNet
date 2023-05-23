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
                            ImageURL="https://m.media-amazon.com/images/I/61v+gQmIbvL._SL1024_.jpg",
                            CategoryId=chargerCategory.CategoryId
                        },
                        new Product()
                        {
                            ProdName="Samsung",
                            Description="Samsung",
                            Price=10000,
                            ImageURL="https://freepngimg.com/thumb/samsung_mobile_phone/5-2-samsung-mobile-phone-png-hd.png",
                            CategoryId=mobileCategory.CategoryId
                        },
                        new Product()
                        {
                            ProdName="Apple",
                            Description="Apple",
                            Price=20000,
                            ImageURL="https://m.media-amazon.com/images/I/51zCJOv9AFL._SL1500_.jpg",
                            CategoryId=mobileCategory.CategoryId
                        },
                        new Product()
                        {
                            ProdName="Xiaomi",
                            Description="Xiaomi",
                            Price=15000,
                            ImageURL="https://5.imimg.com/data5/SELLER/Default/2021/6/VM/TI/EU/21445559/redmi-mobile-phones-redmi-8-xiaomi-mobiles-500x500.jpeg",
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
