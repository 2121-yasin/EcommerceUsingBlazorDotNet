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
                    context.Vendors.AddRange(
                        new List<Vendor>()
                        {
                            new Vendor() { UserId = 2, },
                            new Vendor()
                            {
                                GSTIN = "30AAAAA0000A1Z5",
                                DeliveryPinCode = 403002,
                                UserId = 3,
                            }
                        }
                    );
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
                    electronics.CategoryImageUrl =
                        "https://w7.pngwing.com/pngs/381/468/png-transparent-home-appliance-kitchen-consumer-electronics-house-kitchen-miscellaneous-television-kitchen.png";
                    context.Categories.Add(electronics);

                    // Create child category - headphonesAndEarphones
                    headphonesAndEarphones.Name = "Headphones And Earphones";
                    headphonesAndEarphones.Description = "Headphones And Earphones category";
                    headphonesAndEarphones.ParentCategory = electronics;
                    headphonesAndEarphones.CategoryImageUrl =
                        "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSYQDkf063fyNcUnZ8cqBlV0iJNLSX2H4x2GNdYzV-IOQ&usqp=CAU&ec=48665698";
                    context.Categories.Add(headphonesAndEarphones);

                    // create child category - camera
                    camera.Name = "Camera";
                    camera.Description = "Camera Category";
                    camera.ParentCategory = electronics;
                    camera.CategoryImageUrl =
                        "https://in.canon/media/image/2019/08/27/57604d3575b84323b2f75c125d43dbc4_7_EOS+90D_BK_TheFront_Body.png";
                    context.Categories.Add(camera);

                    // Create child category - Mobile
                    mobileCategory.Name = "Mobile";
                    mobileCategory.Description = "Category for mobile phones";
                    mobileCategory.ParentCategory = electronics;
                    mobileCategory.CategoryImageUrl =
                        "https://pngsource.in/assets/thumbnails/Mobile-PNG-HD-Quality-Pngsource-KIDPSYP2.png";
                    context.Categories.Add(mobileCategory);

                    // Create child category - Charger
                    mobileAccessories.Name = "Charger";
                    mobileAccessories.Description = "Category for mobile phone chargers";
                    mobileAccessories.ParentCategory = electronics;
                    mobileAccessories.CategoryImageUrl =
                        "https://e7.pngegg.com/pngimages/186/881/png-clipart-iphone-4s-iphone-3gs-battery-charger-iphone-electronics-cable.png";
                    context.Categories.Add(mobileAccessories);

                    //parent category of Home Appliances
                    homeAppliances.Name = "Home, Furniture & Appliances";
                    homeAppliances.Description = "Category for Home, Furniture & Appliances";
                    homeAppliances.CategoryImageUrl =
                        "https://img.freepik.com/premium-photo/3d-set-home-appliances-white-background_751108-1072.jpg";
                    context.Categories.Add(homeAppliances);

                    //child category for Home Appliances
                    kitchenAppliances.Name = "Kitchen Appliances & cookware";
                    kitchenAppliances.Description = "Category for Kitchen Appliances & cookware";
                    kitchenAppliances.ParentCategory = homeAppliances;
                    kitchenAppliances.CategoryImageUrl =
                        "https://www.shutterstock.com/image-illustration/kitchen-appliances-blender-toaster-coffee-260nw-366899375.jpg";
                    context.Categories.Add(kitchenAppliances);

                    // Create child category - cookware
                    cookware.Name = "cookware";
                    cookware.Description = "Category for cookware";
                    cookware.ParentCategory = kitchenAppliances;
                    cookware.CategoryImageUrl =
                        "https://media1.popsugar-assets.com/files/thumbor/OECTSb_CKrazWZntSGto1uLcBJw/fit-in/2048xorig/filters:format_auto-!!-:strip_icc-!!-/2017/06/15/055/n/1922195/619fa58e5943244ba3dc51.93655744_Screen_Shot_2017-06-15_at_5.19.40_PM/i/Farberware-Stainless-Steel-10-pc-Cookware-Set.png";
                    context.Categories.Add(cookware);

                    // Create parent category - Fashion & beauty
                    fashionAndBeauty.Name = "Fashion & beauty";
                    fashionAndBeauty.Description = "Category for Fashion & beauty";
                    fashionAndBeauty.CategoryImageUrl =
                        "https://cdn.mos.cms.futurecdn.net/f2dHDcYnaS35LMGGin9vfE.png";
                    context.Categories.Add(fashionAndBeauty);

                    // Create child category - Men's Fashion
                    mensFashion.Name = "Men's Fashion";
                    mensFashion.Description = "Men's fashion";
                    mensFashion.ParentCategory = fashionAndBeauty;
                    mensFashion.CategoryImageUrl =
                        "https://img.freepik.com/premium-photo/men-s-clothing-set-with-oxford-shoes-watch-sunglasses-office-shirt-tie-jacket-isolated-white-background-top-view_107612-80.jpg";
                    context.Categories.Add(mensFashion);

                    // Create child category - Men's Clothing
                    mensClothing.Name = "Men's Clothing";
                    mensClothing.Description = "Category for Men's Clothing";
                    mensClothing.ParentCategory = mensFashion;
                    mensClothing.CategoryImageUrl =
                        "https://png.pngtree.com/element_our/png/20180828/black-t-shirt-mockup-png_72946.jpg";
                    context.Categories.Add(mensClothing);

                    // Create child category - Men's Footwear
                    mensFootwear.Name = "Men's Footwear";
                    mensFootwear.Description = "Category for Men's Footwear";
                    mensFootwear.ParentCategory = mensFashion;
                    mensFootwear.CategoryImageUrl =
                        "https://cdn.shopify.com/s/files/1/0753/2615/products/66913-TAN-1-COPY_2_600x.jpg?v=1641995571";
                    context.Categories.Add(mensFootwear);

                    // Create child category - Men's Watches
                    mensWatches.Name = "Men's Watches";
                    mensWatches.Description = "Category for Men's Watches";
                    mensWatches.ParentCategory = mensFashion;
                    mensWatches.CategoryImageUrl =
                        "https://m.media-amazon.com/images/I/71LS6dJnP5L._AC_UY1000_.jpg";
                    context.Categories.Add(mensWatches);

                    // Create child category - Women's Fashion
                    womensFashion.Name = "Women's Fashion";
                    womensFashion.Description = "Women's fashion";
                    womensFashion.ParentCategory = fashionAndBeauty;
                    womensFashion.CategoryImageUrl =
                        "https://media.istockphoto.com/id/1208148708/photo/polka-dot-summer-brown-dress-suede-wedge-sandals-eco-straw-tote-bag-cosmetics-on-a-light.jpg?s=612x612&w=0&k=20&c=9Y135GYKHLlPotGIfynBbMPhXNbYeuDuFzreL_nfDE8=";
                    context.Categories.Add(womensFashion);

                    // Create child category - Women's Footwear
                    womensFootwear.Name = "Women's Footwear";
                    womensFootwear.Description = "Category for Women's Footwear";
                    womensFootwear.ParentCategory = womensFashion;
                    womensFootwear.CategoryImageUrl =
                        "https://3.imimg.com/data3/NO/EY/MY-1148019/ladies-footwear-500x500.jpg";
                    context.Categories.Add(womensFootwear);

                    // Create child category - beauty
                    beauty.Name = "beauty";
                    beauty.Description = "Category for beauty products";
                    beauty.ParentCategory = womensFashion;
                    beauty.CategoryImageUrl = "https://m.media-amazon.com/images/I/61pGhHj59zL.jpg";
                    context.Categories.Add(beauty);

                    // Create child category - Women's Watches
                    womensWatches.Name = "Women's Watches";
                    womensWatches.Description = "Category for Women's Watches";
                    womensWatches.ParentCategory = womensFashion;
                    womensWatches.CategoryImageUrl =
                        "https://fossil.scene7.com/is/image/FossilPartners/ES5092-alt?$sfcc_fos_medium$";
                    context.Categories.Add(womensWatches);

                    // Create child category - Women's Clothing
                    womensClothing.Name = "Women's Clothing";
                    womensClothing.Description = "Category for Women's Clothing";
                    womensClothing.ParentCategory = womensFashion;
                    womensClothing.CategoryImageUrl =
                        "https://5.imimg.com/data5/CU/AQ/MY-43701257/ladies-designer-top-500x500.jpg";
                    context.Categories.Add(womensClothing);

                    // Create child category - Women's Handbags
                    womensHandBag.Name = "Women's Handbags";
                    womensHandBag.Description = "Category for Women's Handbags";
                    womensHandBag.ParentCategory = womensFashion;
                    womensHandBag.CategoryImageUrl =
                        "https://m.media-amazon.com/images/I/81JpjrM2JES._UY675_.jpg";
                    context.Categories.Add(womensHandBag);

                    // Create parent category - Kids Fashion
                    kidsFashion.Name = "Kids Fashion";
                    kidsFashion.Description = "Kids Fashion";
                    kidsFashion.CategoryImageUrl =
                        "https://media.istockphoto.com/id/1208543231/photo/wooden-toys-clothes-and-shoes-on-green-background.jpg?s=612x612&w=0&k=20&c=LpclhcyyLRBbZGvKy3rorpxZNZfXptRggDP3-pj02QQ=";
                    context.Categories.Add(kidsFashion);

                    // Create child category - Kids Footwear
                    kidsFootwear.Name = "Kids Footwear";
                    kidsFootwear.Description = "Category for Kids Footwear";
                    kidsFootwear.ParentCategory = kidsFashion;
                    kidsFootwear.CategoryImageUrl =
                        "https://3.imimg.com/data3/BS/UB/MY-8243641/kids-footwear-500x500.jpg";
                    context.Categories.Add(kidsFootwear);

                    // Create child category - Kids Clothing
                    kidsClothings.Name = "Kids Clothing";
                    kidsClothings.Description = "Category for Kids Clothing";
                    kidsClothings.ParentCategory = kidsFashion;
                    kidsClothings.CategoryImageUrl =
                        "https://ae01.alicdn.com/kf/Hf68fdfeaae8a486a9349c78d76a1c1805.jpg?width=750&height=750&hash=1500";
                    context.Categories.Add(kidsClothings);

                    // Create child category - Kids Watches
                    kidsWatches.Name = "Kids Watches";
                    kidsWatches.Description = "Category for Kids Watches";
                    kidsWatches.ParentCategory = kidsFashion;
                    kidsWatches.CategoryImageUrl =
                        "https://5.imimg.com/data5/XS/XE/UW/SELLER-26880842/kid-s-watch-500x500.jpg";
                    context.Categories.Add(kidsWatches);

                    context.SaveChanges();
                }

                // Products
                if (!context.Products.Any())
                {
                    context.Products.AddRange(
                        new List<Product>()
                        {
                            new Product()
                            {
                                ProdName = "Charger",
                                Description = "Samsung Galaxy A01 Mobile Charger 2 Amp With Cable",
                                Price = 450,
                                ImageURL =
                                    "https://m.media-amazon.com/images/I/61v+gQmIbvL._SL1024_.jpg",
                                CategoryId = mobileAccessories.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Samsung",
                                Description = "Samsung Galaxy S7 Edge G935F 32GB 64GB 128GB",
                                Price = 10000,
                                ImageURL =
                                    "https://freepngimg.com/thumb/samsung_mobile_phone/5-2-samsung-mobile-phone-png-hd.png",
                                CategoryId = mobileCategory.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Apple",
                                Description =
                                    "iPhone X 64GB/256GB Apple Mobile Smartphone Factory Unlocked | Product ... Apple iPhone 5 32GB Smartphone - T-Mobile - Black",
                                Price = 20000,
                                ImageURL =
                                    "https://m.media-amazon.com/images/I/51zCJOv9AFL._SL1500_.jpg",
                                CategoryId = mobileCategory.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Xiaomi",
                                Description = "Xiaomi Redmi 9 Mobile Phone With 64GB 4GB RAM",
                                Price = 15000,
                                ImageURL =
                                    "https://5.imimg.com/data5/SELLER/Default/2021/6/VM/TI/EU/21445559/redmi-mobile-phones-redmi-8-xiaomi-mobiles-500x500.jpeg",
                                CategoryId = mobileCategory.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Pressure Cooker",
                                Description = "Cuisine art Pressure cooker - upto 40% off",
                                Price = 11000,
                                ImageURL =
                                    "https://st4.depositphotos.com/1005891/25467/i/600/depositphotos_254677412-stock-photo-instant-pot-on-white.jpg",
                                CategoryId = cookware.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Puma T-Shirt",
                                Description = "Puma T-Shirt",
                                Price = 700,
                                ImageURL =
                                    "https://i5.walmartimages.com/asr/90248222-9563-4b82-adc6-f62993dcf2b5_1.a30eca828a22eb04a2152bce6b0b4071.jpeg",
                                CategoryId = mensClothing.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Nike Sports Shoes",
                                Description =
                                    "Shoes Nike Sneakers at - Up to 40% off Top Brands - Wide range of Shoes",
                                Price = 4999,
                                ImageURL =
                                    "https://th.bing.com/th/id/OIP.IfLChAOeW96wX3L1af0FqwHaFK?pid=ImgDet&w=660&h=460&rs=1",
                                CategoryId = mensFootwear.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Women's t-shirt",
                                Description =
                                    "NWT INC International Concepts Women Black Short Sleeve T-Shirt",
                                Price = 1999,
                                ImageURL =
                                    "https://th.bing.com/th/id/OIP.MifSAaMovZHA9EE80uFszgHaJ4?pid=ImgDet&rs=1",
                                CategoryId = womensClothing.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "SparX Shoe",
                                Description = "kids footwear",
                                Price = 999,
                                ImageURL =
                                    "https://th.bing.com/th/id/OIP.kpTU2h7Z89POFJyq5-BWNQHaIq?pid=ImgDet&rs=1",
                                CategoryId = kidsFootwear.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Luis Vuitton",
                                Description = "Luis Vuitton Hand bag",
                                Price = 3999,
                                ImageURL =
                                    "https://th.bing.com/th/id/OIP.eKSJ5pPT7KZ44H3ROT9pigHaG6?pid=ImgDet&rs=1",
                                CategoryId = womensHandBag.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "FirstCry",
                                Description = "FIRSTCRY Baby & Kids Products Online Shopping",
                                Price = 1299,
                                ImageURL =
                                    "https://cdn.fcglcdn.com/brainbees/images/products/438x531/3206249a.jpg",
                                CategoryId = kidsClothings.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "VEPOSE Women's Low Heels",
                                Description =
                                    "VEPOSE Women's Low Heels Closed Toe Pumps Kitten Bridal, Black",
                                Price = 5999,
                                ImageURL =
                                    "https://m.media-amazon.com/images/I/715pmABJm4L._UL1500_.jpg",
                                CategoryId = womensFootwear.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Microwave Oven",
                                Description = "Convection Microwave Oven",
                                Price = 8000,
                                ImageURL =
                                    "https://www.lg.com/in/images/microwave-ovens/md05265523/gallery/MC2886BRUM-Microwave-ovens-Detail-2-view-D-04.jpg",
                                CategoryId = homeAppliances.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Smart TV",
                                Description = "55-Inch Smart LED TV",
                                Price = 35000,
                                ImageURL =
                                    "https://i.gadgets360cdn.com/products/televisions/large/1548155076_832_samsung_65-inch-led-ultra-hd-4k-tv-65nu7100.jpg",
                                CategoryId = homeAppliances.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Laptop",
                                Description = "Gaming Laptop with Intel Core i7",
                                Price = 70000,
                                ImageURL =
                                    "https://m.media-amazon.com/images/I/71GAZomYfyL._SY450_.jpg",
                                CategoryId = homeAppliances.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Headphones",
                                Description = "Wireless Bluetooth Headphones",
                                Price = 1500,
                                ImageURL =
                                    "https://cdn.shopify.com/s/files/1/1676/7297/products/Main-Image_800x.jpg?v=1613022858",
                                CategoryId = headphonesAndEarphones.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Camera",
                                Description = "Digital SLR Camera",
                                Price = 25000,
                                ImageURL =
                                    "https://media.istockphoto.com/id/96826250/photo/digital-camera-with-clipping-path.jpg?s=612x612&w=0&k=20&c=ceAF827zi_UfczajJAWkXowWxdu5tfisZHRoCiSq94w=",
                                CategoryId = camera.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Shoes",
                                Description = "Running Shoes",
                                Price = 2000,
                                ImageURL =
                                    "https://dxkvlfvncvqr8.cloudfront.net/media/images/product/image/sm-706-smrd-1-1667550033.jpg",
                                CategoryId = mensFootwear.CategoryId
                            },
                            new Product()
                            {
                                ProdName = "Handbag",
                                Description = "Women's handbag",
                                Price = 3000,
                                ImageURL =
                                    "https://th.bing.com/th/id/OIP.eKSJ5pPT7KZ44H3ROT9pigHaG6?pid=ImgDet&rs=1",
                                CategoryId = womensHandBag.CategoryId
                            }
                        }
                    );
                    context.SaveChanges();
                }

                // ProductVendors
                // if (!context.ProductVendors.Any())
                // {
                //     Random random = new Random();

                //     for (int i = 1; i <= 19; i++)
                //     {
                //         int randomVendorId = random.Next(1, 3);
                //         int randomPrice = random.Next(100, 10000);
                //         int randomQuantity = random.Next(1, 100);

                //         context.ProductVendors.Add(
                //             new ProductVendor()
                //             {
                //                 ProductId = i,
                //                 VendorId = randomVendorId,
                //                 Price = randomPrice,
                //                 Quantity = randomQuantity
                //             }
                //         );
                //     }

                //     context.SaveChanges();
                // }

                // ProductVendors
                if (!context.ProductVendors.Any())
                {
                    Random random = new Random();

                    for (int i = 1; i <= 10; i++)
                    {
                        int randomPrice = random.Next(100, 10000);
                        int randomQuantity = random.Next(1, 100);

                        context.ProductVendors.Add(
                            new ProductVendor()
                            {
                                ProductId = i,
                                VendorId = 1,
                                Price = randomPrice,
                                Quantity = randomQuantity
                            }
                        );
                    }

                    for (int i = 11; i <= 15; i++)
                    {
                        int randomPrice = random.Next(100, 10000);
                        int randomQuantity = random.Next(1, 100);

                        context.ProductVendors.Add(
                            new ProductVendor()
                            {
                                ProductId = i,
                                VendorId = 2,
                                Price = randomPrice,
                                Quantity = randomQuantity
                            }
                        );
                    }

                    for (int i = 16; i <= 19; i++)
                    {
                        int randomPrice = random.Next(100, 10000);
                        int randomQuantity = random.Next(1, 100);
                        int randomPrice2 = random.Next(100, 10000);
                        int randomQuantity2 = random.Next(1, 100);

                        context.ProductVendors.Add(
                            new ProductVendor()
                            {
                                ProductId = i,
                                VendorId = 1,
                                Price = randomPrice,
                                Quantity = randomQuantity
                            }
                        );

                        context.ProductVendors.Add(
                            new ProductVendor()
                            {
                                ProductId = i,
                                VendorId = 2,
                                Price = randomPrice2,
                                Quantity = randomQuantity2
                            }
                        );
                    }

                    context.SaveChanges();
                }

                // Create dummy data for Cart
                var cart = new Cart { UserId = 1, CartItems = new List<CartItem>() };

                // Add dummy data for CartItem
                var cartItem1 = new CartItem
                {
                    Cart = cart,
                    ProductId = 1, // Dummy product ID
                    Quantity = 2,
                    ProductVendorId = 1, // Dummy product vendor ID
                };

                var cartItem2 = new CartItem
                {
                    Cart = cart,
                    ProductId = 2, // Dummy product ID
                    Quantity = 1,
                    ProductVendorId = 2, // Dummy product vendor ID
                };

                // Add CartItems to the Cart's CartItems list
                cart.CartItems.Add(cartItem1);
                cart.CartItems.Add(cartItem2);

                // Add the Cart and CartItems to the context
                context.Carts.Add(cart);
                context.CartItems.AddRange(cart.CartItems);

                // Save changes to the database
                context.SaveChanges();

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
