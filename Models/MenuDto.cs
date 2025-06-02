namespace TITFood_Backend.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class MenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public List<DishDto> Dishes { get; set; } = new List<DishDto>();
    }

    public class CreateMenuDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int RestaurantId { get; set; }
    }
     public class UpdateMenuDto : CreateMenuDto
    {
    }
}