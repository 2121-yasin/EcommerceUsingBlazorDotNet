using System.ComponentModel.DataAnnotations;

namespace JwtDbApi.DTOs
{
    public class QCRequestDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Product is required")]
        public ProductDto? Product { get; set; } // Object to store product details like name, description, image, MRP, etc.
        [Required(ErrorMessage = "Basic Details is required")]
        public object? BasicDetails { get; set; } // Object to store basic product details
        public object? OptionalDetails { get; set; } // Object to store optional product details
        [Required(ErrorMessage = "ProductVendor is required")]
        public ProductVendorDto? ProductVendor { get; set; } // Object to store productvendor details like vendorId, price, stock, visibility, etc.
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category name is required")]
        public string? CategoryName { get; set; }
        public int VendorId { get; set; }
        [Required(ErrorMessage = "Vendor name is required")]
        public string? VendorName { get; set; }
        public int Status { get; set; } // QCStatus can be 0="Pending", 1="Rejected"
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string? AdminMessage { get; set; }
        public string? VendorMessage { get; set; }
    }
}
