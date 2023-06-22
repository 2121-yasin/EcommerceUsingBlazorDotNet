using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
    }
}