using System.Text.Json;
using System.Text.Json.Serialization;
using JwtDbApi.Data;
using JwtDbApi.DTOs;
using JwtDbApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace JwtDbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Category
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        // {
        //     return await _context.Categories.ToListAsync();
        // }

        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        // {
        //     // var categories = await _context.Categories
        //     //     .Select(c => new Category { CategoryId = c.CategoryId, Name = c.Name, Description = c.Description })
        //     //     .ToListAsync();

        //     var categories = await _context.Categories
        //         .Select(c => new CategoryDto { CategoryId = c.CategoryId, Name = c.Name, Description = c.Description })
        //         .ToListAsync();
        //     return categories;
        // }

        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                // ReferenceHandler = ReferenceHandler.IgnoreCycles,
                // WriteIndented = true
            };

            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.ChildCategories)
                .ToListAsync();

            var json = System.Text.Json.JsonSerializer.Serialize(categories, options);

            return Ok(json);

            // var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);

            // return Ok(categoryDtos);
        }

        // [HttpGet]
        // public IActionResult GetCategories()
        // {
        //     var categories = _context.Categories
        //         .Include(c => c.ParentCategory)
        //         .Include(c => c.ChildCategories)
        //         .ToList()
        //         .Select(c => new CategoryDto
        //         {
        //             CategoryId = c.CategoryId,
        //             Name = c.Name,
        //             Description = c.Description,
        //             ParentCategoryId = c.ParentCategoryId,
        //             ChildCategories = c.ChildCategories?.Select(cc => new CategoryDto
        //             {
        //                 CategoryId = cc.CategoryId,
        //                 Name = cc.Name,
        //                 Description = cc.Description,
        //                 ParentCategoryId = cc.ParentCategoryId,
        //                 ChildCategories = null, // don't include child categories of child categories
        //                 ProductCount = cc.ProductCount
        //             }).ToList(),
        //             ProductCount = c.ProductCount
        //         })
        //         .ToList();

        //     return Ok(categories);
        // }

        // GET: api/Category/Mobile
        // [HttpGet("{name}", Name = "GetCategoryByName")]
        // public ActionResult<CategoryDto> GetCategoryByName(string name)
        // {
        //     var category = _context.Categories.FirstOrDefault(c => c.Name == name);
        //     if (category == null)
        //     {
        //         return NotFound();
        //     }

        //     CategoryDto categoryDto = new CategoryDto();
        //     categoryDto.Name = category.Name;
        //     categoryDto.Description = category.Description;

        //     return Ok(categoryDto);
        // }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var basicDetails =
                category.BasicDetails != null
                    ? JsonConvert.DeserializeObject<List<string>>(category.BasicDetails)
                    : new List<string>();

            var optionalDetails =
                category.OptionalDetails != null
                    ? JsonConvert.DeserializeObject<List<string>>(category.OptionalDetails)
                    : new List<string>();

            var selectedCategory = new
            {
                category.CategoryId,
                category.Name,
                category.Description,
                category.CategoryImageUrl,
                BasicDetails = basicDetails,
                OptionalDetails = optionalDetails
            };

            return selectedCategory;
        }

        // POST: api/Category
        [HttpPost]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<ActionResult<CategoryDto>> PostCategory(CategoryDto categoryDto)
        {
            // Check if parentCategoryId is 0 and treat it as null
            if (categoryDto.ParentCategoryId == 0)
            {
                categoryDto.ParentCategoryId = null;
            }

            // Category validation
            var validationErrors = await ValidateCategory(categoryDto);

            if (validationErrors.Any())
            {
                return BadRequest(validationErrors);
            }

            // Create a new category
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                ParentCategoryId = categoryDto.ParentCategoryId
            };

            if (categoryDto.HasSpecifications)
            {
                category.HasSpecifications = true;
                category.BasicDetails = categoryDto.BasicDetails?.Any() == true
                    ? JsonConvert.SerializeObject(categoryDto.BasicDetails)
                    : null;
                category.OptionalDetails = categoryDto.OptionalDetails?.Any() == true
                    ? JsonConvert.SerializeObject(categoryDto.OptionalDetails)
                    : null;
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Assign the generated CategoryId to the CategoryDto
            categoryDto.CategoryId = category.CategoryId;

            return CreatedAtAction("GetCategory", new { id = categoryDto.CategoryId }, categoryDto);
        }

        private async Task<List<ErrorResponse>> ValidateCategory(CategoryDto categoryDto)
        {
            var errors = new List<ErrorResponse>();

            // Check if category Name is null or more than 50 characters - Data annotation does this but error response format is different
            // if (string.IsNullOrWhiteSpace(categoryDto.Name))
            // {
            //     errors.Add(new ErrorResponse { Field = "Name", Message = "Category name is required." });
            // }
            // else if (categoryDto.Name.Length > 50)
            // {
            //     errors.Add(new ErrorResponse { Field = "Name", Message = "Category name must be at most 50 characters." });
            // }

            // Check if category Description is null or more than 250 characters - Data annotation does this but error response format is different
            // if (string.IsNullOrWhiteSpace(categoryDto.Description))
            // {
            //     errors.Add(new ErrorResponse { Field = "Description", Message = "Category description is required." });
            // }
            // else if (categoryDto.Description.Length > 250)
            // {
            //     errors.Add(new ErrorResponse { Field = "Description", Message = "Description must be at most 250 characters." });
            // }

            if (categoryDto.ParentCategoryId != null && categoryDto.ParentCategoryId != 0)
            {
                var parentCategory = await _context.Categories.FindAsync(categoryDto.ParentCategoryId);
                if (parentCategory == null)
                {
                    errors.Add(new ErrorResponse { Field = "ParentCategoryId", Message = "Parent category not found. Cannot add a subcategory." });
                }
                else if (parentCategory.HasSpecifications)
                {
                    errors.Add(new ErrorResponse { Field = "ParentCategoryId", Message = "Parent category already has specifications. Cannot add a subcategory." });
                }
            }

            // Check if a category with the same name under the same parent already exists
            var isExistingCategory = await _context.Categories
                .AnyAsync(c => c.Name == categoryDto.Name && c.ParentCategoryId == categoryDto.ParentCategoryId);

            if (isExistingCategory)
            {
                var errorMessage = categoryDto.ParentCategoryId == null
                    ? "A main category with the same name already exists."
                    : "A category with the same name already exists under the selected parent category.";

                errors.Add(new ErrorResponse { Field = "Name", Message = errorMessage });
            }

            return errors;
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return category;
        }

        [HttpPut("{categoryId}/postarraydetails")]
        public async Task<IActionResult> PutArrayProductDetails(
            int categoryId,
            [FromBody] List<string> basicDetail
        )
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return NotFound();
            }

            category.BasicDetails = JsonConvert.SerializeObject(basicDetail);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{categoryId}/getarraydetails")]
        public async Task<IActionResult> GetArrayProductDetails(int categoryId)
        {
            var category = _context.Categories.Find(categoryId);
            if (category == null)
            {
                return NotFound();
            }

            var basicDetail = category.BasicDetails;
            var arrayValues = JsonConvert.DeserializeObject<List<string>>(basicDetail);

            // Return the array values or any other desired format
            return Ok(arrayValues);
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
