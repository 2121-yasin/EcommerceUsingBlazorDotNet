namespace JwtDbApi.DTOs
{
    public class VendorDto
    {
        public string? Name { get; set; }
        public string? ProfilePicURL { get; set; }
        public string? GSTIN
        { get; set; }
        public int DeliveryPinCode { get; set; }
        public int UserId { get; set; }
    }
}
