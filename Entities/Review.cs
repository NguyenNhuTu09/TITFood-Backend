using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TITFood_Backend.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        public int RestaurantId { get; set; }
        [ForeignKey("RestaurantId")]
        public virtual Restaurant? Restaurant { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; } // 1 đến 5 sao

        [MaxLength(1000)]
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    }
}