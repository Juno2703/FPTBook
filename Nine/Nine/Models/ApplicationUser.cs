using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Nine.Models
{
    public class ApplicationUser :IdentityUser
    {
        public string? Descripstion { get; set; }
        
        public string? FullName { get; set; }
        public string? Address { get; set; }
    }
}
