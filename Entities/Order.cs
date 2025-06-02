using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TITFood_Backend.Entities
{
    public enum OrderStatus
    {
        Pending,        // Đang chờ xử lý
        Processing,     // Đang chuẩn bị
        OutForDelivery, // Đang giao hàng
        Delivered,      // Đã giao
        Cancelled,      // Đã hủy
        Failed          // Thất bại
    }

    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        public int RestaurantId { get; set; } // Để biết đơn hàng thuộc nhà hàng nào
        [ForeignKey("RestaurantId")]
        public virtual Restaurant? Restaurant { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? ShippingAddress { get; set; }
        public string? Notes { get; set; } // Ghi chú từ khách hàng

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}