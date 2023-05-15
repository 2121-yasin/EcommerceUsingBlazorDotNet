using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Auth.Models
{
   public class ClientProfile
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public Guid ClientId { get; set; }
    public string RedirectUrl { get; set; }
}
}