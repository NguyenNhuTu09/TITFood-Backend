using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TITFood_Backend.Entities
{
    public class Restaurant
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public double Rating { get; set; } 

        public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}