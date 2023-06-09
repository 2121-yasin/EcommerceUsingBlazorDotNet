
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
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

            var userInfoDto = userInfo.Select(u => MapToUserInfoDto(u));

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

            var userInfoDto = MapToUserInfoDto(userInfo);

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

        // GET: api/UserInfo/paginated-users
        // GET paginated Users with sorting and filtering
        [HttpGet("paginated-users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetUserInfo(int page = 1, int pageSize = 10,
            string sortBy = "UserName", bool sortDesc = false,
            string? column = "", string? filterOperator = "", string? value = "")
        {
            try
            {
                IQueryable<UserInfo>? query = _context.UserInfo.Include(u => u.Address);

                // Apply filtering conditions if column, and filterOperator are provided
                if (!string.IsNullOrEmpty(column) && !string.IsNullOrEmpty(filterOperator))
                {
                    query = ApplyFilter(query, column, filterOperator, value);
                }

                query = ApplySorting(query, sortBy, sortDesc);

                int totalItems = await query.CountAsync();

                // Ensure pageSize is at least 1
                pageSize = Math.Max(1, pageSize);

                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                // Ensure page is within valid range
                page = Math.Max(1, Math.Min(page, totalPages));

                var users = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Map UserInfo to UserInfoDto
                var usersDto = users.Select(u => MapToUserInfoDto(u)).ToList();

                var response = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Page = page,
                    PageSize = pageSize,
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

        private IQueryable<UserInfo> ApplySorting(IQueryable<UserInfo> query, string sortBy, bool sortDesc)
        {
            var sortFieldMappings = new Dictionary<string, Expression<Func<UserInfo, object>>>(StringComparer.OrdinalIgnoreCase)
            {
                { nameof(UserInfo.UserId), u => u.UserId },
                { nameof(UserInfo.UserName), u => u.UserName },
                { nameof(UserInfo.Email), u => u.Email },
                { nameof(UserInfo.Role), u => u.Role }
            };

            if (sortFieldMappings.TryGetValue(sortBy, out var sortField))
            {
                query = sortDesc ? query.OrderByDescending(sortField) : query.OrderBy(sortField);
            }
            // else
            // {
            //     throw new ArgumentException("Invalid sortBy parameter.");
            // }

            return query;
        }


        // Apply sorting to users
        // private IQueryable<UserInfo> ApplySorting(IQueryable<UserInfo> query, string sortBy, bool sortDesc)
        // {
        //     switch (sortBy)
        //     {
        //         case nameof(UserInfo.UserId):
        //             query = sortDesc ? query.OrderByDescending(u => u.UserId) : query.OrderBy(u => u.UserId);
        //             break;
        //         case nameof(UserInfo.UserName):
        //             query = sortDesc ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName);
        //             break;
        //         case nameof(UserInfo.Email):
        //             query = sortDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email);
        //             break;
        //         case nameof(UserInfo.Role):
        //             query = sortDesc ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role);
        //             break;
        //         default:
        //             throw new ArgumentException("Invalid sortBy parameter.");
        //     }

        //     return query;
        // }

        // Apply sorting to users - Dynamically create the sorting expression using reflection - Switch statement approach might perform better
        // private IQueryable<UserInfo> ApplySorting(IQueryable<UserInfo> query, string sortBy, bool sortDesc)
        // {
        //     PropertyInfo? prop = typeof(UserInfo).GetProperty(sortBy);
        //     if (prop == null || sortBy == nameof(UserInfo.Password))
        //     {
        //         throw new ArgumentException("Invalid sortBy parameter.");
        //     }

        //     ParameterExpression parameter = Expression.Parameter(typeof(UserInfo), "u");
        //     Expression property = Expression.Property(parameter, prop);
        //     LambdaExpression lambda = Expression.Lambda(property, parameter);

        //     string methodName = sortDesc ? "OrderByDescending" : "OrderBy";
        //     MethodCallExpression orderByExpression = Expression.Call(typeof(Queryable), methodName,
        //         new[] { typeof(UserInfo), prop.PropertyType }, query.Expression, Expression.Quote(lambda));

        //     query = query.Provider.CreateQuery<UserInfo>(orderByExpression);

        //     return query;
        // }

        // Apply filtering
        private IQueryable<UserInfo> ApplyFilter(IQueryable<UserInfo> query, string column, string filterOperator, string? value)
        {
            if (string.IsNullOrEmpty(column) || string.IsNullOrEmpty(filterOperator))
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
                    if (string.IsNullOrEmpty(value) && filterOperator.ToLower() != "isempty")
                    {
                        throw new ArgumentException("Invalid filter value for string column.");
                    }

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
                        case "isempty":
                            filterExpression = $"string.IsNullOrEmpty({column})";
                            break;
                        default:
                            throw new ArgumentException("Invalid filter operator for string column.");
                    }
                    filterValues = filterOperator.ToLower() != "isempty" ? new object[] { value! } : new object[] { };
                }
                else if (columnType == typeof(int) || columnType == typeof(int?))
                {
                    // Handle int or nullable int columns
                    if (value == null && filterOperator.ToLower() != "isempty")
                    {
                        throw new ArgumentException("Invalid filter value for int column.");
                    }

                    int? intValue = null;
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!int.TryParse(value, out int parsedValue))
                        {
                            throw new ArgumentException("Invalid filter value for int column.");
                        }
                        intValue = parsedValue;
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
                        case "isempty":
                            filterExpression = $"{column} == null";
                            break;
                        default:
                            throw new ArgumentException("Invalid filter operator for int column.");
                    }

                    filterValues = filterOperator.ToLower() == "isempty" ? new object[] { } : new object[] { intValue };
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

        private UserInfoDto MapToUserInfoDto(UserInfo userInfo)
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
