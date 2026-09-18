using AutoMapper;
using RestaurantApp.Business.DTOs.Category;
using RestaurantApp.Business.DTOs.MenuItem;
using RestaurantApp.Business.DTOs.Order;
using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Business.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category
        CreateMap<Category, CategoryDto>();

        // MenuItem
        CreateMap<MenuItem, MenuItemDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
        CreateMap<MenuItemCreateDto, MenuItem>();
        CreateMap<MenuItemUpdateDto, MenuItem>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Order & OrderItem
        CreateMap<OrderItem, OrderItemDetailDto>()
            .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem != null ? src.MenuItem.Name : string.Empty));

        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.TotalItemCount, opt => opt.MapFrom(src => src.OrderItems.Sum(oi => oi.Count)));

        CreateMap<Order, OrderDetailDto>()
            .ForMember(dest => dest.TotalItemCount, opt => opt.MapFrom(src => src.OrderItems.Sum(oi => oi.Count)))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
    }
}
