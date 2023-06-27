using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class ProductVendor
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Products Product { get; set; }

        public int VendorId { get; set; }

        [ForeignKey("VendorId")]
        public Vendor Vendor { get; set; }

        public DateTime ListedOn { get; set; } = DateTime.Now;
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Visible { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
