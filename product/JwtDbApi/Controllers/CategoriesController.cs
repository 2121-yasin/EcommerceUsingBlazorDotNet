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
            Category parentCategory;
            Category category = new Category();

            if (categoryDto.ParentCategoryId != null && categoryDto.ParentCategoryId != 0)
            {
                parentCategory = await _context.Categories.FindAsync(categoryDto.ParentCategoryId);

                if (parentCategory != null && parentCategory.HasProducts)
                {
                    return BadRequest(
                        new
                        {
                            Field = "ParentCategoryId",
                            Message = "Parent category already has products. Cannot add a new category."
                        }
                    );
                }
                else if (parentCategory == null)
                {
                    return BadRequest(
                        new
                        {
                            Field = "ParentCategoryId",
                            Message = "Parent category not found. Cannot add a new category."
                        }
                    );
                }

                category.ParentCategoryId = categoryDto.ParentCategoryId;
            }

            category.CategoryId = categoryDto.CategoryId;
            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;

            if (categoryDto.HasProducts)
            {
                category.HasProducts = categoryDto.HasProducts;
                category.BasicDetails = JsonConvert.SerializeObject(categoryDto.BasicDetails);
                category.OptionalDetails = categoryDto.OptionalDetails;
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = categoryDto.CategoryId }, categoryDto);
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
