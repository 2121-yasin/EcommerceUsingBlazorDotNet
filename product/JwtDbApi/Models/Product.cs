using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtDbApi.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProdId { get; set; }
        public double StarRatings { get; set; } // Average star ratings for the product
        public int NumberOfRatings { get; set; } // Number of ratings given

        // public int ListedBy { get; set; }

        // [ForeignKey("ListedBy")]
        // public Vendor Vendor { get; set; }
        public string ProdName { get; set; }
        public string Description { get; set; }

        public string? BasicDetails { get; set; }
        public string? OptionalDetails { get; set; }
        public int Price { get; set; }
        public string ImageURL { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.Now;
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        // public int StockQty { get; set; }

        //Relationships
        public List<CartItem> CartItems { get; set; }
        public List<ProductVendor> ProductVendors { get; set; } = new List<ProductVendor>(); // Initialization required for the  GetOverallQuantity to work

        // Calculated property for overall quantity of visible listings
        [NotMapped]
        public int StockQty
        {
            get { return GetOverallQuantity(); }
            set { }
        }

        // Method to calculate overall quantity of visible listings
        public int GetOverallQuantity()
        {
            int overallQuantity = 0;
            foreach (var productVendor in ProductVendors)
            {
                if (productVendor.Visible == 1)
                {
                    overallQuantity += productVendor.Quantity;
                }
            }

            return overallQuantity;
        }
    }
}
