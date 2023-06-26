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

        // GET: api/UserInfo/vendors
        // [HttpGet("vendors")]
        // [Authorize(Roles = "Admin")]
        // public async Task<ActionResult<IEnumerable<VendorUserInfoDTO>>> GetVendorUserDetails([FromQuery(Name = "vendorUserIds[]")] int[] vendorUserIds)
        // {
        //     try
        //     {
        //         var vendorUsers = await _context.UserInfo
        //             .Where(u => vendorUserIds.Contains(u.UserId))
        //             .Include(u => u.Address)
        //             .Select(u => new VendorUserInfoDTO
        //             {
        //                 UserId = u.UserId,
        //                 UserName = u.UserName,
        //                 Email = u.Email,
        //                 Phone = u.Phone,
        //                 Address = u.Address
        //             })
        //             .ToListAsync();

        //         return Ok(vendorUsers);
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }

        // GET: api/UserInfo/vendors
        [HttpGet("vendors")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<VendorUserInfoDTO>>> GetVendorUserDetails([FromQuery(Name = "vendorUserIds")] string vendorUserIdsString)
        {
            try
            {
                var vendorUserIds = vendorUserIdsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();

                var vendorUsers = await _context.UserInfo
                    .Where(u => vendorUserIds.Contains(u.UserId))
                    .Include(u => u.Address)
                    .Select(u => new VendorUserInfoDTO
                    {
                        UserId = u.UserId,
                        UserName = u.UserName,
                        Email = u.Email,
                        Phone = u.Phone,
                        Address = u.Address
                    })
                    .ToListAsync();

                return Ok(vendorUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET Users by pages with sorting
        [HttpGet("paginated-users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetUserInfo(int page = 1, int pageSize = 10, string sortBy = "UserName", bool sortDesc = false)
        {
            // Get count of total users
            int totalItems = await _context.UserInfo.CountAsync();

            // Calculate total pages and ensure page is within range
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));

            // Calculate skip count for pagination
            int skipCount = (page - 1) * pageSize;

            // Apply sorting dynamically
            IQueryable<UserInfo> query = _context.UserInfo.AsQueryable();
            query = ApplySorting(query, sortBy, sortDesc);

            // Retrieve paginated users
            List<UserInfo> users = await query.Skip(skipCount).Take(pageSize).ToListAsync();

            // Create response object
            var response = new
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Data = users
            };

            return Ok(response);
        }

        // Apply sorting to users
        private IQueryable<UserInfo> ApplySorting(IQueryable<UserInfo> query, string sortBy, bool sortDesc)
        {
            switch (sortBy)
            {
                case "UserId":
                    query = sortDesc ? query.OrderByDescending(u => u.UserId) : query.OrderBy(u => u.UserId);
                    break;
                case "UserName":
                    query = sortDesc ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName);
                    break;
                case "Email":
                    query = sortDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email);
                    break;
                case "Role":
                    query = sortDesc ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role);
                    break;
                default:
                    break;
            }

            return query;
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