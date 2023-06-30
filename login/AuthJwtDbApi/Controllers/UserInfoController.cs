using System.Linq.Dynamic.Core;
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
        public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetUserInfo()
        {
            var userInfo = await _context.UserInfo.Include(u => u.Address).ToListAsync();

            var userInfoDto = userInfo.Select(u => ConvertToUserInfoDto(u));

            return Ok(userInfoDto);
        }

        // GET: api/UserInfo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserInfoDto>> GetUserInfo(int id)
        {
            var userInfo = await _context.UserInfo.Include(u => u.Address).FirstOrDefaultAsync(u => u.UserId == id);

            if (userInfo == null)
            {
                return NotFound();
            }

            var userInfoDto = ConvertToUserInfoDto(userInfo);

            return Ok(userInfoDto);
        }

        // GET: api/UserInfo/vendors    -   Not used anymore as vendor name added in product application
        // [HttpGet("vendors")]
        // [Authorize(Roles = "Admin")]
        // public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetVendorUserDetails([FromQuery(Name = "vendorUserIds[]")] int[] vendorUserIds)
        // {
        //     try
        //     {
        //         var vendorUsers = await _context.UserInfo
        //             .Where(u => vendorUserIds.Contains(u.UserId))
        //             .Include(u => u.Address)
        //             .Select(u => new UserInfoDto
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

        // GET: api/UserInfo/vendors    -   Not used anymore as vendor name added in product application
        // [HttpGet("vendors")]
        // [Authorize(Roles = "Admin")]
        // public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetVendorUserDetails([FromQuery(Name = "vendorUserIds")] string vendorUserIdsString)
        // {
        //     try
        //     {
        //         var vendorUserIds = vendorUserIdsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
        //             .Select(int.Parse)
        //             .ToArray();

        //         var vendorUsers = await _context.UserInfo
        //             .Where(u => vendorUserIds.Contains(u.UserId))
        //             .Include(u => u.Address)
        //             .Select(u => new UserInfoDto
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

        // GET Users by pages with sorting and filtering
        [HttpGet("paginated-users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetUserInfo(int page = 1, int pageSize = 10, string sortBy = "UserName", bool sortDesc = false,
            string? column = "", string? filterOperator = "", string? value = "")
        {
            try
            {
                // Create a base query
                IQueryable<UserInfo> query = _context.UserInfo.Include(u => u.Address).AsQueryable();

                // Apply filtering conditions if column, filterOperator, and value are provided
                if (!string.IsNullOrEmpty(column) && !string.IsNullOrEmpty(filterOperator) && !string.IsNullOrEmpty(value))
                {
                    query = ApplyFilter(query, column, filterOperator, value);
                }

                // Get count of total users
                int totalItems = await query.CountAsync();

                // Calculate total pages and ensure page is within range
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                page = Math.Max(1, Math.Min(page, totalPages));

                // Calculate skip count for pagination
                int skipCount = (page - 1) * pageSize;

                // Apply sorting dynamically
                query = ApplySorting(query, sortBy, sortDesc);

                // Retrieve paginated users
                List<UserInfo> users = await query.Skip(skipCount).Take(pageSize).ToListAsync();

                // Map UserInfo to UserInfoDto
                List<UserInfoDto> usersDto = users.Select(u => ConvertToUserInfoDto(u)).ToList();

                // Create response object
                var response = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Data = usersDto
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception /* ex */)
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while fetching user information.");

                return StatusCode(500, "An unexpected error occurred while fetching user information. Please try again later.");
            }
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
                    throw new ArgumentException("Invalid sortBy parameter.");
            }

            return query;
        }

        private IQueryable<UserInfo> ApplyFilter(IQueryable<UserInfo> query, string column, string filterOperator, string value)
        {
            if (string.IsNullOrEmpty(column) || string.IsNullOrEmpty(filterOperator) || string.IsNullOrEmpty(value))
            {
                // No filtering parameters provided, return the original query
                return query;
            }

            try
            {
                // Determine the data type of the column
                var property = typeof(UserInfo).GetProperty(column);
                if (property == null)
                {
                    throw new ArgumentException("Invalid column name.");
                }

                Type columnType = property.PropertyType;

                // Build the filter expression dynamically
                string filterExpression;
                object[] filterValues;

                if (columnType == typeof(string))
                {
                    // Handle string columns
                    switch (filterOperator.ToLower())
                    {
                        case "equals":
                            filterExpression = $"{column} == @0";
                            break;
                        case "contains":
                            filterExpression = $"{column}.Contains(@0)";
                            break;
                        case "startswith":
                            filterExpression = $"{column}.StartsWith(@0)";
                            break;
                        case "endswith":
                            filterExpression = $"{column}.EndsWith(@0)";
                            break;
                        default:
                            throw new ArgumentException("Invalid filter operator for string column.");
                    }

                    filterValues = new object[] { value };
                }
                else if (columnType == typeof(int))
                {
                    // Handle int columns
                    int intValue;
                    if (!int.TryParse(value, out intValue))
                    {
                        throw new ArgumentException("Invalid value for int column.");
                    }

                    switch (filterOperator.ToLower())
                    {
                        case "equals":
                            filterExpression = $"{column} == @0";
                            break;
                        case "notequals":
                            filterExpression = $"{column} != @0";
                            break;
                        case "greaterthan":
                            filterExpression = $"{column} > @0";
                            break;
                        case "lessthan":
                            filterExpression = $"{column} < @0";
                            break;
                        default:
                            throw new ArgumentException("Invalid filter operator for int column.");
                    }

                    filterValues = new object[] { intValue };
                }
                else
                {
                    throw new ArgumentException("Unsupported column type.");
                }

                // Apply the filter using System.Linq.Dynamic
                query = query.Where(filterExpression, filterValues);

                return query;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Error applying filter: {ex.Message}");
            }
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

        private UserInfoDto ConvertToUserInfoDto(UserInfo userInfo)
        {
            var userInfoDto = new UserInfoDto
            {
                UserId = userInfo.UserId,
                UserName = userInfo.UserName,
                Email = userInfo.Email,
                Role = userInfo.Role,
                Phone = userInfo.Phone,
                Address = userInfo.Address
            };

            return userInfoDto;
        }
    }
}
