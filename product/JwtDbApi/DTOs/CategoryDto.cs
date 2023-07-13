using System.ComponentModel.DataAnnotations;

namespace JwtDbApi.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(50, ErrorMessage = "Category name must be at most 50 characters.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Category description is required.")]
        [MaxLength(250, ErrorMessage = "Description must be at most 250 characters.")]
        public string? Description { get; set; }

        public string? CategoryImageUrl { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool HasSpecifications { get; set; } = false;
        public List<string>? BasicDetails { get; set; }
        public List<string>? OptionalDetails { get; set; }

        // public List<CategoryDto>? ChildCategories { get; set; }
        // public int ProductCount { get; set; }
        // public List<int>? ChildCategoryIds { get; set; }
        // public List<Product>? Products { get; set; }
    }
}
