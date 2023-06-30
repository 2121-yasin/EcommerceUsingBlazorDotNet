using AuthJwtDbApi.Models;

namespace AuthJwtDbApi.DTOs
{
    public class UserInfoDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; } = "User";
        public string? Phone { get; set; }
        public AddressInfo? Address { get; set; }
    }
}
