using ZarmallStore.Data.DTOs.CommonDto;
using ZarmallStore.Data.DTOs.ProductCategoryDto;
using ZarmallStore.Data.Entities.ProductEntities;

namespace ZarmallStore.Application.Services.Interfaces
{
    public interface ICategoryService : IAsyncDisposable
    {
        #region Categories
        Task<List<ProductCategory>> GetAllActiveCategories();
        Task<List<CategoryItemDto>> GetProductsCategoryForHome();
        Task<List<ProductCategory>> GetAllCategories(long? parentId);
        Task<List<ProductCategory>> GetAllCategoriesForEdit(long? parentId, long thisCategoryId);
        Task<bool> AddProductSelectedCategories(List<long> selectedCategories, long productId);
        Task RemoveProductSelectedCategories(long productId);
        Task<FilterCategoryDto> FilterCategory(FilterCategoryDto filter);
        Task<bool> CreateCategory(CreateCategoryDto dto);
        Task<bool> EditCategory(EditCategoryDto dto);
        Task<EditCategoryDto> GetEditCategory(long categoryId);
        Task<bool> DeleteCategory(long categoryId);
        #endregion
    }
}
