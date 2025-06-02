using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TITFood_Backend.Entities;

namespace TITFood_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Restaurant>()
                .Property(r => r.Rating)
                .HasDefaultValue(0);

            // User - Order (One-to-Many)
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            // User - Review (One-to-Many)
            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Restaurant - Review (One-to-Many)
            builder.Entity<Review>()
                .HasOne(r => r.Restaurant)
                .WithMany(res => res.Reviews)
                .HasForeignKey(r => r.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade); 

            // Restaurant - Menu (One-to-Many)
            builder.Entity<Menu>()
                .HasOne(m => m.Restaurant)
                .WithMany(r => r.Menus)
                .HasForeignKey(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Menu - Dish (One-to-Many)
            builder.Entity<Dish>()
                .HasOne(d => d.Menu)
                .WithMany(m => m.Dishes)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Restaurant - Order (One-to-Many)
            builder.Entity<Order>()
                .HasOne(o => o.Restaurant)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa Order khi Restaurant bị xóa (hoặc tùy logic)

            // User - Cart (One-to-One)
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Cart khi User bị xóa

            // Cart - CartItem (One-to-Many)
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa CartItem khi Cart bị xóa

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Dish)
                .WithMany(d => d.CartItems)
                .HasForeignKey(ci => ci.DishId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa CartItem nếu Dish bị xóa (hoặc set null DishId)
        }
    }
}
