using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JwtDbApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JwtDbApi.DTOs;
using JwtDbApi.Models;

namespace JwtDbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Vendor")]
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

        // make a controller to post into productvendors tables a new entry,
        // the controller will accept a productvendor object and a vendor id and a product id
        // if the productvendor object is not null, then add it to the database
        // if a entry with the same vendor id and product id already exists, then return a bad request with a message

        // POST: api/ProductVendor/{vendorId}
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
                    Visible = 1
                };

                // Add the new ProductVendor instance to the database
                _context.ProductVendors.Add(newProductVendor);
                await _context.SaveChangesAsync();

                return Ok();
            }
            else
            {
                // find the productvendor with the same vendor id and product id
                // if it exists, then return the productvendor object along with a message

                return Ok(productVendor);
            }
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
