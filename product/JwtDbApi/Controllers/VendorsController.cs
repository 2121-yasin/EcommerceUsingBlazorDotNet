using System.Text.Json;
using System.Text.Json.Serialization;
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
            var vendor = new Vendor
            {
                GSTIN = vendorDto.GSTIN,
                UserId = id
                // You can leave Id empty as it will be auto-generated by the database
            };

            // Add the vendor to the database context and save changes
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            // Return the newly created vendor with the generated Id
            return CreatedAtAction(nameof(GetVendorInfo), new { id = vendor.Id }, vendor);
        }

        // GET products by vendor

        // [HttpGet("{id}/products")]
        // public async Task<ActionResult<IEnumerable<Product>>> GetProductsByVendor(int id)
        // {
        //     var options = new JsonSerializerOptions
        //     {
        //         ReferenceHandler = ReferenceHandler.Preserve,
        //         // MaxDepth = 32,
        //         WriteIndented = true
        //     };

        //     var vendor = await _context.Vendors
        //         .Include(v => v.ProductVendors)
        //         .ThenInclude(pv => pv.Product)
        //         .FirstOrDefaultAsync(v => v.Id == id);

        //     if (vendor == null)
        //     {
        //         return NotFound();
        //     }

        //     var products = vendor.ProductVendors.Select(pv => pv.Product).ToList();
        //     var jsonString = JsonSerializer.Serialize(products, options);
        //     return Ok(jsonString);
        // }


        [HttpGet("{Id}/products")]
        public ActionResult<List<Product>> GetProductsByVendor(int Id)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                // MaxDepth = 32,
                WriteIndented = true
            };
            // Retrieve the vendor from the database including the associated products
            Vendor vendor = _context.Vendors
                .Include(v => v.ProductVendors)
                .ThenInclude(pv => pv.Product)
                .FirstOrDefault(v => v.Id == Id);

            if (vendor == null)
            {
                return NotFound(); // Return 404 if vendor is not found
            }

            // Extract the list of products from the vendor's product vendors
            List<Product> products = vendor.ProductVendors
                .Select(pv => pv.Product!)
                .ToList();
            var jsonString = JsonSerializer.Serialize(products, options);
            return Ok(jsonString);
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
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the vendor.");
            }
        }


        private bool VendorExists(int id)
        {
            return _context.Vendors.Any(v => v.Id == id);
        }
    }
}