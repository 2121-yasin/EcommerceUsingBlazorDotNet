
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class OfferCode
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int MaxUses { get; set; }
        public int UsedCount { get; set; }
    }

}

