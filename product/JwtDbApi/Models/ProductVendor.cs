using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtDbApi.Models
{
    public class ProductVendor
    {
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int VendorId { get; set; }

        [ForeignKey("VendorId")]
        public Vendor Vendor { get; set; }
        public DateTime ListedOn { get; set; } = DateTime.Now;
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Visible { get; set; }
    }
}
