using JwtDbApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JwtDbApi.DTOs;
using JwtDbApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace JwtDbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Vendor")]
    public class ProductVendorController : ControllerBase
    {
        private readonly ILogger<ProductVendorController> _logger;
        private readonly AppDbContext _context;

        public ProductVendorController(
            ILogger<ProductVendorController> logger,
            AppDbContext context
        )
        {
            _context = context;
            _logger = logger;
        }

        // PUT: api/ProductVendor/{vendorId}
        [HttpPut("{vendorId}")]
        public async Task<IActionResult> PutProductInfo(
            int prodId,
            int vendorId,
            [FromBody] ProductVendorDto productVendorDto
        )
        {
            var productVendor = _context.ProductVendors.FirstOrDefault(
                pv => pv.ProductId == prodId && pv.VendorId == vendorId
            );
            var product = _context.Products.FirstOrDefault(p => p.ProdId == prodId);

            if (product != null)
            {
                // Update product information
                product.ProdName = productVendorDto.Product.ProdName;
                product.Description = productVendorDto.Product.Description;
                product.ImageURL = productVendorDto.Product.ImageURL;
                await _context.SaveChangesAsync();
            }

            if (productVendor != null)
            {
                // Update product vendor information
                productVendor.Price = productVendorDto.Price;
                productVendor.Quantity = productVendorDto.Quantity;
                await _context.SaveChangesAsync();

                return Ok();
            }

            return NotFound();
        }

        // GET: api/ProductVendor/{vendorId}
        // Returns product vendor information if it exists
        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetProductVendor(int vendorId, int productId)
        {
            var productVendor = _context.ProductVendors.FirstOrDefault(
                pv => pv.ProductId == productId && pv.VendorId == vendorId
            );

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProdId == productId);

            if (product == null)
            {
                return NotFound();
            }

            if (productVendor == null)
            {
                return Ok(
                    new
                    {
                        productVendor = new
                        {
                            Price = 0,
                            Quantity = 0,
                            Visible = 1,
                            Product = new
                            {
                                product.ProdId,
                                product.ProdName,
                                product.Description,
                                product.ImageURL,
                                product.StartDate,
                                Category = product.Category.Name
                            }
                        }
                    }
                );
            }

            if (productVendor != null)
            {
                // Find the productVendor with the same vendor ID and product ID
                return Ok(
                    new
                    {
                        message = "Product is already listed by you",
                        productVendor = new
                        {
                            productVendor.Price,
                            productVendor.Quantity,
                            productVendor.Visible,
                            Product = new
                            {
                                product.ProdId,
                                product.ProdName,
                                product.Description,
                                product.ImageURL,
                                product.StartDate,
                                Category = product.Category.Name
                            }
                        }
                    }
                );
            }

            return NotFound();
        }

        // GET: api/productvendor/category/{categoryId} - Cannot sort with vendor name. But is efficient as sorting and pagination is done in database directly
        // [HttpGet("category/{categoryId}")]
        // public async Task<IActionResult> GetProductVendor(int categoryId, int page = 1, int pageSize = 10, string sortBy = "ProductName", bool sortDesc = false)
        // {
        //     try
        //     {
        //         var query = _context.Products
        //             .Where(product => product.CategoryId == categoryId)
        //             .Include(product => product.ProductVendors)
        //             .ThenInclude(productVendor => productVendor.Vendor)
        //             .SelectMany(product => product.ProductVendors, (product, productVendor) => new
        //             {
        //                 UniqueId = Guid.NewGuid(), // Generate a unique identifier since ProductId is duplicated cause of flattened data.
        //                 ProductId = product.ProdId,
        //                 ProductName = product.ProdName,
        //                 ProductBasePrice = product.Price,
        //                 ProductImageUrl = product.ImageURL,
        //                 ProductVendorId = productVendor.Id,
        //                 ProductVendorListedOn = productVendor.ListedOn,
        //                 ProductVendorPrice = productVendor.Price,
        //                 ProductVendorQuantity = productVendor.Quantity,
        //                 ProductVendorVisible = productVendor.Visible,
        //                 VendorId = productVendor.Vendor.Id,
        //                 VendorUserId = productVendor.Vendor.UserId
        //             });

        //         // Apply sorting
        //         switch (sortBy.ToLower())
        //         {
        //             case "productname":
        //                 query = sortDesc ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName);
        //                 break;
        //             case "productbaseprice":
        //                 query = sortDesc ? query.OrderByDescending(p => p.ProductBasePrice) : query.OrderBy(p => p.ProductBasePrice);
        //                 break;
        //             default:
        //                 break;
        //         }

        //         // Total count before pagination
        //         var totalItems = await query.CountAsync();

        //         // Calculate total pages and ensure page is within range
        //         int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        //         page = Math.Max(1, Math.Min(page, totalPages));

        //         // Query with pagination
        //         var products = await query
        //             .Skip((page - 1) * pageSize)
        //             .Take(pageSize)
        //             .ToListAsync();

        //         // Create the response object
        //         var response = new
        //         {
        //             TotalItems = totalItems,
        //             TotalPages = totalPages,
        //             Page = page,
        //             PageSize = pageSize,
        //             SortBy = sortBy,
        //             SortDesc = sortDesc,
        //             Data = products
        //         };

        //         return Ok(response);
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }

        // GET: api/productvendor/category/{categoryId} - Can sort with vendor name. But is not efficient as all records are fetched from database and then sorting and pagination is done
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductVendor(int categoryId, int page = 1, int pageSize = 10, string sortBy = "ProductName", bool sortDesc = false)
        {
            try
            {
                var query = _context.Products
                    .Where(product => product.CategoryId == categoryId)
                    .Include(product => product.ProductVendors)
                    .ThenInclude(productVendor => productVendor.Vendor)
                    .SelectMany(product => product.ProductVendors, (product, productVendor) => new
                    {
                        UniqueId = Guid.NewGuid(),
                        ProductId = product.ProdId,
                        ProductName = product.ProdName,
                        ProductBasePrice = product.Price,
                        ProductImageUrl = product.ImageURL,
                        ProductVendorId = productVendor.Id,
                        ProductVendorListedOn = productVendor.ListedOn,
                        ProductVendorPrice = productVendor.Price,
                        ProductVendorQuantity = productVendor.Quantity,
                        ProductVendorVisible = productVendor.Visible,
                        VendorId = productVendor.Vendor.Id,
                        VendorUserId = productVendor.Vendor.UserId
                    });

                var vendorUserIds = query.Select(p => p.VendorUserId).Distinct().ToArray();

                var httpClientHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                var httpClient = new HttpClient(httpClientHandler);
                var apiUrl = "https://localhost:7240/api/UserInfo";

                // Extract the JWT token from the incoming request
                var incomingToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

                // Include the extracted JWT token in the outgoing request
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", incomingToken);

                var secondApiResponse = await httpClient.GetAsync($"{apiUrl}/vendors?vendorUserIds={string.Join(",", vendorUserIds)}");

                if (secondApiResponse.IsSuccessStatusCode)
                {
                    var vendorUsers = await secondApiResponse.Content.ReadAsAsync<List<VendorUserInfoDTO>>();

                    // var productsWithVendorInfo = query.Join(vendorUsers, p => p.VendorUserId, v => v.UserId, (p, v) => new
                    // {
                    //     p.UniqueId,
                    //     p.ProductId,
                    //     p.ProductName,
                    //     p.ProductBasePrice,
                    //     p.ProductImageUrl,
                    //     p.ProductVendorId,
                    //     p.ProductVendorListedOn,
                    //     p.ProductVendorPrice,
                    //     p.ProductVendorQuantity,
                    //     p.ProductVendorVisible,
                    //     p.VendorId,
                    //     p.VendorUserId,
                    //     VendorName = v.UserName
                    // });

                    // var productsWithVendorInfo = query.Select(p => new
                    // {
                    //     p.UniqueId,
                    //     p.ProductId,
                    //     p.ProductName,
                    //     p.ProductBasePrice,
                    //     p.ProductImageUrl,
                    //     p.ProductVendorId,
                    //     p.ProductVendorListedOn,
                    //     p.ProductVendorPrice,
                    //     p.ProductVendorQuantity,
                    //     p.ProductVendorVisible,
                    //     p.VendorId,
                    //     p.VendorUserId,
                    //     VendorName = vendorUsers.FirstOrDefault(v => v.UserId == p.VendorUserId).UserName
                    // });

                    var productsWithVendorInfo = query.Select(p => new
                    {
                        p.UniqueId,
                        p.ProductId,
                        p.ProductName,
                        p.ProductBasePrice,
                        p.ProductImageUrl,
                        p.ProductVendorId,
                        p.ProductVendorListedOn,
                        p.ProductVendorPrice,
                        p.ProductVendorQuantity,
                        p.ProductVendorVisible,
                        p.VendorId,
                        p.VendorUserId
                    })
                    .AsEnumerable() // Switch to client-side evaluation
                    .Select(p => new
                    {
                        p.UniqueId,
                        p.ProductId,
                        p.ProductName,
                        p.ProductBasePrice,
                        p.ProductImageUrl,
                        p.ProductVendorId,
                        p.ProductVendorListedOn,
                        p.ProductVendorPrice,
                        p.ProductVendorQuantity,
                        p.ProductVendorVisible,
                        p.VendorId,
                        p.VendorUserId,
                        VendorName = vendorUsers.FirstOrDefault(v => v.UserId == p.VendorUserId)?.UserName
                    });

                    switch (sortBy.ToLower())
                    {
                        case "productname":
                            productsWithVendorInfo = sortDesc ? productsWithVendorInfo.OrderByDescending(p => p.ProductName) : productsWithVendorInfo.OrderBy(p => p.ProductName);
                            break;
                        case "productbaseprice":
                            productsWithVendorInfo = sortDesc ? productsWithVendorInfo.OrderByDescending(p => p.ProductBasePrice) : productsWithVendorInfo.OrderBy(p => p.ProductBasePrice);
                            break;
                        case "vendorname":
                            productsWithVendorInfo = sortDesc ? productsWithVendorInfo.OrderByDescending(p => p.VendorName) : productsWithVendorInfo.OrderBy(p => p.VendorName);
                            break;
                        case "productvendorprice":
                            productsWithVendorInfo = sortDesc ? productsWithVendorInfo.OrderByDescending(p => p.ProductVendorPrice) : productsWithVendorInfo.OrderBy(p => p.ProductVendorPrice);
                            break;
                        case "productvendorquantity":
                            productsWithVendorInfo = sortDesc ? productsWithVendorInfo.OrderByDescending(p => p.ProductVendorQuantity) : productsWithVendorInfo.OrderBy(p => p.ProductVendorQuantity);
                            break;
                        case "productvendorvisible":
                            productsWithVendorInfo = sortDesc ? productsWithVendorInfo.OrderByDescending(p => p.ProductVendorVisible) : productsWithVendorInfo.OrderBy(p => p.ProductVendorVisible);
                            break;
                        default:
                            break;
                    }

                    var products = productsWithVendorInfo.ToList(); // Execute the query to retrieve all products

                    var totalItems = products.Count;

                    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                    page = Math.Max(1, Math.Min(page, totalPages));

                    var paginatedProducts = products
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList(); // Retrieve the products for the current page

                    var responseObj = new
                    {
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        Page = page,
                        PageSize = pageSize,
                        SortBy = sortBy,
                        SortDesc = sortDesc,
                        Data = paginatedProducts
                    };

                    return Ok(responseObj);
                }
                else
                {
                    var errorMessage = await secondApiResponse.Content.ReadAsStringAsync();
                    return StatusCode((int)secondApiResponse.StatusCode, errorMessage);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // POST: api/ProductVendor/{vendorId}
        // To add a new product to the listing of a vendor from the available products
        [HttpPost("{vendorId}")]
        public async Task<IActionResult> PostProductVendor(
            int vendorId,
            [FromBody] ProductVendorDto productVendorDto
        )
        {
            var productVendor = _context.ProductVendors.FirstOrDefault(
                pv => pv.ProductId == productVendorDto.ProductId && pv.VendorId == vendorId
            );

            if (productVendor == null)
            {
                // Create a new ProductVendor instance with the provided data
                var newProductVendor = new ProductVendor
                {
                    ProductId = productVendorDto.ProductId,
                    VendorId = vendorId,
                    Price = productVendorDto.Price,
                    Quantity = productVendorDto.Quantity,
                    Visible = 1,
                };

                // Add the new ProductVendor instance to the database
                _context.ProductVendors.Add(newProductVendor);
                await _context.SaveChangesAsync();

                return Ok();
            }
            return NotFound();
        }

        // PUT
        // update visibilty of the product
        [HttpPut("visibility/{vendorId}")]
        public async Task<IActionResult> UpdateProductVisibility(
            int vendorId,
            int prodId,
            [FromBody] int visibility
        )
        {
            var productVendor = _context.ProductVendors.FirstOrDefault(
                pv => pv.ProductId == prodId && pv.VendorId == vendorId
            );

            if (productVendor != null)
            {
                productVendor.Visible = visibility;
                await _context.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        // GET: api/ProductVendor/low-stock/{vendorId}
        [HttpGet("low-stock/{vendorId}")]
        public IActionResult GetLowStockProducts(int vendorId)
        {
            var lowStockProducts = _context.ProductVendors
                .Where(pv => pv.VendorId == vendorId && pv.Quantity < 10)
                .Select(
                    pv =>
                        new
                        {
                            pv.ProductId,
                            pv.Product.ProdName,
                            pv.Product.ImageURL,
                            pv.Quantity
                        }
                )
                .ToList();

            return Ok(lowStockProducts);
        }
    }
}
