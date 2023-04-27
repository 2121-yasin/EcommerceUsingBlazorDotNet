using JwtDbApi.Data;
using JwtDbApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtDbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendorsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Vendor>> GetVendorInfo(int id)
        {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.UserId == id);

            if (vendor == null)
            {
                return NotFound();
            }

            return vendor;
        }

    }
}