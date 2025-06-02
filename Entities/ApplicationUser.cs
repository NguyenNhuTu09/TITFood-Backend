using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TITFood_Backend.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual Cart? Cart { get; set; }
    }
}
