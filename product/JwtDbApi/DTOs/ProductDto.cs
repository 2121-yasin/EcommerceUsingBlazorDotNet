using System.ComponentModel.DataAnnotations;

namespace JwtDbApi.DTOs
{
    public class ProductDto
    {
        public string? ProdName { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public string? ImageURL { get; set; }
        public object? BasicDetails { get; set; }
        public object? OptionalDetails { get; set; }
        public int CategoryId { get; set; }
        public ProductVendorDto? productVendor { get; set; }
    }
}
