public class ProductVendorSortingDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public object? ProductBasicDetails { get; set; }
    public object? ProductOptionalDetails { get; set; }
    public int ProductBasePrice { get; set; }
    public string? ProductImageUrl { get; set; }
    // public DateTime? ProductStartDate { get; set; } = DateTime.Now;
    public int ProductVendorId { get; set; }
    public DateTime ProductVendorListedOn { get; set; }
    public int ProductVendorPrice { get; set; }
    public int ProductVendorQuantity { get; set; }
    public int ProductVendorVisible { get; set; }
    // public int VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? VendorGSTIN { get; set; }
    public int VendorDeliveryPinCode { get; set; }
    public int VendorUserId { get; set; }
    // public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
}
