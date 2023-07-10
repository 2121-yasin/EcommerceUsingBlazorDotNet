using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Models
{
    public class Products
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProdId { get; set; }
        public int VendorId{get; set;}
        public string ProdName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string ImageURL { get; set; }
        public DateTime StartDate { get; set; }
        public int StockQty { get; set; }  
        public string StockStatus { get; set; }
        public int ProductVendorId { get; set; }
    }
}

