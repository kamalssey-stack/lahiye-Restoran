using AutoMapper;
using RestaurantApp.Business.DTOs.Order;
using RestaurantApp.Business.Services.Interfaces;
using RestaurantApp.Data.UnitOfWork;
using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Business.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderDetailDto> AddOrderAsync(OrderCreateDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            throw new ArgumentException("Sifariş üçün ən az bir məhsul seçilməlidir!");

        var order = new Order
        {
            Date = DateTime.Now,
            OrderItems = new List<OrderItem>()
        };

        decimal total = 0;

        foreach (var itemDto in dto.Items)
        {
            if (itemDto.Count <= 0)
                throw new ArgumentException("Məhsul sayı 0-dan böyük olmalıdır!");

            var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(itemDto.MenuItemId);
            if (menuItem == null)
                throw new KeyNotFoundException($"#{itemDto.MenuItemId} nömrəli menyu məhsulu tapılmadı!");

            var orderItem = new OrderItem
            {
                MenuItemId = menuItem.Id,
                Count = itemDto.Count,
                UnitPrice = menuItem.Price
            };

            total += orderItem.UnitPrice * orderItem.Count;
            order.OrderItems.Add(orderItem);
        }

        order.TotalAmount = total;

        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        var createdOrder = await _unitOfWork.Orders.GetByIdWithItemsAsync(order.Id);
        return _mapper.Map<OrderDetailDto>(createdOrder);
    }

    public async Task RemoveOrderAsync(int id)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id);
        if (order == null)
            throw new KeyNotFoundException($"#{id} nömrəli sifariş tapılmadı!");

        _unitOfWork.Orders.Delete(order);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _unitOfWork.Orders.GetAllWithItemsAsync();
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByDatesIntervalAsync(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Başlanğıc tarixi son tarixdən böyük ola bilməz!");

        var orders = await _unitOfWork.Orders.GetByDateRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByDateAsync(DateTime date)
    {
        var orders = await _unitOfWork.Orders.GetByDateAsync(date);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByPriceIntervalAsync(decimal minPrice, decimal maxPrice)
    {
        if (minPrice < 0 || maxPrice < minPrice)
            throw new ArgumentException("Məbləğ aralığı düzgün daxil edilməyib!");

        var orders = await _unitOfWork.Orders.GetByPriceRangeAsync(minPrice, maxPrice);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDetailDto?> GetOrderByNoAsync(int id)
    {
        var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(id);
        return order == null ? null : _mapper.Map<OrderDetailDto>(order);
    }
}
