using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtDbApi.DTOs
{
    public class VendorDto
    {
        public string? GSTIN { get; set; }
        public int DeliveryPinCode { get; set; }
        public int UserId { get; set; }
    }
}