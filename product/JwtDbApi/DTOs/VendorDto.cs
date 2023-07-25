namespace JwtDbApi.DTOs
{
    public class VendorDto
    {
        public int VendorId { get; set; }
        public string? Name { get; set; }
        public string? GSTIN
        { get; set; }
        public int DeliveryPinCode { get; set; }
        public int UserId { get; set; }
    }
}
