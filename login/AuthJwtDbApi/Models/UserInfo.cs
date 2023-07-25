using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuthJwtDbApi.DTOs;

namespace AuthJwtDbApi.Models
{
    public class UserInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? ProfilePicURL { get; set; }
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least 8 characters, one uppercase letter, one lowercase letter, one digit, and one special character")]
        public string Password { get; set; }
        public string Role { get; set; } = "User";
        public string Phone { get; set; }
        public int AddressId { get; set; }
        public AddressInfo Address { get; set; }

        public static implicit operator UserInfo(UserInfoDto v)
        {
            throw new NotImplementedException();
        }
    }

}