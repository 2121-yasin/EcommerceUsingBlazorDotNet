using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Vendor
    {
        [Key]
        public int Id { get; set; }
        public string? GSTIN { get; set; }
        public int DeliveryPinCode { get; set; }
        public int UserId { get; set; }

        //Relationships
        public List<ProductVendor>? ProductVendors { get; set; }
    }
}