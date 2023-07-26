
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net.Http.Headers;
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

        // PUT: api/UserInfo/UpdateName
        [HttpPut("UpdateName")]
        public IActionResult UpdateName([FromBody] string newName)
        {
            try
            {
                // Get the user ID from the JWT token
                var userId = GetUserIdFromToken();

                // Retrieve the user from the database
                var user = _context.UserInfo.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Update the user's name
                user.UserName = newName;
                _context.SaveChanges();

                return Ok("User name updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating user name: {ex.Message}");
            }
        }


        [HttpPut("UpdatePhoneNumber")]
        public IActionResult UpdatePhoneNumber([FromBody] string phoneNumber)
        {
            try
            {
                // Get the user ID from the JWT token
                var userId = GetUserIdFromToken();

                // Retrieve the user from the database
                var user = _context.UserInfo.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Update the phone number
                user.Phone = phoneNumber;
                _context.SaveChanges();

                return Ok("Phone number updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating phone number: {ex.Message}");
            }
        }

        // PUT: api/UserInfo/UpdateAddress
        // PUT: api/UserInfo/UpdateAddress
        [HttpPut("UpdateAddress")]
        public IActionResult UpdateAddress([FromBody] AddressInfo updatedAddress)
        {
            try
            {
                // Get the user ID from the JWT token
                var userId = GetUserIdFromToken();

                // Retrieve the user from the database
                var user = _context.UserInfo.Include(u => u.Address).FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Update the address information
                user.Address.Street = updatedAddress.Street;
                user.Address.City = updatedAddress.City;
                user.Address.State = updatedAddress.State;
                user.Address.Pincode = updatedAddress.Pincode;

                // Mark the address entity as Modified
                _context.Entry(user.Address).State = EntityState.Modified;

                _context.SaveChanges();

                return Ok("Address information updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating address: {ex.Message}");
            }
        }

        // PUT: api/UserInfo/UpdatePassword
        [HttpPut("UpdatePassword")]
        public IActionResult UpdatePassword([FromBody] UpdatePasswordDto updatePassword)
        {
            try
            {
                // Get the user ID from the JWT token
                var userId = GetUserIdFromToken();
                if (userId != updatePassword.UserId)
                {
                    return BadRequest("User Id from token and user Id from request body do not match");
                }
                // Retrieve the user from the database
                var user = _context.UserInfo.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Verify the current password
                var isCurrentPasswordCorrect = BCrypt.Net.BCrypt.Verify(updatePassword.CurrentPassword, user.Password);
                if (!isCurrentPasswordCorrect)
                {
                    return BadRequest("Current password is incorrect");
                }

                // Hash the new password
                var newPassword = BCrypt.Net.BCrypt.HashPassword(updatePassword.NewPassword);
                user.Password = newPassword;

                _context.SaveChanges();

                return Ok("Password updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating password: {ex.Message}");
            }
        }

        // Helper method to get the user ID from the JWT token
        private int GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            throw new InvalidOperationException("User ID not found in the JWT token.");
        }






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

        // Input Validation and Sanitization for filtering
        private bool ValidateFilterValue(string? value)
        {
            // Sanitize the input value

            return true;
        }
        // Whitelisting columns for filtering
        private bool IsAllowedColumn(string column)
        {
            // Define a list of allowed column names
            var allowedColumns = new List<string>
            {
                nameof(UserInfo.UserId),
                nameof(UserInfo.UserName),
                nameof(UserInfo.Email),
                nameof(UserInfo.Role)
            };

            // Check if the provided column is in the allowed list
            return allowedColumns.Contains(column, StringComparer.OrdinalIgnoreCase);
        }

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
                if (!ValidateFilterValue(value))
                {
                    throw new ArgumentException("Invalid filter input.");
                }

                if (!IsAllowedColumn(column))
                {
                    throw new ArgumentException("Invalid column name.");
                }

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
                    filterExpression = GetStringFilterExpression(column, filterOperator, value, out filterValues);
                }
                else if (columnType == typeof(int) || columnType == typeof(int?))
                {
                    // Handle int or nullable int columns
                    filterExpression = GetIntFilterExpression(column, filterOperator, value, out filterValues);
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

        // Get filter expression for string columns
        private string GetStringFilterExpression(string column, string filterOperator, string? value, out object[] filterValues)
        {
            if (string.IsNullOrEmpty(value) && (filterOperator.ToLower() != "isempty" && filterOperator.ToLower() != "isnotempty"))
            {
                throw new ArgumentException("Invalid filter value for string column.");
            }

            if (filterOperator.ToLower() == "isanyof")
            {
                return GetIsAnyOfFilterExpressionForString(column, value, out filterValues);
            }

            filterValues = new object[1];

            switch (filterOperator.ToLower())
            {
                case "contains":
                    filterValues[0] = value;
                    return $"{column}.Contains(@0)";
                case "equals":
                    filterValues[0] = value;
                    return $"{column} == @0";
                case "notequals":
                    filterValues[0] = value;
                    return $"{column} != @0";
                case "startswith":
                    filterValues[0] = value;
                    return $"{column}.StartsWith(@0)";
                case "endswith":
                    filterValues[0] = value;
                    return $"{column}.EndsWith(@0)";
                case "isempty":
                    return $"string.IsNullOrEmpty({column})";
                case "isnotempty":
                    return $"!string.IsNullOrEmpty({column})";
                default:
                    throw new ArgumentException("Invalid filter operator for string column.");
            }
        }

        // Get filter expression for int columns
        private string GetIntFilterExpression(string column, string filterOperator, string? value, out object[] filterValues)
        {
            if (string.IsNullOrEmpty(value) && (filterOperator.ToLower() != "isempty" && filterOperator.ToLower() != "isnotempty"))
            {
                throw new ArgumentException("Invalid filter value for int column.");
            }

            if (filterOperator.ToLower() == "isanyof")
            {
                return GetIsAnyOfFilterExpressionForInt(column, value, out filterValues);
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

            filterValues = new object[1];
            filterValues[0] = intValue;

            switch (filterOperator.ToLower())
            {
                case "equals":
                    return $"{column} == @0";
                case "notequals":
                    return $"{column} != @0";
                case "greaterthan":
                    return $"{column} > @0";
                case "greaterthanorequalto":
                    return $"{column} >= @0";
                case "lessthan":
                    return $"{column} < @0";
                case "lessthanorequalto":
                    return $"{column} <= @0";
                case "isempty":
                    return $"{column} == null";
                case "isnotempty":
                    return $"{column} != null";
                default:
                    throw new ArgumentException("Invalid filter operator for int column.");
            }
        }

        // Get filter expression for 'is any of' operator for string columns
        private string GetIsAnyOfFilterExpressionForString(string column, string value, out object[] filterValues)
        {
            // Split the value by comma to get individual items
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Invalid filter value for 'is any of' operator.");
            }

            var filterValuesArray = value.Split(',');

            // Generate a dynamic OR condition to check if the column matches any of the filter values
            var orConditions = new List<string>();
            filterValues = new object[filterValuesArray.Length];

            for (int i = 0; i < filterValuesArray.Length; i++)
            {
                var parameterName = $"@{i}";
                orConditions.Add($"{column} == {parameterName}");
                filterValues[i] = filterValuesArray[i];
            }

            // Combine the OR conditions using dynamic OR operator
            return string.Join(" || ", orConditions);
        }

        // Get filter expression for 'is any of' operator for integer columns
        private string GetIsAnyOfFilterExpressionForInt(string column, string value, out object[] filterValues)
        {
            // Split the value by comma to get individual items
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Invalid filter value for 'is any of' operator.");
            }

            var filterValuesArray = value.Split(',');

            // Generate a dynamic OR condition to check if the column matches any of the filter values
            var orConditions = new List<string>();
            filterValues = new object[filterValuesArray.Length];

            for (int i = 0; i < filterValuesArray.Length; i++)
            {
                if (!int.TryParse(filterValuesArray[i], out int parsedValue))
                {
                    throw new ArgumentException("Invalid filter value for int column.");
                }
                var parameterName = $"@{i}";
                orConditions.Add($"{column} == {parameterName}");
                filterValues[i] = parsedValue;
            }

            // Combine the OR conditions using the dynamic OR operator
            return string.Join(" || ", orConditions);
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

        // PUT : api/UserInfo/UpdateVendorInfo/
        [HttpPut("UpdateVendorInfo/{userId}")]
        public async Task<ActionResult> UpdateVendorInfo(int userId, UserInfoDto userInfo)
        {
            try
            {
                // Get the user ID from the JWT token
                var userIdFromToken = GetUserIdFromToken();
                if (userIdFromToken != userId)
                {
                    return BadRequest("User Id from token and user Id from request body do not match");
                }

                // Retrieve the user from the database using the user ID
                var user = await _context.UserInfo.Include(u => u.Address).FirstOrDefaultAsync(u => u.UserId == userIdFromToken);

                // If the user with the specified user ID is not found, return a "Not Found" response
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                // Update the user's fields with the data from the UserInfoDto object received in the request body
                user.UserName = userInfo.UserName;
                user.ProfilePicURL = userInfo.ProfilePicURL;
                user.Phone = userInfo.Phone;

                user.Address.Street = userInfo.Address.Street;
                user.Address.City = userInfo.Address.City;
                user.Address.State = userInfo.Address.State;
                user.Address.Pincode = userInfo.Address.Pincode;

                await _context.SaveChangesAsync();

                // Return an "Ok" response with a success message
                return Ok("User information updated successfully.");
            }
            catch (Exception ex)
            {
                // If any error occurs during the update process, return a "Bad Request" response with the error message.
                return BadRequest($"Error updating user information: {ex.Message}");
            }
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
                ProfilePicURL = userInfo.ProfilePicURL,
                Email = userInfo.Email,
                Role = userInfo.Role,
                Phone = userInfo.Phone,
                Address = userInfo.Address
            };

            return userInfoDto;
        }
    }
}
