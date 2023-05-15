using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtDbApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtDbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }





        // GET: api/Products
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Products>> GetProducts(int id)
        {
            var products = await _context.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }

            return products;
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducts(int id, Products products)
        {
            if (id != products.ProdId)
            {
                return BadRequest();
            }

            _context.Entry(products).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductsExists(id))
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

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Products>> PostProducts(Products products)
        {
            _context.Products.Add(products);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducts", new { id = products.ProdId }, products);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Products>> DeleteProducts(int id)
        {
            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }

            _context.Products.Remove(products);
            await _context.SaveChangesAsync();

            return products;
        }



        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProdId == id);
        }


//         //offerCode
//         [HttpPost]
// public IActionResult Checkout(string offerCode)
// {
//     using (var db = new ApplicationDbContext())
//     {
//         var validOfferCode = db.OfferCodes.FirstOrDefault(c => c.Code == offerCode && c.UsedCount < c.MaxUses);
//         if (validOfferCode != null)
//         {
//             // Apply the offer code discount to the order total
//             var orderTotal = // get the order total from your database or model
//             var discount = (orderTotal * validOfferCode.DiscountPercentage) / 100;
//             var discountedTotal = orderTotal - discount;
//             // Update the order total on the checkout page
//             ViewBag.OrderTotal = discountedTotal;
//             // Update the offer code usage count in the database
//             validOfferCode.UsedCount++;
//             db.SaveChanges();
//         }
//         else
//         {
//             // Display an error message to the user
//             ViewBag.Error = "Invalid offer code";
//         }
//     }
//     return View();
// }


        
    }
}