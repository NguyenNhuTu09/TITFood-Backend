namespace TITFood_Backend.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TITFood_Backend.Entities; 

    public class OrderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public UserDto? User { get; set; } 
        public int RestaurantId { get; set; }
        public RestaurantBriefDto? Restaurant { get; set; } 
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
    
    public class RestaurantBriefDto 
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }


    public class OrderItemDto
    {
        public int Id { get; set; }
        public int DishId { get; set; }
        public DishBriefDto? Dish { get; set; } 
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class DishBriefDto 
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    public class CreateOrderDto
    {
        [Required]
        public int RestaurantId { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Notes { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất một sản phẩm.")]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }

    public class CreateOrderItemDto
    {
        [Required]
        public int DishId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}