using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JwtDbApi.Models
{
    public class QCRequest
    {
        [Key]
        public int QCId { get; set; }
        public string ProductName { get; set; }
        public string ImageURL { get; set; }
        public int Status { get; set; }
        public int BasePrice { get; set; }
        public string Description { get; set; }
        public string BasicDetails { get; set; }
        public string? OptionalDetails { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int Visible { get; set; }
        public string QCStatus { get; set; }
        public int AdminAlert { get; set; }
        public int VendorAlert { get; set; }
        public string RequestMessage { get; set; }
        public string ResponseMessage { get; set; }
    }
}
