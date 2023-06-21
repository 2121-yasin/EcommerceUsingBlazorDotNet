using System.Linq;
using JwtDbApi.Data;
using JwtDbApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtDbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public ActionResult<Cart> GetCart(int userId)
        {
            // Retrieve the cart based on the user ID
            var cart = _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVendor)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                // Cart not found for the user
                return NotFound();
            }

            var response = new
            {
                cart.Id,
                cart.UserId,
                cart.TotalPrice,
                products = cart.CartItems.Select(ci => new
                {
                    ci.Product.ProdName,
                    ci.Product.ImageURL,
                    ci.Product.Description,
                    price = ci.ProductVendor.Price,
                    quantity = ci.ProductVendor.Quantity
                }

                )
            };

            return Ok(response);
        }


        // Create new cart if cart for a user is not created
        // POST: api/cart/{userId}
        [HttpPost("{userId}")]
        public async Task<ActionResult> AddToCart(int userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                var newCart = new Cart
                {
                    UserId = userId
                };
                _context.Carts.Add(newCart);
                await _context.SaveChangesAsync(); // Save changes to the database
                return Ok(newCart.Id);
            }

            return BadRequest("Cart already exists");
        }

    }
}
