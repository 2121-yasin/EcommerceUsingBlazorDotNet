using System.ComponentModel.DataAnnotations;

namespace JwtDbApi.DTOs
{
    public class ProductVendorDto
    {
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Visible { get; set; }
        public int ProductId { get; set; }
        public ProductDto? Product { get; set; }
    }
}
