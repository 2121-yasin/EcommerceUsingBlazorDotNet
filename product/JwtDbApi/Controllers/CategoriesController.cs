using System.Text.Json;
using System.Text.Json.Serialization;
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

            var json = JsonSerializer.Serialize(categories, options);

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
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // POST: api/Category
        [HttpPost]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<ActionResult<CategoryDto>> PostCategory(CategoryDto categoryDto)
        {
            Category category = new Category();
            category.CategoryId = categoryDto.CategoryId;
            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;
            if (categoryDto.ParentCategoryId > 0)
            {
                category.ParentCategoryId = categoryDto.ParentCategoryId;
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

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
