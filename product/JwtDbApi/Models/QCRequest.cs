using System.ComponentModel.DataAnnotations;

namespace JwtDbApi.Models
{
    public class QCRequest
    {
        [Key]
        public int Id { get; set; }
        public string Product { get; set; } // Object to store product details like name, description, image, MRP, etc.
        public string BasicDetails { get; set; } // Object to store basic product details
        public string? OptionalDetails { get; set; } // Object to store optional product details
        public string ProductVendor { get; set; } // Object to store productvendor details like vendorId, price, stock, visibility, etc.
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public int Status { get; set; } // QCStatus can be 0="Pending", 1="Rejected"
        public DateTime RequestDate { get; set; }

        // public int AdminAlert { get; set; }
        // public int VendorAlert { get; set; }
        public string AdminMessage { get; set; }
        public string VendorMessage { get; set; }
    }
}
