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
    public class QCRequestController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly int _pendingStatus = 0;
        private readonly int _rejectedStatus = 1;

        public QCRequestController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/QCRequest/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> GetQCRequest(int id)
        {
            try
            {
                var qCRequest = await _context.QCRequests.FindAsync(id);

                if (qCRequest == null)
                {
                    return NotFound();
                }

                var qCrequestsDto = MapToQCRequestDto(qCRequest);

                return Ok(qCrequestsDto);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception /* ex */)
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while fetching the QCRequest.");

                return StatusCode(500, "An unexpected error occurred while fetching the QCRequest. Please try again later.");
            }
        }

        // GET: api/QCRequest
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetQCRequests(int page = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.QCRequests.OrderBy(qc => qc.Status).ThenBy(qc => qc.RequestDate);

                int totalItems = await query.CountAsync();

                // Ensure pageSize is at least 1
                pageSize = Math.Max(1, pageSize);

                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                // Ensure page is within valid range
                page = Math.Max(1, Math.Min(page, totalPages));

                var qCRequests = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var qCRequestsDto = qCRequests.Select(qc => MapToQCRequestDto(qc)).ToList();

                var response = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Page = page,
                    PageSize = pageSize,
                    Data = qCRequestsDto
                };

                return Ok(response);
            }
            catch (Exception /* ex */)
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while fetching QCRequests.");

                return StatusCode(500, "An unexpected error occurred while fetching QCRequests. Please try again later.");
            }
        }

        // GET: api/QCRequest/pending
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingQCRequests(int page = 1, int pageSize = 10,
            string sortBy = "RequestDate", bool sortDesc = false)
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
            catch (Exception /* ex */)
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while fetching the the pending QCRequests.");

                return StatusCode(500, "An unexpected error occurred while fetching the pending QCRequests. Please try again later.");
            }
        }

        // GET: api/QCRequest/count-pending
        [HttpGet("count-pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCountPendingQCRequests()
        {
            try
            {
                var query = _context.QCRequests.Where(qc => qc.Status == _pendingStatus);
                int count = await query.CountAsync();

                return Ok(count);
            }
            catch (Exception /* ex */)
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while fetching the count of pending QCRequests.");

                return StatusCode(500, "An unexpected error occurred while fetching the count of pending QCRequests. Please try again later.");
            }
        }

        // POST: api/QCRequest
        [HttpPost]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> PostQCRequest(QCRequestDto qCRequestDto)
        {
            try
            {
                var qCRequest = MapToQCRequest(qCRequestDto);

                _context.Add(qCRequest);
                await _context.SaveChangesAsync();

                // Assign the generated qCRequest.Id to the qCRequestDto.Id
                qCRequestDto.Id = qCRequest.Id;

                // Return qCRequestDto as it is in object format
                return CreatedAtAction(nameof(GetQCRequest), new { id = qCRequest.Id }, qCRequestDto);

            }
            catch (Exception /* ex */)
            {
                // Log the exception for further investigation
                // logger.LogError(ex, "An error occurred while adding the QCRequest.");

                return StatusCode(500, "An unexpected error occurred while adding the QCRequest. Please try again later.");
            }
        }

        private QCRequest MapToQCRequest(QCRequestDto qCRequestDto)
        {
            try
            {
                return new QCRequest
                {
                    Product = SerializeObject(qCRequestDto.Product),
                    BasicDetails = qCRequestDto.BasicDetails?.ToString(),
                    OptionalDetails = qCRequestDto.OptionalDetails?.ToString(),
                    ProductVendor = SerializeObject(qCRequestDto.ProductVendor),
                    CategoryId = qCRequestDto.CategoryId,
                    CategoryName = qCRequestDto.CategoryName,
                    VendorId = qCRequestDto.VendorId,
                    VendorName = qCRequestDto.VendorName,
                    Status = qCRequestDto.Status,
                    RequestDate = qCRequestDto.RequestDate,
                    AdminMessage = qCRequestDto.AdminMessage,
                    VendorMessage = qCRequestDto.VendorMessage,
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while converting the qCRequestDto to QCRequest. Possible serialization error", ex);
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
                    BasicDetails = DeserializeJson<Dictionary<string, string>>(qcRequest.BasicDetails),
                    OptionalDetails = DeserializeJson<Dictionary<string, string>>(qcRequest.OptionalDetails),
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
                throw new Exception("An error occurred while converting the qCRequestDto to QCRequest. Possible serialization error", ex);
            }
        }

        private IQueryable<QCRequest> ApplySorting(IQueryable<QCRequest> query, string sortBy, bool sortDesc)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = "RequestDate";
            }

            var sortFieldMappings = new Dictionary<string, Expression<Func<QCRequest, object>>>
            {
                { "vendorname", qc => qc.VendorName ?? string.Empty },
                { "requestdate", qc => qc.RequestDate }
            };

            if (sortFieldMappings.TryGetValue(sortBy.ToLower(), out var sortField))
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
                throw new ArgumentException("The property " + nameof(property) + " cannot be null.");
            }

            try
            {
                return JsonSerializer.Deserialize<T>(property) ?? throw new Exception("Deserialization returned null.");
            }
            catch (Exception ex)
            {
                throw new Exception("Deserialization failed.", ex);
            }
        }
    }
}
