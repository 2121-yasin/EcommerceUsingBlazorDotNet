using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtDbApi.DTOs
{
    public class ProductVendorDto
    {
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int Visible { get; set; }
        public int ProductId { get; set; }
        public ProductDto? Product { get; set; }
    }
}
