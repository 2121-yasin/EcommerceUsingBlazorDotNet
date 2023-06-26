using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
public class CartItemModel
{
    public int ProductId { get; set; }
    public int ProductVendorId { get; set; }
}
}
