using AutoMapper;
using RestaurantApp.Business.Caching;
using RestaurantApp.Business.DTOs.MenuItem;
using RestaurantApp.Business.Services.Interfaces;
using RestaurantApp.Data.UnitOfWork;
using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Business.Services.Implementations;

public class MenuItemService : IMenuItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private const string CacheKeyAllMenuItems = "all_menu_items";

    public MenuItemService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<MenuItemDto>> GetAllAsync()
    {
        return await _cacheService.GetOrCreateAsync(CacheKeyAllMenuItems, async () =>
        {
            var items = await _unitOfWork.MenuItems.GetAllWithCategoryAsync();
            return _mapper.Map<IEnumerable<MenuItemDto>>(items);
        }, TimeSpan.FromMinutes(10));
    }

    public async Task<MenuItemDto?> GetByIdAsync(int id)
    {
        var item = await _unitOfWork.MenuItems.GetByIdWithCategoryAsync(id);
        return item == null ? null : _mapper.Map<MenuItemDto>(item);
    }

    public async Task<MenuItemDto> AddAsync(MenuItemCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Məhsulun adı boş ola bilməz!");

        if (dto.Price <= 0)
            throw new ArgumentException("Məhsulun qiyməti 0-dan böyük olmalıdır!");

        var exists = await _unitOfWork.MenuItems.NameExistsAsync(dto.Name);
        if (exists)
            throw new InvalidOperationException($"'{dto.Name}' adlı menyu məhsulu artıq mövcuddur! Eyni ad ilə birdən çox menu item ola bilməz.");

        var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new ArgumentException("Göstərilən kateqoriya mövcud deyil!");

        var entity = _mapper.Map<MenuItem>(dto);
        await _unitOfWork.MenuItems.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        _cacheService.Remove(CacheKeyAllMenuItems);

        var created = await _unitOfWork.MenuItems.GetByIdWithCategoryAsync(entity.Id);
        return _mapper.Map<MenuItemDto>(created);
    }

    public async Task EditAsync(int id, MenuItemUpdateDto dto)
    {
        var item = await _unitOfWork.MenuItems.GetByIdWithCategoryAsync(id);
        if (item == null)
            throw new KeyNotFoundException($"#{id} nömrəli menyu məhsulu tapılmadı!");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Məhsulun adı boş ola bilməz!");

        if (dto.Price <= 0)
            throw new ArgumentException("Məhsulun qiyməti 0-dan böyük olmalıdır!");

        var nameExists = await _unitOfWork.MenuItems.NameExistsAsync(dto.Name, excludeId: id);
        if (nameExists)
            throw new InvalidOperationException($"'{dto.Name}' adlı menyu məhsulu artıq mövcuddur! Eyni ad ilə birdən çox menu item ola bilməz.");

        item.Name = dto.Name.Trim();
        item.Price = dto.Price;
        if (dto.CategoryId.HasValue && dto.CategoryId.Value > 0)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId.Value);
            if (category == null)
                throw new ArgumentException("Göstərilən kateqoriya mövcud deyil!");
            item.CategoryId = dto.CategoryId.Value;
        }

        _unitOfWork.MenuItems.Update(item);
        await _unitOfWork.SaveChangesAsync();

        _cacheService.Remove(CacheKeyAllMenuItems);
    }

    public async Task RemoveAsync(int id)
    {
        var item = await _unitOfWork.MenuItems.GetByIdAsync(id);
        if (item == null)
            throw new KeyNotFoundException($"#{id} nömrəli menyu məhsulu tapılmadı!");

        _unitOfWork.MenuItems.Delete(item);
        await _unitOfWork.SaveChangesAsync();

        _cacheService.Remove(CacheKeyAllMenuItems);
    }

    public async Task<IEnumerable<MenuItemDto>> GetByCategoryAsync(int categoryId)
    {
        var items = await _unitOfWork.MenuItems.GetByCategoryIdAsync(categoryId);
        return _mapper.Map<IEnumerable<MenuItemDto>>(items);
    }

    public async Task<IEnumerable<MenuItemDto>> GetByPriceIntervalAsync(decimal minPrice, decimal maxPrice)
    {
        if (minPrice < 0 || maxPrice < minPrice)
            throw new ArgumentException("Qiymət aralığı düzgün daxil edilməyib!");

        var items = await _unitOfWork.MenuItems.GetByPriceRangeAsync(minPrice, maxPrice);
        return _mapper.Map<IEnumerable<MenuItemDto>>(items);
    }

    public async Task<IEnumerable<MenuItemDto>> SearchByNameAsync(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return await GetAllAsync();

        var items = await _unitOfWork.MenuItems.SearchByNameAsync(searchText);
        return _mapper.Map<IEnumerable<MenuItemDto>>(items);
    }
}
