using System.Linq.Expressions;
using System.Text.Json;
using JwtDbApi.Data;
using JwtDbApi.DTOs;
using JwtDbApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtDbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Vendor")]
    public class QCRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly int _pendingStatus = 0;
        private readonly int _rejectedStatus = 1;

        public QCRequestsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/QCRequests/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> GetQCRequest(int id)
        {
            try
            {
                var qcRequest = await _context.QCRequests.FindAsync(id);

                if (qcRequest == null)
                {
                    return NotFound();
                }

                var qcRequestDto = MapToQCRequestDto(qcRequest);

                return Ok(qcRequestDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception /* ex */
            )
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while fetching the QCRequest.");

                return StatusCode(
                    500,
                    "An unexpected error occurred while fetching the QCRequest. Please try again later."
                );
            }
        }

        // GET: api/QCRequests
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetQCRequests(int page = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.QCRequests
                    .OrderBy(qc => qc.Status)
                    .ThenBy(qc => qc.RequestDate);

                int totalItems = await query.CountAsync();

                // Ensure pageSize is at least 1
                pageSize = Math.Max(1, pageSize);

                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                // Ensure page is within valid range
                page = Math.Max(1, Math.Min(page, totalPages));

                var qcRequests = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var qcRequestsDto = qcRequests.Select(qc => MapToQCRequestDto(qc)).ToList();

                var response = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Page = page,
                    PageSize = pageSize,
                    Data = qcRequestsDto
                };

                return Ok(response);
            }
            catch (Exception /* ex */
            )
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while fetching QCRequests.");

                return StatusCode(
                    500,
                    "An unexpected error occurred while fetching QCRequests. Please try again later."
                );
            }
        }

        // GET: api/QCRequests/pending
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingQCRequests(
            int page = 1,
            int pageSize = 10,
            string sortBy = "RequestDate",
            bool sortDesc = false
        )
        {
            try
            {
                var query = _context.QCRequests.Where(qc => qc.Status == _pendingStatus);

                query = ApplySorting(query, sortBy, sortDesc);

                int totalItems = await query.CountAsync();

                // Ensure pageSize is at least 1
                pageSize = Math.Max(1, pageSize);

                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                // Ensure page is within valid range
                page = Math.Max(1, Math.Min(page, totalPages));

                var QCRequests = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var response = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Page = page,
                    PageSize = pageSize,
                    Data = QCRequests,
                };

                return Ok(response);
            }
            catch (Exception /* ex */
            )
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while fetching the the pending QCRequests.");

                return StatusCode(
                    500,
                    "An unexpected error occurred while fetching the pending QCRequests. Please try again later."
                );
            }
        }

        // GET: api/QCRequests/rejected
        [HttpGet("rejected/{vendorId}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> GetPendingQCRequests(
            int vendorId,
            int page = 1,
            int pageSize = 10
        )
        {
            try
            {
                var query = _context.QCRequests
                    .Where(qc => qc.VendorId == vendorId)
                    .OrderByDescending(qc => qc.Status);

                int totalItems = await query.CountAsync();

                // Ensure pageSize is at least 1
                pageSize = Math.Max(1, pageSize);

                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                // Ensure page is within valid range
                page = Math.Max(1, Math.Min(page, totalPages));

                var QCRequests = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var qcRequestsDto = QCRequests.Select(qc => MapToQCRequestDto(qc)).ToList();

                var response = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Page = page,
                    PageSize = pageSize,
                    Data = qcRequestsDto,
                };

                return Ok(response);
            }
            catch (Exception)
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while fetching the the pending QCRequests.");

                return StatusCode(
                    500,
                    "An unexpected error occurred while fetching the pending QCRequests. Please try again later."
                );
            }
        }


        // GET: api/QCRequests/count-pending
        [HttpGet("count-pending")]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> GetCountPendingQCRequests([FromQuery] int? vendorId = null)
        {
            try
            {
                IQueryable<QCRequest>? query;

                if (vendorId != null)
                {
                    query = _context.QCRequests.Where(
                        qc => qc.VendorId == vendorId && qc.Status == _pendingStatus
                    );
                }
                else
                {
                    query = _context.QCRequests.Where(qc => qc.Status == _pendingStatus);
                }

                int count = await query.CountAsync();

                return Ok(count);
            }
            catch (Exception)
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while fetching the count of pending QCRequests.");

                return StatusCode(
                    500,
                    "An unexpected error occurred while fetching the count of pending QCRequests. Please try again later."
                );
            }
        }

        // GET: api/QCRequests/count-rejected
        [HttpGet("count-rejected/{vendorId}")]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> GetCountRejectedQCRequests(int vendorId)
        {
            try
            {
                var rejected = _context.QCRequests.Where(
                    qc => qc.VendorId == vendorId && qc.Status == _rejectedStatus
                );
                int rejectedCount = await rejected.CountAsync();
                var pending = _context.QCRequests.Where(
                    qc => qc.VendorId == vendorId && qc.Status == _pendingStatus
                );
                int pendingCount = await pending.CountAsync();

                return Ok(new
                {
                    rejectedCount,
                    pendingCount
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    "An unexpected error occurred while fetching the count of rejected QCRequests. Please try again later."
                );
            }
        }

        // POST: api/QCRequests
        [HttpPost]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> PostQCRequest(QCRequestDto qcRequestDto)
        {
            try
            {
                var qcRequest = MapToQCRequest(qcRequestDto);

                _context.Add(qcRequest);
                await _context.SaveChangesAsync();

                // Assign the generated qcRequest.Id to the qcRequestDto.Id
                qcRequestDto.Id = qcRequest.Id;

                // Return qcRequestDto as it is in object format
                return CreatedAtAction(
                    nameof(GetQCRequest),
                    new { id = qcRequest.Id },
                    qcRequestDto
                );
            }
            catch (Exception)
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while adding the QCRequest.");

                return StatusCode(
                    500,
                    "An unexpected error occurred while adding the QCRequest. Please try again later."
                );
            }
        }

        // PUT: api/QCRequests/reject/5
        [HttpPut("reject/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectQCRequest(int id, [FromBody] string adminMessage)
        {
            try
            {
                var qcRequest = await _context.QCRequests.FindAsync(id);

                if (qcRequest == null)
                {
                    return NotFound();
                }

                if (qcRequest.Status == _rejectedStatus)
                {
                    return BadRequest("QC request is already rejected.");
                }

                qcRequest.AdminMessage = adminMessage;
                qcRequest.Status = _rejectedStatus;
                await _context.SaveChangesAsync();

                var qcRequestDto = MapToQCRequestDto(qcRequest);

                return Ok(qcRequestDto);
            }
            catch (Exception /* ex */
            )
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while rejecting the QCRequest.");

                return StatusCode(
                    500,
                    "An unexpected error occurred while rejecting the QCRequest. Please try again later."
                );
            }
        }

        // PUT: api/QCRequests/VendorResponse/5
        [HttpPut("VendorResponse/{QCRequestId}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> VendorResponse(int QCRequestId, QCRequestDto qcRequestDto)
        {
            try
            {
                var qcRequest = await _context.QCRequests.FindAsync(QCRequestId);
                if (qcRequest == null)
                {
                    return NotFound();
                }

                qcRequest.Product = SerializeObject(qcRequestDto.Product);
                qcRequest.ProductVendor = SerializeObject(qcRequestDto.ProductVendor);
                qcRequest.Status = qcRequestDto.Status;
                qcRequest.AdminMessage = qcRequestDto.AdminMessage;
                qcRequest.VendorMessage = qcRequestDto.VendorMessage;

                await _context.SaveChangesAsync();

                // Return qcRequestDto as it is in object format
                return Ok();
            }
            catch (Exception)
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while adding the QCRequest.");

                return StatusCode(
                    500,
                    "An unexpected error occurred while adding the QCRequest. Please try again later."
                );
            }
        }

        // DELETE: api/QCRequests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQCRequest(int id)
        {
            try
            {
                var qcRequest = await _context.QCRequests.FindAsync(id);

                if (qcRequest == null)
                {
                    return NotFound();
                }

                _context.QCRequests.Remove(qcRequest);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception /* ex */
            )
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while updating the AdminMessage.");

                return StatusCode(
                    500,
                    "An unexpected error occurred while deleting the QCRequest. Please try again later."
                );
            }
        }

        private QCRequest MapToQCRequest(QCRequestDto qcRequestDto)
        {
            try
            {
                return new QCRequest
                {
                    Product = SerializeObject(qcRequestDto.Product),
                    // BasicDetails = qcRequestDto.BasicDetails?.ToString(),
                    // OptionalDetails = qcRequestDto.OptionalDetails?.ToString(),
                    ProductVendor = SerializeObject(qcRequestDto.ProductVendor),
                    CategoryId = qcRequestDto.CategoryId,
                    CategoryName = qcRequestDto.CategoryName,
                    VendorId = qcRequestDto.VendorId,
                    VendorName = qcRequestDto.VendorName,
                    Status = qcRequestDto.Status,
                    RequestDate = qcRequestDto.RequestDate,
                    AdminMessage = qcRequestDto.AdminMessage,
                    VendorMessage = qcRequestDto.VendorMessage,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "An error occurred while converting the qcRequestDto to QCRequest. Possible serialization error",
                    ex
                );
            }
        }

        private QCRequestDto MapToQCRequestDto(QCRequest qcRequest)
        {
            try
            {
                return new QCRequestDto
                {
                    Id = qcRequest.Id,
                    Product = DeserializeJson<ProductDto>(qcRequest.Product),
                    // BasicDetails = DeserializeJson<Dictionary<string, string>>(
                    //     qcRequest.BasicDetails
                    // ),
                    // OptionalDetails = DeserializeJson<Dictionary<string, string>>(
                    //     qcRequest.OptionalDetails
                    // ),
                    ProductVendor = DeserializeJson<ProductVendorDto>(qcRequest.ProductVendor),
                    CategoryId = qcRequest.CategoryId,
                    CategoryName = qcRequest.CategoryName,
                    VendorId = qcRequest.VendorId,
                    VendorName = qcRequest.VendorName,
                    Status = qcRequest.Status,
                    RequestDate = qcRequest.RequestDate,
                    AdminMessage = qcRequest.AdminMessage,
                    VendorMessage = qcRequest.VendorMessage
                };
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "An error occurred while converting the qcRequest to QCRequestDto. Possible deserialization error",
                    ex
                );
            }
        }

        private IQueryable<QCRequest> ApplySorting(
            IQueryable<QCRequest> query,
            string sortBy,
            bool sortDesc
        )
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = nameof(QCRequest.RequestDate);
            }

            var sortFieldMappings = new Dictionary<string, Expression<Func<QCRequest, object>>>(
                StringComparer.OrdinalIgnoreCase
            )
            {
                { nameof(QCRequest.VendorName), qc => qc.VendorName ?? string.Empty },
                { nameof(QCRequest.RequestDate), qc => qc.RequestDate }
            };

            if (sortFieldMappings.TryGetValue(sortBy, out var sortField))
            {
                query = sortDesc ? query.OrderByDescending(sortField) : query.OrderBy(sortField);
            }

            return query;
        }

        private string SerializeObject<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentException("The property " + nameof(obj) + " cannot be null.");
            }

            try
            {
                return JsonSerializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                throw new Exception("Serialization failed.", ex);
            }
        }

        private T DeserializeJson<T>(string? property)
        {
            if (property == null)
            {
                throw new ArgumentException(
                    "The property " + nameof(property) + " cannot be null."
                );
            }

            try
            {
                return JsonSerializer.Deserialize<T>(property)
                    ?? throw new Exception("Deserialization returned null.");
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialization failed.", ex);
            }
        }
    }
}
