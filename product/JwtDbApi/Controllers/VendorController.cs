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
    public class VendorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VendorController(ApplicationDbContext context)
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