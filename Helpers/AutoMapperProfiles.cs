using AutoMapper;
using TITFood_Backend.Entities;
using TITFood_Backend.Models;
using System.Linq;

namespace TITFood_Backend.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // User
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); 
            CreateMap<RegisterModel, ApplicationUser>();
            CreateMap<UpdateUserDto, ApplicationUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Chỉ map các thuộc tính không null
            CreateMap<ApplicationUser, UserBriefDto>();


            // Restaurant
            CreateMap<Restaurant, RestaurantDto>()
                .ForMember(dest => dest.Menus, opt => opt.MapFrom(src => src.Menus));
            CreateMap<CreateRestaurantDto, Restaurant>();
            CreateMap<UpdateRestaurantDto, Restaurant>();
            CreateMap<Restaurant, RestaurantBriefDto>();


            // Menu
            CreateMap<Menu, MenuDto>()
                 .ForMember(dest => dest.Dishes, opt => opt.MapFrom(src => src.Dishes));
            CreateMap<CreateMenuDto, Menu>();
            CreateMap<UpdateMenuDto, Menu>();

            // Dish
            CreateMap<Dish, DishDto>();
            CreateMap<CreateDishDto, Dish>();
            CreateMap<UpdateDishDto, Dish>();
            CreateMap<Dish, DishBriefDto>();

            // Order
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Restaurant, opt => opt.MapFrom(src => src.Restaurant))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
            CreateMap<CreateOrderDto, Order>(); 

            // OrderItem
            CreateMap<OrderItem, OrderItemDto>()
                 .ForMember(dest => dest.Dish, opt => opt.MapFrom(src => src.Dish));
            CreateMap<CreateOrderItemDto, OrderItem>(); 

            // Review
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Restaurant, opt => opt.MapFrom(src => src.Restaurant));
            CreateMap<CreateReviewDto, Review>(); 
            CreateMap<UpdateReviewDto, Review>();

            // Cart
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Items.Sum(i => i.Quantity * (i.Dish != null ? i.Dish.Price : 0))));
            
            // CartItem
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.DishName, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.Name : string.Empty))
                .ForMember(dest => dest.DishImageUrl, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.ImageUrl : null))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.Price : 0))
                .ForMember(dest => dest.TotalItemPrice, opt => opt.MapFrom(src => src.Quantity * (src.Dish != null ? src.Dish.Price : 0)));
            CreateMap<AddToCartDto, CartItem>(); // DishId và Quantity sẽ được map
        }
    }
}