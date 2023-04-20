using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Landing()
        {
            var products = await new ProductsController().GetProducts();
            return View(products);
        }
    }
}