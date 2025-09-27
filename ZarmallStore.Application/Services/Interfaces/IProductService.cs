using ZarmallStore.Data.DTOs.CommonDto;
using ZarmallStore.Data.DTOs.ProductCategoryDto;
using ZarmallStore.Data.DTOs.ProductDto;
using ZarmallStore.Data.Entities.ProductEntities;

namespace ZarmallStore.Application.Services.Interfaces
{
    public interface IProductService : IAsyncDisposable
    {
        #region Product
        Task<FilterProductDto> FilterProduct(FilterProductDto filter);
        Task<List<ProductItemDto>> GetNewProducts();
        Task<List<ProductItemDto>> BestSellerProducts();
        Task<List<Product>> GetSimilarProducts(long productId);
        Task<ProductDetailDto> ProductDetail(long productId);
        Task<CreateProductResult> CreateProduct(CreateProductDto dto);
        Task<EditProductDto> GetEditProduct(long productId);
        Task<EditProductResult> EditProduct(EditProductDto dto);
        Task<bool> DeleteProduct(long productId);
        #endregion

        #region Color
        Task<FilterColorDto> FilterColor(FilterColorDto filter);
        Task<List<ProductColor>> GetAllProductColors();
        Task CreateColor(CreateColorDTo dto);
        Task<EditColorDto> GetEditColor(long colorId);
        Task EditColor(EditColorDto dto);
        Task<bool> DeleteColor(long colorId);
        #endregion

        #region ProductVariant
        Task CreateProductVariant(List<CreateVariantItemDto> variants, long productId);
        Task<EditProductVariantDto> GetEditProductVariant(long variantId);
        Task<bool> EditProductVariant(EditProductVariantDto dto);
        Task<bool> DeleteProductVariant(long variantId);
        #endregion

        #region Feature
        Task CreateProductFeatures(List<CreateFeatureItemDTo> features, long productId);
        Task<EditFeaturesDto> GetEditFeature(long featureId);
        Task EditFeature(EditFeaturesDto dto);
        Task<bool> DeleteFeature(long featureId);
        #endregion

        #region Gallery
        Task CreateGallery(CreateGalleryDto dto);
        Task<EditGalleryDto> GetEditGallery(long galleryId);
        Task EditGallery(EditGalleryDto dto);
        Task<bool> DeleteGallery(long galleryId);
        #endregion

        #region Brand

        Task<List<Brand>> GetAllBrands();
        Task<FilterBrandDto> FilterBrand(FilterBrandDto filter);
        Task<bool> CreateBrand(CreateBrandDto dto);
        Task<bool> EditBrand(EditBrandDto dto);
        Task<EditBrandDto> GetEditBrand(long brandId);
        Task<bool> DeleteBrand(long brandId);
        #endregion
    }
}
