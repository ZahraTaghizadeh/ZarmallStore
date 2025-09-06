
using ZarmallStore.Data.DTOS.ProductCategoryDto;
using ZarmallStore.Data.DTOS.ProductDto;

namespace ZarmallStore.Application.Services.Interface
{
    public interface IProductService : IAsyncDisposable
    {
        #region Product
        Task<FilterProductDto> FilterProduct(FilterProductDto filter);
        Task<ProductDetailDto> ProductDetail(long productId);
        Task<CreateProductResult> CreateProduct(CreateProductDto dto);
        Task<EditProductDto> GetProductDto(long productId);
        Task<EditProductResult> EditProduct(EditProductDto dto);
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

        #region Color
        Task<FilterColorDto> FilterColor(FilterColorDto filter);
        Task CreateColor(CreateColorDto dto);
        Task<EditeColorDto> GetEditeColor(long colorId);
        Task EditeColor(EditeColorDto dto);
        Task<bool> DeleteColor(long colorId);
        #endregion

        #region ProductVariant
        Task CreateProductVariant(CreateProductVariantDto dto);
        Task<EditProductVariantDto> GetEditeProductVariant(long VariantId);
        Task EditeProductVariant(EditProductVariantDto dto);
        Task<bool> DeleteProductVariant(long VariantId);
        #endregion

        #region Feature
        Task<bool> DeleteFeature(long featureId);

        #endregion

        #region Gallery
        Task CreateGallery(CreateGalleryDto dto);
        Task<EditGalleryDto> GetEditGallery(long galleryId);
        Task EditGallery(EditGalleryDto dto);
        Task<bool> DeleteGallery(long galleryId);
        #endregion
    }
}
