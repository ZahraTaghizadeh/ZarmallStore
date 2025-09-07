using Microsoft.EntityFrameworkCore;
using ZarmallStore.Application.Extention;
using ZarmallStore.Application.Services.Interface;
using ZarmallStore.Application.Utils;
using ZarmallStore.Data.DTOS.ProductCategoryDto;
using ZarmallStore.Data.DTOS.ProductDto;
using ZarmallStore.Data.Entities.ProductEntities;
using ZarmallStore.Data.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly IGenericRepository<Brand> _brandRepository;
        private readonly IGenericRepository<ProductSelectedBrand> _selectedBrandRepository;
        private readonly IGenericRepository<ProductFeature> _featureRepository;
        private readonly IGenericRepository<ProductGallery> _galleryRepository;
        public ProductService(IGenericRepository<Product> productRepository, IGenericRepository<ProductCategory> categoryRepository, IGenericRepository<ProductColor> colorRepository,
            IGenericRepository<ProductVariant> variantRepository, IGenericRepository<ProductComment> commentRepository, IGenericRepository<ProductSelectedCategory> selectedCategoryRepository,
            IGenericRepository<Brand> brandRepository, IGenericRepository<ProductSelectedBrand> selectedBrandRepository,
            IGenericRepository<ProductFeature> featureRepository, IGenericRepository<ProductGallery> galleryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _colorRepository = colorRepository;
            _variantRepository = variantRepository;
            _commentRepository = commentRepository;
            _selectedCategoryRepository = selectedCategoryRepository;
            _brandRepository = brandRepository;
            _selectedBrandRepository = selectedBrandRepository;
            _featureRepository = featureRepository;
            _galleryRepository = galleryRepository;
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
            await _brandRepository.DisposeAsync();
            await _selectedBrandRepository.DisposeAsync();
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

        public async Task<CreateProductResult> CreateProduct(CreateProductDto dto)
        {
            var product = new Product
            {
                Title = dto.Title,
                Description = dto.Description,
                IsAvailable = dto.IsAvailable,
                ShortDescription = dto.ShortDescription,
            };

            #region Main Image
            var mainImageName = Guid.NewGuid().ToString("N") + Path.GetExtension(dto.MainImage.FileName);
            var res = dto.MainImage.AddImageToServer(mainImageName, PathExtension.ProductImageServer, 150, 150, PathExtension.ProductImageThumbServer);
            if (res)
            {
                product.MainImageName = mainImageName;
            }
            else
            {
                return CreateProductResult.SavingMainImageFailed;
            }
            #endregion

            await _productRepository.AddEntity(product);
            await _productRepository.SaveAsync();

            #region Categories
            foreach (var category in dto.Categories)
            {
                var selectedCategory = await _categoryRepository.GetQuery().FirstOrDefaultAsync(d => d.Id == category);
                if (selectedCategory == null) return CreateProductResult.CategoryNotFound;

                var selected = new ProductSelectedCategory
                {
                    Product = product,
                    Category = selectedCategory,
                    ProductId = product.Id,
                    CategoryId = category
                };
                await _selectedCategoryRepository.AddEntity(selected);
            }
            #endregion

            #region  Brand
            if (dto.BrandId != null)
            {
                var brand = await _brandRepository.GetQuery().FirstOrDefaultAsync(d => d.Id == dto.BrandId);
                if (brand == null) return CreateProductResult.BrandNotFound;
                var selectedBrand = new ProductSelectedBrand
                {
                    Product = product,
                    Brand = brand,
                    BrandId = brand.Id,
                    ProductId = product.Id,

                };
                await _selectedBrandRepository.AddEntity(selectedBrand);
                await _selectedBrandRepository.SaveAsync();
            }
            #endregion

            #region Galleries
            if (dto.ProductGalleries != null && dto.ProductGalleries.Any())
            {
                var galleryOrder = 2;
                foreach (var item in dto.ProductGalleries)
                {
                    var galleryItem = new ProductGallery
                    {
                        ProductId = product.Id,
                        Order = galleryOrder,
                    };

                    //Image

                    var galleryImageName = Guid.NewGuid().ToString("N") + Path.GetExtension(dto.MainImage.FileName);
                    dto.MainImage.AddImageToServer(mainImageName, PathExtension.ProductGalleryServer, 150, 150, PathExtension.ProductGalleryThumbServer);
                    galleryItem.ImageName = galleryImageName;

                    await _galleryRepository.AddEntity(galleryItem);

                    galleryOrder++;
                }

            }
            #endregion

            #region Features
            if(dto.ProductFeatures != null && dto.ProductFeatures.Any())
            {
                foreach (var item in dto.ProductFeatures)
                {
                    var featureOrder = 1;
                    var feature = new ProductFeature
                    {
                        Product = product,
                        ProductId = product.Id,
                        Value = item.Value,
                        Title = item.Title,
                        Order = featureOrder,
                    };
                    await _featureRepository.AddEntity(feature);
                    featureOrder ++;
                };
                await _featureRepository.SaveAsync();
            }
            #endregion

            return CreateProductResult.Success;
        }

        public Task<bool> DeleteCategory(long categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProduct(long productId)
        {
            throw new NotImplementedException();
        }
        public Task<EditProductResult> EditProduct(EditProductDto dto)
        {
            throw new NotImplementedException();
        }
        public Task<FilterProductDto> FilterProduct(FilterProductDto filter)
        {
            throw new NotImplementedException();
        }
        public async Task<EditProductDto> GetEditProduct(long productId)
        {
            var brand = await _selectedBrandRepository.GetQuery().FirstOrDefaultAsync(d => d.ProductId == productId);
            var data = await _productRepository.GetEntityById(productId);
            var model = new EditProductDto
            {
                
                Description = data.Description,
                IsAvailable = data.IsAvailable,
                Title = data.Title,
                ShortDescription = data.ShortDescription,
                ProductId = productId,
                Categories = await _selectedCategoryRepository.GetQuery().Where(d=> d.ProductId == productId)
                                .Select(d=>d.CategoryId).ToListAsync(),
            };

            if (brand != null)
                model.BrandId = brand.Id;
            return model;
        }

        public async Task<ProductDetailDto> ProductDetail(long productId)
        {
            var data = await _productRepository.GetEntityById(productId);
            return new ProductDetailDto
            {
                Id = data.Id,
                Title = data.Title,
                IsDeleted = data.IsDeleted,
                CreatDate = data.CreatDate,
                Description = data.Description,
                IsAvailable = data.IsAvailable,
                MainImageName = data.MainImageName,
                LastUpdateDate = data.LastUpdateDate,
                ShortDescription = data.ShortDescription,
                productVariants = await _variantRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync(),
                ProductSelectedBrand = await _selectedBrandRepository.GetQuery().FirstOrDefaultAsync(d => d.ProductId == productId),
                SelectedCategories = await _selectedCategoryRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync(),
                ProductGalleries = await _galleryRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync(),
                ProductComments = await _commentRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync(),
                ProductFeatures = await _featureRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync(),
            };
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

        #region Color
        public Task<FilterColorDto> FilterColor(FilterColorDto filter)
        {
            throw new NotImplementedException();
        }
        public Task CreateColor(CreateColorDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<EditeColorDto> GetEditeColor(long colorId)
        {
            throw new NotImplementedException();
        }

        public Task EditeColor(EditeColorDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteColor(long colorId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Gallery
        public Task CreateGallery(CreateGalleryDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<EditGalleryDto> GetEditGallery(long galleryId)
        {
            throw new NotImplementedException();
        }

        public Task EditGallery(EditGalleryDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteGallery(long galleryId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Feature
        public Task<bool> DeleteFeature(long featureId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Variant


        public Task CreateProductVariant(CreateProductVariantDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<EditProductVariantDto> GetEditeProductVariant(long VariantId)
        {
            throw new NotImplementedException();
        }

        public Task EditeProductVariant(EditProductVariantDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProductVariant(long VariantId)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
