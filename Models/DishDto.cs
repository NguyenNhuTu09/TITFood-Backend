namespace TITFood_Backend.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DishDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public int MenuId { get; set; }
    }

    public class CreateDishDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        [Required]
        public int MenuId { get; set; }
    }
    public class UpdateDishDto : CreateDishDto
    {
    }
}