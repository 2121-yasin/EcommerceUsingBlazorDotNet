using System.Text.Json;
using System.Text.Json.Serialization;
using JwtDbApi.Data;
using JwtDbApi.DTOs;
using JwtDbApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JwtDbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        //GET: api/Product/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<object>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.ProductVendors)
                .Include(p => p.Category)
                .ToListAsync();

            var products1 = products
                .Select(
                    p =>
                        new
                        {
                            p.ProdId,
                            p.ProdName,
                            StockQty = p.GetOverallQuantity(),
                            p.Price,
                            p.ImageURL,
                            p.StartDate,
                            p.Description,
                            Category = p.Category?.Name
                        }
                )
                .ToList();

            return products1;
        }

        // GET: api/Product/byPages
        [HttpGet("byPages")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
            int page = 1,
            int pageSize = 10
        )
        {
            var totalProducts = await _context.Products.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var products = await _context.Products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(
                new
                {
                    TotalProducts = totalProducts,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize,
                    Products = products
                }
            );
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProduct([FromRoute] int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductVendors)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProdId == id);

            if (product == null)
            {
                return NotFound();
            }

            var product1 = new
            {
                product.ProdId,
                product.ProdName,
                StockQty = product.GetOverallQuantity(),
                product.Price,
                product.ImageURL,
                product.StartDate,
                product.Description,
                Category = product.Category?.Name
            };

            return Ok(product1);
        }

        // GET: api/Product/search
        [HttpGet("search")]
        public IActionResult SearchProductsByName([FromQuery] string name)
        {
            var searchResults = _context.Products.Where(p => p.ProdName.Contains(name)).ToList();

            return Ok(searchResults);
        }

        // Post api/Products
        [HttpPost]
        [Authorize(Roles = "Admin,Vendor")]
        public ActionResult PostNewProduct([FromBody] ProductDto productDto)
        {
            // Work in progress in vendor side
            return Ok();
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProdId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        [HttpPut("{prodId}/postdetails")]
        public async Task<IActionResult> PutProductDetails(int prodId, [FromBody] object details)
        {
            var product = await _context.Products.FindAsync(prodId);
            if (product == null)
            {
                return NotFound();
            }

            product.BasicDetails = details.ToString();
            await _context.SaveChangesAsync();

            return Ok(
                new
                {
                    product.ProdId,
                    product.ProdName,
                    descrption = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                        product.BasicDetails
                    ),
                    product.ImageURL,
                }
            );
        }

        [HttpGet("{prodId}/getdetails")]
        public async Task<IActionResult> GetProductDetails(int prodId)
        {
            var product = await _context.Products.FindAsync(prodId);
            if (product == null)
            {
                return NotFound();
            }

            var details = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                product.BasicDetails
            );

            return Ok(details);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProdId == id);
        }

//get productvendorid
        [HttpGet("{productId}/vendor")]
        public ActionResult<ProductVendor> GetProductVendor(int productId)
        {
            var vendor = _context.ProductVendors.FirstOrDefault(v => v.ProductId == productId);
            if (vendor == null)
            {
                return NotFound();
            }

            return vendor;
        }

    }
}
