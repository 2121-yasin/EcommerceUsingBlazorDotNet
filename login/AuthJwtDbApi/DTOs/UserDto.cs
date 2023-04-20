using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthJwtDbApi.DTOs
{
    public class UserModel
    {
        //public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
    }

}