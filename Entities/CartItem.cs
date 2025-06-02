using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TITFood_Backend.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }
        [ForeignKey("CartId")]
        public virtual Cart? Cart { get; set; }

        public int DishId { get; set; }
        [ForeignKey("DishId")]
        public virtual Dish? Dish { get; set; }

        [Required]
        [Range(1, 100)] 
        public int Quantity { get; set; }
    }
}