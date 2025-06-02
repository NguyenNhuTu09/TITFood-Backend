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
            CreateMap<CreateOrderDto, Order>(); // UserId sẽ được set trong service

            // OrderItem
            CreateMap<OrderItem, OrderItemDto>()
                 .ForMember(dest => dest.Dish, opt => opt.MapFrom(src => src.Dish));
            CreateMap<CreateOrderItemDto, OrderItem>(); // UnitPrice sẽ được set trong service

            // Review
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Restaurant, opt => opt.MapFrom(src => src.Restaurant));
            CreateMap<CreateReviewDto, Review>(); // UserId sẽ được set trong service
        }
    }
}