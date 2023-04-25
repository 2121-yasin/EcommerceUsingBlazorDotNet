using System.ComponentModel.DataAnnotations;

namespace JwtDbApi.Models
{
    public class Vendor
    {
        [Key]
        public int Id { get; set; }
        public string GSTIN { get; set; }
        public int PinCode { get; set; }
        public int UserId { get; set; }
        //Relationships
        public List<ProductVendor> ProductVendors { get; set; }
    }
}