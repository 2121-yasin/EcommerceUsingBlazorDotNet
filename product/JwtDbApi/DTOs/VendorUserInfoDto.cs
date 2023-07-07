namespace JwtDbApi.DTOs
{   // This DTO may no longer be required as vendor name added to vendor model
    public class VendorUserInfoDTO
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public AddressInfo? Address { get; set; }
    }

    public class AddressInfo
    {
        public int AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
    }
}
