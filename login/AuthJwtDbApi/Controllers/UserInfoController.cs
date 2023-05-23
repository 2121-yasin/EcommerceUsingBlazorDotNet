using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthJwtDbApi.Data;
using AuthJwtDbApi.DTOs;
using AuthJwtDbApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthJwtDbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserInfoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UserInfo
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetUserInfo()
        {
            return await _context.UserInfo.Include(u => u.Address).ToListAsync();
        }

        // GET: api/UserInfo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserInfo>> GetUserInfo(int id)
        {
            var userInfo = await _context.UserInfo.Include(u => u.Address).FirstOrDefaultAsync(u => u.UserId == id);

            if (userInfo == null)
            {
                return NotFound();
            }

            return userInfo;
        }

        // PUT: api/UserInfo/5
        [HttpPut("{id}")]
        [Authorize(Roles = "User,Vendor")]
        public async Task<IActionResult> PutUserInfo(int id, UserInfo userInfo)
        {
            if (id != userInfo.UserId)
            {
                return BadRequest();
            }

            _context.Entry(userInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserInfoExists(id))
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

        // POST: api/UserInfo
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserRegistrationDto>> PostUserInfo(UserRegistrationDto userRegistrationDto)
        {
            var existingUser = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == userRegistrationDto.Email);

            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password);

            if (userRegistrationDto.Role != "User" && userRegistrationDto.Role != "Vendor")
            {
                return BadRequest();
            }
            
            AddressInfo address = new AddressInfo
            {
                Street = userRegistrationDto.Street,
                City = userRegistrationDto.City,
                State = userRegistrationDto.State,
                Pincode = userRegistrationDto.Pincode
            };

            UserInfo userInfo = new UserInfo
            {
                Email = userRegistrationDto.Email,
                Password = passwordHash,
                UserName = userRegistrationDto.UserName,
                Phone = userRegistrationDto.Phone,
                Address = address,
                Role = userRegistrationDto.Role
            };

            _context.UserInfo.Add(userInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserInfo", new { id = userInfo.UserId }, userInfo);
        }

        // DELETE: api/UserInfo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserInfo>> DeleteUserInfo(int id)
        {
            var userInfo = await _context.UserInfo.FindAsync(id);
            if (userInfo == null)
            {
                return NotFound();
            }

            _context.UserInfo.Remove(userInfo);
            await _context.SaveChangesAsync();

            return userInfo;
        }

        [AllowAnonymous]
        // GET: api/UserInfo/RedirectUrl
        [HttpGet("RedirectUrl")]
        public async Task<ActionResult<string>> GetClientRedirectUrl(Guid clientId)
        {
            var RedirectUrl = await _context.ClientProfile.Where(e => e.ClientId == clientId).Select(c => c.RedirectUrl).FirstOrDefaultAsync();

            if (RedirectUrl == null)
            {
                return NotFound();
            }

            return RedirectUrl.ToString();
        }


        private bool UserInfoExists(int id)
        {
            return _context.UserInfo.Any(e => e.UserId == id);
        }
    }


}