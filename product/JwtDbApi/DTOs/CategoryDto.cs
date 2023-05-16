namespace JwtDbApi.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int ParentCategoryId { get; set; } = -1;
        // public List<CategoryDto>? ChildCategories { get; set; }
        // public int ProductCount { get; set; }
        // public List<int>? ChildCategoryIds { get; set; }
        // public List<Product>? Products { get; set; }
    }
}