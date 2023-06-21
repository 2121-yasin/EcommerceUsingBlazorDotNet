using JwtDbApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JwtDbApi.DTOs;
using JwtDbApi.Models;
using Microsoft.EntityFrameworkCore;

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

        // GET: api/productvendor/category/{categoryId}
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
                        UniqueId = Guid.NewGuid(), // Generate a unique identifier since ProductId is duplicated cause of flattened data.
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

                // Apply sorting
                switch (sortBy.ToLower())
                {
                    case "productname":
                        query = sortDesc ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName);
                        break;
                    case "productbaseprice":
                        query = sortDesc ? query.OrderByDescending(p => p.ProductBasePrice) : query.OrderBy(p => p.ProductBasePrice);
                        break;
                    default:
                        break;
                }

                // Total count before pagination
                var totalItems = await query.CountAsync();

                // Calculate total pages and ensure page is within range
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                page = Math.Max(1, Math.Min(page, totalPages));

                // Query with pagination
                var products = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Create the response object
                var response = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Page = page,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortDesc = sortDesc,
                    Data = products
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
