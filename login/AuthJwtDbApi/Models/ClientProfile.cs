using System.ComponentModel.DataAnnotations;

namespace AuthJwtDbApi.Models
{
    public class ClientProfile
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid ClientId { get; set; }
        public string RedirectUrl { get; set; }
    }
}