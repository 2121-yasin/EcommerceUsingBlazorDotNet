using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtDbApi.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Category description is required.")]
        [MaxLength(250)]
        public string? Description { get; set; }

        public string? CategoryImageUrl { get; set; }

        [Display(Name = "Parent Category")]
        public int? ParentCategoryId { get; set; }

        [ForeignKey("ParentCategoryId")]
        public Category? ParentCategory { get; set; }

        public List<Category>? ChildCategories { get; set; }

        public virtual List<Product>? Products { get; set; }

        [NotMapped]
        public int ProductCount => Products?.Count() ?? 0;

        public bool HasSpecifications { get; set; } = false;

        public string? BasicDetails { get; set; }

        public string? OptionalDetails { get; set; }
    }
}
