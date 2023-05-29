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
                var electronics = new Category();
                var headphonesAndEarphones = new Category();
                var mobileCategory = new Category();
                var mobileAccessories = new Category();
                var camera = new Category();
                var homeAppliances = new Category();
                var kitchenAppliancesAndCookware = new Category();
                var kitchenAppliances = new Category();
                var cookware = new Category();

                var fashionAndBeauty = new Category();

                var mensFashion = new Category();
                var mensClothing = new Category();
                var mensFootwear = new Category();
                var mensWatches = new Category();

                var womensFashion = new Category();
                var womensFootwear = new Category();
                var beauty = new Category();
                var womensWatches = new Category();
                var womensClothing = new Category();
                var womensHandBag = new Category();

                var kidsFashion = new Category();
                var kidsFootwear = new Category();
                var kidsClothings = new Category();
                var kidsWatches = new Category();




                if (!context.Categories.Any())
                {
                    // Create parent category
                    electronics.Name = "Electronics";
                    electronics.Description = "Electronics Category";
                    context.Categories.Add(electronics);


                    // Create child category - headphonesAndEarphones
                    headphonesAndEarphones.Name = "Headphones And Earphones";
                    headphonesAndEarphones.Description = "Headphones And Earphones category";
                    headphonesAndEarphones.ParentCategory = electronics;
                    context.Categories.Add(headphonesAndEarphones);


                    // create child category - camera
                    camera.Name = "Camera";
                    camera.Description = "Camera Category";
                    camera.ParentCategory = electronics;
                    context.Categories.Add(camera);

                    // Create child category - Mobile
                    mobileCategory.Name = "Mobile";
                    mobileCategory.Description = "Category for mobile phones";
                    mobileCategory.ParentCategory = electronics;
                    context.Categories.Add(mobileCategory);

                    // Create child category - Charger
                    mobileAccessories.Name = "Charger";
                    mobileAccessories.Description = "Category for mobile phone chargers";
                    mobileAccessories.ParentCategory = electronics;
                    context.Categories.Add(mobileAccessories);

                    //parent category of Home Appliances
                    homeAppliances.Name = "Home, Furniture & Appliances";
                    homeAppliances.Description = "Category for Home, Furniture & Appliances";
                    context.Categories.Add(homeAppliances);

                    //child category for Home Appliances
                    kitchenAppliances.Name = "Kitchen Appliances & cookware";
                    kitchenAppliances.Description = "Category for Kitchen Appliances & cookware";
                    kitchenAppliances.ParentCategory = homeAppliances;
                    context.Categories.Add(kitchenAppliances);


                    // Create child category - cookware
                    cookware.Name = "cookware";
                    cookware.Description = "Category for cookware";
                    cookware.ParentCategory = kitchenAppliances;
                    context.Categories.Add(cookware);

                    // Create parent category - Fashion & beauty
                    fashionAndBeauty.Name = "Fashion & beauty";
                    fashionAndBeauty.Description = "Category for Fashion & beauty";
                    context.Categories.Add(fashionAndBeauty);

                    // Create child category - Men's Fashion
                    mensFashion.Name = "Men's Fashion";
                    mensFashion.Description = "Men's fashion";
                    mensFashion.ParentCategory = fashionAndBeauty;
                    context.Categories.Add(mensFashion);

                    // Create child category - Men's Clothing
                    mensClothing.Name = "Men's Clothing";
                    mensClothing.Description = "Category for Men's Clothing";
                    mensClothing.ParentCategory = mensFashion;
                    context.Categories.Add(mensClothing);

                    // Create child category - Men's Footwear
                    mensFootwear.Name = "Men's Footwear";
                    mensFootwear.Description = "Category for Men's Footwear";
                    mensFootwear.ParentCategory = mensFashion;
                    context.Categories.Add(mensFootwear);

                    // Create child category - Men's Watches
                    mensWatches.Name = "Men's Watches";
                    mensWatches.Description = "Category for Men's Watches";
                    mensWatches.ParentCategory = mensFashion;
                    context.Categories.Add(mensWatches);

                    // Create child category - Women's Fashion
                    womensFashion.Name = "Women's Fashion";
                    womensFashion.Description = "Women's fashion";
                    womensFashion.ParentCategory = fashionAndBeauty;
                    context.Categories.Add(womensFashion);

                    // Create child category - Women's Footwear
                    womensFootwear.Name = "Women's Footwear";
                    womensFootwear.Description = "Category for Women's Footwear";
                    womensFootwear.ParentCategory = womensFashion;
                    context.Categories.Add(womensFootwear);

                    // Create child category - beauty
                    beauty.Name = "beauty";
                    beauty.Description = "Category for beauty products";
                    beauty.ParentCategory = womensFashion;
                    context.Categories.Add(beauty);

                    // Create child category - Women's Watches
                    womensWatches.Name = "Women's Watches";
                    womensWatches.Description = "Category for Women's Watches";
                    womensWatches.ParentCategory = womensFashion;
                    context.Categories.Add(womensWatches);

                    // Create child category - Women's Clothing
                    womensClothing.Name = "Women's Clothing";
                    womensClothing.Description = "Category for Women's Clothing";
                    womensClothing.ParentCategory = womensFashion;
                    context.Categories.Add(womensClothing);

                    // Create child category - Women's Handbags
                    womensHandBag.Name = "Women's Handbags";
                    womensHandBag.Description = "Category for Women's Handbags";
                    womensHandBag.ParentCategory = womensFashion;
                    context.Categories.Add(womensHandBag);

                    // Create parent category - Kids Fashion
                    kidsFashion.Name = "Kids Fashion";
                    kidsFashion.Description = "Kids Fashion";
                    context.Categories.Add(kidsFashion);

                    // Create child category - Kids Footwear
                    kidsFootwear.Name = "Kids Footwear";
                    kidsFootwear.Description = "Category for Kids Footwear";
                    kidsFootwear.ParentCategory = kidsFashion;
                    context.Categories.Add(kidsFootwear);

                    // Create child category - Kids Clothing
                    kidsClothings.Name = "Kids Clothing";
                    kidsClothings.Description = "Category for Kids Clothing";
                    kidsClothings.ParentCategory = kidsFashion;
                    context.Categories.Add(kidsClothings);

                    // Create child category - Kids Watches
                    kidsWatches.Name = "Kids Watches";
                    kidsWatches.Description = "Category for Kids Watches";
                    kidsWatches.ParentCategory = kidsFashion;
                    context.Categories.Add(kidsWatches);

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
                            Description="Samsung Galaxy A01 Mobile Charger 2 Amp With Cable",
                            Price=450,
                            ImageURL="https://m.media-amazon.com/images/I/61v+gQmIbvL._SL1024_.jpg",
                            CategoryId=mobileAccessories.CategoryId
                        },
                        new Product()
                        {
                            ProdName="Samsung",
                            Description="Samsung Galaxy S7 Edge G935F 32GB 64GB 128GB",
                            Price=10000,
                            ImageURL="https://freepngimg.com/thumb/samsung_mobile_phone/5-2-samsung-mobile-phone-png-hd.png",
                            CategoryId=mobileCategory.CategoryId
                        },
                        new Product()
                        {
                            ProdName="Apple",
                            Description="iPhone X 64GB/256GB Apple Mobile Smartphone Factory Unlocked | Product ... Apple iPhone 5 32GB Smartphone - T-Mobile - Black",
                            Price=20000,
                            ImageURL="https://m.media-amazon.com/images/I/51zCJOv9AFL._SL1500_.jpg",
                            CategoryId=mobileCategory.CategoryId
                        },
                        new Product()
                        {
                            ProdName="Xiaomi",
                            Description="Xiaomi Redmi 9 Mobile Phone With 64GB 4GB RAM",
                            Price=15000,
                            ImageURL="https://5.imimg.com/data5/SELLER/Default/2021/6/VM/TI/EU/21445559/redmi-mobile-phones-redmi-8-xiaomi-mobiles-500x500.jpeg",
                            CategoryId=mobileCategory.CategoryId
                        },
                         new Product()
                        {
                            ProdName="Pressure Cooker",
                            Description="Cuisine art Pressure cooker - upto 40% off",
                            Price=11000,
                            ImageURL="https://th.bing.com/th/id/R.d7e5597926f7eb0c4319124ef38a75b1?rik=UabnOSY1mu2cbA&riu=http%3a%2f%2fimage.sportsmansguide.com%2fadimgs%2fl%2f2%2f205663_ts.jpg&ehk=MJr85STVKMajybhEQH58fBCp%2fHvqcr6NnVTLh8VLjew%3d&risl=&pid=ImgRaw&r=0",
                            CategoryId=cookware.CategoryId
                        },
                          new Product()
                        {
                            ProdName="Puma T-Shirt",
                            Description="Puma T-Shirt",
                            Price=700,
                            ImageURL="https://i5.walmartimages.com/asr/90248222-9563-4b82-adc6-f62993dcf2b5_1.a30eca828a22eb04a2152bce6b0b4071.jpeg",
                            CategoryId=mensClothing.CategoryId
                        },
                             new Product()
                        {
                            ProdName="Nike Sports Shoes",
                            Description="Shoes Nike Sneakers at - Up to 40% off Top Brands - Wide range of Shoes",
                            Price=4999,
                            ImageURL="https://th.bing.com/th/id/OIP.WqX8OlmM6bsHUdpWIxa9mgHaHe?w=200&h=202&c=7&r=0&o=5&dpr=1.5&pid=1.7",
                            CategoryId=mensFootwear.CategoryId
                        },
                             new Product()
                        {
                            ProdName="Women's t-shirt",
                            Description="NWT INC International Concepts Women Black Short Sleeve T-Shirt",
                            Price=1999,
                            ImageURL="https://th.bing.com/th/id/OIP.MifSAaMovZHA9EE80uFszgHaJ4?pid=ImgDet&rs=1",
                            CategoryId=womensClothing.CategoryId
                        },
                               new Product()
                        {
                            ProdName="SparX Shoe",
                            Description="kids footwear",
                            Price=999,
                            ImageURL="https://th.bing.com/th/id/OIP.kpTU2h7Z89POFJyq5-BWNQHaIq?pid=ImgDet&rs=1",
                            CategoryId=kidsFootwear.CategoryId
                        },
                               new Product()
                        {
                            ProdName="Luis Vuitton",
                            Description="Luis Vuitton Hand bag",
                            Price=3999,
                            ImageURL="https://th.bing.com/th/id/OIP.eKSJ5pPT7KZ44H3ROT9pigHaG6?pid=ImgDet&rs=1",
                            CategoryId=womensHandBag.CategoryId
                        },
                            new Product()
                        {
                            ProdName="FirstCry",
                            Description="FIRSTCRY Baby & Kids Products Online Shopping",
                            Price=1299,
                            ImageURL="https://cdn.fcglcdn.com/brainbees/images/products/438x531/3206249a.jpg",
                            CategoryId=kidsClothings.CategoryId
                        },
                            new Product()
                        {
                            ProdName="VEPOSE Women's Low Heels",
                            Description="VEPOSE Women's Low Heels Closed Toe Pumps Kitten Bridal, Black",
                            Price=5999,
                            ImageURL="https://ae01.alicdn.com/kf/HTB1IH0gfrZnBKNjSZFhq6A.oXXak/2018-New-Women-Platform-Pumps-Sandals-Black-Mesh-Lace-High-Heels-Peep-Toe-Shoes-Coarse-Heels.jpg",
                            CategoryId=womensFootwear.CategoryId
                        },
                         new Product()
                        {
            ProdName="Microwave Oven",
            Description="Convection Microwave Oven",
            Price=8000,
            ImageURL="https://www.lg.com/in/images/microwave-ovens/md05265523/gallery/MC2886BRUM-Microwave-ovens-Detail-2-view-D-04.jpg",
            CategoryId=homeAppliances.CategoryId
        },
        new Product()
        {
            ProdName="Smart TV",
            Description="55-Inch Smart LED TV",
            Price=35000,
            ImageURL="https://i.gadgets360cdn.com/products/televisions/large/1548155076_832_samsung_65-inch-led-ultra-hd-4k-tv-65nu7100.jpg",
            CategoryId=homeAppliances.CategoryId
        },
        new Product()
        {
            ProdName="Laptop",
            Description="Gaming Laptop with Intel Core i7",
            Price=70000,
            ImageURL="https://m.media-amazon.com/images/I/71GAZomYfyL._SY450_.jpg",
            CategoryId=homeAppliances.CategoryId
        }, new Product()
      {
          ProdName="Headphones",
          Description="Wireless Bluetooth Headphones",
          Price=1500,
          ImageURL="https://cdn.shopify.com/s/files/1/1676/7297/products/Main-Image_800x.jpg?v=1613022858",
          CategoryId=headphonesAndEarphones.CategoryId
      },
      new Product()
      {
          ProdName="Camera",
          Description="Digital SLR Camera",
          Price=25000,
          ImageURL="https://media.istockphoto.com/id/96826250/photo/digital-camera-with-clipping-path.jpg?s=612x612&w=0&k=20&c=ceAF827zi_UfczajJAWkXowWxdu5tfisZHRoCiSq94w=",
          CategoryId=camera.CategoryId
      },
      new Product()
      {
          ProdName="Shoes",
          Description="Running Shoes",
          Price=2000,
          ImageURL="https://dxkvlfvncvqr8.cloudfront.net/media/images/product/image/sm-706-smrd-1-1667550033.jpg",
          CategoryId=mensFootwear.CategoryId
      },
      new Product()
      {
          ProdName="Handbag",
          Description="Women's handbag",
          Price=3000,
          ImageURL="https://th.bing.com/th/id/OIP.eKSJ5pPT7KZ44H3ROT9pigHaG6?pid=ImgDet&rs=1",
          CategoryId=womensHandBag.CategoryId
      }


            });
                    context.SaveChanges();
                }

                // ProductVendors
                if (!context.ProductVendors.Any())
                {
                    Random random = new Random();

                    for (int i = 1; i <= 19; i++)
                    {
                        int randomVendorId = random.Next(1, 3);
                        int randomPrice = random.Next(100, 10000);
                        int randomQuantity = random.Next(1, 100);

                        context.ProductVendors.Add(new ProductVendor()
                        {
                            ProductId = i,
                            VendorId = randomVendorId,
                            Price = randomPrice,
                            Quantity = randomQuantity
                        });
                    }

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
