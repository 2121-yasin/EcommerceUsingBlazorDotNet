using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class CartRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ProductVendorId { get; set; }
    }
}
