namespace TITFood_Backend.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public double Rating { get; set; }
        public List<MenuDto> Menus { get; set; } = new List<MenuDto>();
    }

    public class CreateRestaurantDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdateRestaurantDto : CreateRestaurantDto
    {
    }
}