namespace TITFood_Backend.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ReviewDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public UserDto? User {get; set;} // Thông tin người dùng review
        public int RestaurantId { get; set; }
        public RestaurantBriefDto? Restaurant {get; set;} // Thông tin nhà hàng được review
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }

    public class CreateReviewDto
    {
        [Required]
        public int RestaurantId { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}