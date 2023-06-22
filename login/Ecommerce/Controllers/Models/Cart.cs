using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Ecommerce.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [NotMapped]
        public int TotalPrice => CartItems?.Sum(item => item.Price) ?? 0;

        public List<CartItem> CartItems { get; set; }
    }
}
