
using ZarmallStore.Data.DTOS.ProductCategoryDto;
using ZarmallStore.Data.DTOS.ProductDto;

namespace ZarmallStore.Application.Services.Interface
{
    public interface IProductService : IAsyncDisposable
    {
        #region Product
        Task<FilterProductDto> FilterProduct(FilterProductDto filter);
        Task<ProductDetailDto> ProductDetail(long productId);
        Task CreateProduct(CreateProductDto dto);
        Task<EditProductDto> GetProductDto(long productId);
        Task EditProduct(EditProductDto dto);
        Task<bool> DeleteProduct(long productId);
        #endregion

        #region Categories
        Task AddProductSelectedCategories(List<long> selectedCategories, long productId);
        Task RemoveProductSelectedCategories(long productId);
        Task<FilterCategoryDto> FilterCategory(FilterCategoryDto filter);
        Task CreateCategory(CreateCategoryDto dto);
        Task EditCategory(EditCategoryDto dto);
        Task<EditCategoryDto> GetEditCategory(long categoryId);
        Task<bool> DeleteCategory(long categoryId);
        #endregion
    }
}
