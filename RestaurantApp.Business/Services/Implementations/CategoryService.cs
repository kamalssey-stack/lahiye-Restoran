using AutoMapper;
using RestaurantApp.Business.Caching;
using RestaurantApp.Business.DTOs.Category;
using RestaurantApp.Business.Services.Interfaces;
using RestaurantApp.Data.UnitOfWork;

namespace RestaurantApp.Business.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private const string CacheKeyAllCategories = "all_categories";

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        return await _cacheService.GetOrCreateAsync(CacheKeyAllCategories, async () =>
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }, TimeSpan.FromMinutes(30));
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }
}
