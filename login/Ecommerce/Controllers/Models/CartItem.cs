using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int CartId { get; set; }

        [ForeignKey("CartId")]
        public Cart Cart { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public int ProductVendorId { get; set; }

        [ForeignKey("ProductVendorId")]
        public ProductVendor ProductVendor { get; set; }

        [NotMapped]
        public int Price => ProductVendor?.Price ?? 0;
    }
}
