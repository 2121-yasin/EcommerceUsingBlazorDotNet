using JwtDbApi.Data;
using JwtDbApi.DTOs;
using JwtDbApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtDbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Vendor")]
    public class VendorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendorsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendorInfo(int id)
        {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.UserId == id);

            if (vendor == null)
            {
                return NotFound();
            }

            return vendor;
        }

        // POST
        // api/vendors/id
        [Authorize(Roles = "Vendor")]
        [HttpPost("{id}")]
        public async Task<IActionResult> CreateVendor(int id, [FromBody] VendorDto vendorDto)
        {
            // Create a new Vendor instance with the provided data
            var vendor = new Vendor { GSTIN = vendorDto.GSTIN, UserId = id };

            // Add the vendor to the database context and save changes
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            // Return the newly created vendor with the generated Id
            return CreatedAtAction(nameof(GetVendorInfo), new { id = vendor.Id }, vendor);
        }

        // GET products by vendor

        // [HttpGet("{Id}/products")]
        // public async Task<ActionResult<List<Product>>> GetProductsByVendor(int Id)
        // {
        //     var products = await (from pv in _context.ProductVendors
        //                           join p in _context.Products on pv.ProductId equals p.ProdId
        //                           where pv.VendorId == Id
        //                           select new Product
        //                           {
        //                               ProdId = p.ProdId,
        //                               ProdName = p.ProdName,
        //                               // Add other properties you want to include
        //                           }).ToListAsync();

        //     return products;
        // }


        // [HttpGet("{Id}/products")]
        // public async Task<ActionResult<List<Product>>> GetProductsByVendor(int Id)
        // {
        //     var productIds = await _context.ProductVendors
        //                                    .Where(v => v.VendorId == Id)
        //                                    .Select(v => v.ProductId)
        //                                    .ToListAsync();

        //     var products = await _context.Products
        //                                 .Where(p => productIds.Contains(p.ProdId))
        //                                 .Select(p => new Product
        //                                 {
        //                                     ProdId = p.ProdId,
        //                                     ProdName = p.ProdName,
        //                                     // Add other properties you want to include
        //                                 })
        //                                 .ToListAsync();

        //     return products;
        // }



        // GET products by vendor

        [HttpGet("{id}/products")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductsByVendor(
            int id,
            int page = 1,
            int pageSize = 10,
            string? sortBy = null,
            string? sortOrder = null
        )
        {
            var query = _context.Vendors
                .Where(v => v.Id == id)
                .Include(v => v.ProductVendors)
                .ThenInclude(pv => pv.Product)
                .SelectMany(
                    v =>
                        v.ProductVendors.Select(
                            pv =>
                                new
                                {
                                    Vendor = v,
                                    Product = pv.Product,
                                    Price = pv.Price,
                                    Quantity = pv.Quantity,
                                    Visibility = pv.Visible
                                }
                        )
                );

            // Apply sorting if sortBy and sortOrder are provided
            if (sortBy != null && sortOrder != null)
            {
                switch (sortBy.Trim().ToLower())
                {
                    case "name":

                        if (sortOrder == "asc")
                        {
                            query = query.OrderBy(vp => vp.Product.ProdName);
                        }
                        else if (sortOrder.ToLower() == "desc")
                        {
                            query = query.OrderByDescending(vp => vp.Product.ProdName);
                        }
                        break;
                    case "visibility":
                        if (sortOrder.ToLower() == "asc")
                        {
                            query = query.OrderByDescending(vp => vp.Visibility);
                        }
                        else if (sortOrder.ToLower() == "desc")
                        {
                            query = query.OrderBy(vp => vp.Visibility);
                        }
                        break;
                    case "price":
                        if (sortOrder.ToLower() == "asc")
                        {
                            query = query.OrderBy(vp => vp.Price);
                        }
                        else if (sortOrder.ToLower() == "desc")
                        {
                            query = query.OrderByDescending(vp => vp.Price);
                        }
                        break;
                    case "quantity":
                        if (sortOrder.ToLower() == "asc")
                        {
                            query = query.OrderBy(vp => vp.Quantity);
                        }
                        else if (sortOrder.ToLower() == "desc")
                        {
                            query = query.OrderByDescending(vp => vp.Quantity);
                        }
                        break;
                    default:
                        // Handle other sorting criteria if needed
                        break;
                }
            }

            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var products = await query
                .Select(
                    vp =>
                        new
                        {
                            Product = new
                            {
                                vp.Product.ProdId,
                                vp.Product.ProdName,
                                vp.Product.Description,
                                vp.Product.ImageURL,
                                vp.Product.StartDate,
                                vp.Product.Price,
                                Category = vp.Product.Category.Name
                            },
                            Price = vp.Price,
                            Quantity = vp.Quantity,
                            Visibility = vp.Visibility
                        }
                )
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var listedProductCount = await _context.ProductVendors.CountAsync(
                pv => pv.VendorId == id && pv.Visible == 1
            );

            var notListedProductCount = await _context.ProductVendors.CountAsync(
                pv => pv.VendorId == id && pv.Visible == 0
            );

            return Ok(
                new
                {
                    TotalProducts = totalProducts,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize,
                    Products = products,
                    ActiveListings = listedProductCount,
                    InactiveListings = notListedProductCount
                }
            );
        }

        // PUT
        [Authorize(Roles = "Vendor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendorInfo(int id, [FromBody] VendorDto vendorDto)
        {
            // Find the vendor by ID
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound(); // Vendor not found
            }

            // Update the vendor's fields
            vendor.GSTIN = vendorDto.GSTIN;
            vendor.DeliveryPinCode = vendorDto.DeliveryPinCode;

            try
            {
                // Save the changes to the database
                await _context.SaveChangesAsync();
                return Ok(); // Success
            }
            catch (DbUpdateException)
            {
                // Handle any exception that occurs during database update
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while updating the vendor."
                );
            }
        }

        private bool VendorExists(int id)
        {
            return _context.Vendors.Any(v => v.Id == id);
        }
    }
}
