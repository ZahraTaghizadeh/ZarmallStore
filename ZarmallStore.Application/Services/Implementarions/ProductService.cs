using ZarmallStore.Application.Services.Interface;
using ZarmallStore.Data.DTOS.ProductCategoryDto;
using ZarmallStore.Data.DTOS.ProductDto;
using ZarmallStore.Data.Entities.ProductEntities;
using ZarmallStore.Data.Repository;

namespace ZarmallStore.Application.Services.Implementarions
{
    public class ProductService : IProductService
    {
        #region CTOR
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductCategory> _categoryRepository;
        private readonly IGenericRepository<ProductColor> _colorRepository;
        private readonly IGenericRepository<ProductVariant> _variantRepository;
        private readonly IGenericRepository<ProductComment> _commentRepository;
        private readonly IGenericRepository<ProductSelectedCategory> _selectedCategoryRepository;
        private readonly IGenericRepository<ProductFeature> _featureRepository;
        private readonly IGenericRepository<ProductGallery> _galleryRepository;
        public ProductService(IGenericRepository<Product> productRepository, IGenericRepository<ProductCategory> categoryRepository, IGenericRepository<ProductColor> colorRepository, IGenericRepository<ProductVariant> variantRepository, IGenericRepository<ProductComment> commentRepository, IGenericRepository<ProductSelectedCategory> selectedCategoryRepository, IGenericRepository<ProductFeature> featureRepository, IGenericRepository<ProductGallery> galleryRepository)
        {
            _productRepository.productRepository;
            _categoryRepository.categoryRepository;
            _colorRepository.colorRepository;
            _variantRepository.variantRepository;
            _commentRepository.commentRepository;
            _selectedCategoryRepository.selectedCategoryRepository;
            _featureRepository.featureRepository;
            _galleryRepository.galleryRepository;
        }

        public async ValueTask DisposeAsync()
        {
            await _productRepository.DisposeAsync();
            await _categoryRepository.DisposeAsync();
            await _colorRepository.DisposeAsync();
            await _variantRepository.DisposeAsync();
            await _commentRepository.DisposeAsync();
            await _selectedCategoryRepository.DisposeAsync();
            await _featureRepository.DisposeAsync();
            await _galleryRepository.DisposeAsync();
        }
        #endregion

        #region Product
        public Task AddProductSelectedCategories(List<long> selectedCategories, long productId)
        {
            throw new NotImplementedException();
        }

        public Task CreateCategory(CreateCategoryDto dto)
        {
            throw new NotImplementedException();
        }

        public Task CreateProduct(CreateProductDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCategory(long categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProduct(long productId)
        {
            throw new NotImplementedException();
        }
        public Task EditProduct(EditProductDto dto)
        {
            throw new NotImplementedException();
        }
        public Task<FilterProductDto> FilterProduct(FilterProductDto filter)
        {
            throw new NotImplementedException();
        }
        public Task<EditProductDto> GetProductDto(long productId)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDetailDto> ProductDetail(long productId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Category
        public Task EditCategory(EditCategoryDto dto)
        {
            throw new NotImplementedException();
        }
        public Task<FilterCategoryDto> FilterCategory(FilterCategoryDto filter)
        {
            throw new NotImplementedException();
        }

        public Task<EditCategoryDto> GetEditCategory(long categoryId)
        {
            throw new NotImplementedException();
        }
        public Task RemoveProductSelectedCategories(long productId)
        {
            throw new NotImplementedException();
        }
        #endregion











    }
}
