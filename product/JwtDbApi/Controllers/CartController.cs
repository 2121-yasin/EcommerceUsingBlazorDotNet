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


        [HttpDelete("RemoveFromCart/{cartItemId}")]
        public async Task<ActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);

            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok("Item successfully removed from the cart.");
        }



        // Create new cart if cart for a user is not created
        // POST: api/cart/{userId}
        // [HttpPost("{userId}")]
        // public async Task<ActionResult> AddToCart(int userId)
        // {
        //     var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        //     if (cart == null)
        //     {
        //         var newCart = new Cart
        //         {
        //             UserId = userId
        //         };
        //         _context.Carts.Add(newCart);
        //         await _context.SaveChangesAsync(); // Save changes to the database
        //         return Ok(newCart.Id);
        //     }

        //     return BadRequest("Cart already exists");
        // }


        //     userId, productId, and productVendorId. The userId parameter is used to find the cart associated with the user, or create a new cart if it doesn't exist.

        // Once the cart is retrieved or created, a new CartItem entity is created with the provided productId and productVendorId, and the CartId is set to the ID of the cart. The CartItem is then added to the context and saved to the database.

        // POST: api/cart/{userId}
        // [HttpPost("{userId}")]
        // public async Task<ActionResult> AddToCart(int userId, int productId, int productVendorId)
        // {
        //     var cart = await _context.Carts
        //         .Include(c => c.CartItems)
        //         .FirstOrDefaultAsync(c => c.UserId == userId);

        //     if (cart == null)
        //     {
        //         var newCart = new Cart
        //         {
        //             UserId = userId
        //         };
        //         _context.Carts.Add(newCart);
        //         await _context.SaveChangesAsync();

        //         cart = newCart; // Assign the newly created cart to the 'cart' variable
        //     }

        //     var cartItem = new CartItem
        //     {
        //         CartId = cart.Id,
        //         ProductId = productId,
        //         ProductVendorId = productVendorId
        //     };

        //     _context.CartItems.Add(cartItem);
        //     await _context.SaveChangesAsync();

        //     return Ok(cartItem);
        // }


        [HttpPost("{userId}")]
        public async Task<ActionResult> AddToCart(int userId, int productId, int productVendorId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                var newCart = new Cart
                {
                    UserId = userId
                };
                _context.Carts.Add(newCart);
                await _context.SaveChangesAsync();

                cart = newCart; // Assign the newly created cart to the 'cart' variable
            }

            // Check if the product is already present in the cart
            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingCartItem != null)
            {
                return BadRequest("Item already present in the cart.");
            }

            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                ProductVendorId = productVendorId
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return Ok("Item successfully added to the cart.");
        }




    }
}
