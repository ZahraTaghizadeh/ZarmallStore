using Microsoft.EntityFrameworkCore;
using ZarmallStore.Application.Extention;
using ZarmallStore.Application.Services.Interface;
using ZarmallStore.Application.Utils;
using ZarmallStore.Data.DTOS.Paging;
using ZarmallStore.Data.DTOS.ProductCategoryDto;
using ZarmallStore.Data.DTOS.ProductDto;
using ZarmallStore.Data.Entities.OrderEntities;
using ZarmallStore.Data.Entities.ProductEntities;
using ZarmallStore.Data.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
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
        private readonly IGenericRepository<OrderDetail> _orderDetailRepository;
        public ProductService(IGenericRepository<Product> productRepository, IGenericRepository<ProductCategory> categoryRepository, IGenericRepository<ProductColor> colorRepository,
            IGenericRepository<ProductVariant> variantRepository, IGenericRepository<ProductComment> commentRepository, IGenericRepository<ProductSelectedCategory> selectedCategoryRepository,
            IGenericRepository<Brand> brandRepository, IGenericRepository<ProductSelectedBrand> selectedBrandRepository,
            IGenericRepository<ProductFeature> featureRepository, IGenericRepository<ProductGallery> galleryRepository, IGenericRepository<OrderDetail> orderRepository)
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
            _orderDetailRepository = orderRepository;
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
            await _orderDetailRepository.DisposeAsync();
        }
        #endregion

        #region Product
        public async Task<FilterProductDto> FilterProduct(FilterProductDto filter)
        {
            #region Query
            var query = _productRepository.GetQuery()
                .Include(d => d.productVariants)
                .Include(d => d.SelectedCategories)
                .ThenInclude(d => d.Category).OrderByDescending(d => d.CreatDate).AsQueryable();
            #endregion

            #region Switch
            switch (filter.ProductOrder)
            {
                case FilterProductOrder.Newest:
                    query = query.OrderByDescending(d => d.CreatDate);
                    break;
                case FilterProductOrder.Oldest:
                    query = query.OrderBy(d => d.CreatDate);
                    break;
                case FilterProductOrder.MostExpensive:
                    query = query.OrderByDescending(d => d.productVariants.OrderByDescending(v => v.Price));
                    break;
                case FilterProductOrder.Cheapest:
                    query = query.OrderBy(d => d.productVariants.OrderByDescending(v => v.Price));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (filter.ProductStatus)
            {
                case FilterProductStatus.All:
                    break;
                case FilterProductStatus.Available:
                    query = query.Where(d => d.IsAvailable);
                    break;
                case FilterProductStatus.NotAvailable:
                    query = query.Where(d => !d.IsAvailable);
                    break;
                case FilterProductStatus.HasStockCount:
                    query = query.Where(d => d.productVariants.Any(v => v.StockCount > 0));
                    break;
                case FilterProductStatus.HasZeroStockCount:
                    query = query.Where(d => !d.productVariants.Any(v => v.StockCount > 0));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion

            #region Filter

            #region Title
            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(p => EF.Functions.Like(p.Title, $"{filter.Title}"));
            }
            #endregion

            #region Price
            if (filter.StartPrice != null)
            {
                query = query.Where(d => d.Price > filter.StartPrice);
            }
            if (filter.EndPrice != null)
            {
                query = query.Where(d => d.Price < filter.EndPrice);
            }
            if (filter is { StartPrice: not null, EndPrice: not null })
            {
                query = query.Where(d => d.Price < filter.EndPrice && d.Price > filter.StartPrice);
            }
            if (query.Any())
            {
                filter.MostPrice = query.OrderByDescending(d => d.Price).First().Price;
                filter.LeastPrice = query.OrderBy(d => d.Price).First().Price;
            }
            #endregion

            #region Specification Ids
            if (filter.CategoryId is > 0)
            {
                query = query.Where(d => d.SelectedCategories.Any(s => s.CategoryId == filter.CategoryId));
            }

            if (filter.ColorId is > 0)
            {
                query = query.Where(d => d.productVariants.Any(v => v.ColorId == filter.ColorId));
            }

            if (filter.BrandId is >0)
            {
                query = query.Where(d=> d.ProductSelectedBrand!=null && d.ProductSelectedBrand.BrandId == filter.BrandId);
            }
            #endregion

            #endregion

            #region Paging
            var pager = Pager.Build(filter.PageId, await query.CountAsync(),filter.TakeEntity,filter.HowManyShowPageAfterAndBefore);
            var allEntities = await query.Paging(pager).ToListAsync();
            #endregion

            return filter.SetData(allEntities).SetPaging(pager);
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
            var addCategoriesResult = await AddProductSelectedCategories(dto.Categories, product.Id);
            if (!addCategoriesResult)
                return CreateProductResult.CategoryNotFound;
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
            if (dto.ProductFeatures != null && dto.ProductFeatures.Any())
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
                    featureOrder++;
                };
                await _featureRepository.SaveAsync();
            }
            #endregion

            return CreateProductResult.Success;
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
                Categories = await _selectedCategoryRepository.GetQuery().Where(d => d.ProductId == productId)
                                .Select(d => d.CategoryId).ToListAsync(),
            };

            if (brand != null)
                model.BrandId = brand.Id;
            return model;
        }

        public async Task<EditProductResult> EditProduct(EditProductDto dto)
        {
            var product = await _productRepository.GetQuery().FirstOrDefaultAsync(d => d.Id == dto.ProductId);
            if (product == null) return EditProductResult.Error;
            product.Title = dto.Title;
            product.Description = dto.Description;
            product.ShortDescription = dto.ShortDescription;
            product.IsAvailable = dto.IsAvailable;

            #region Brand
            if (dto.BrandId != null)
            {
                var brand = await _brandRepository.GetQuery().FirstOrDefaultAsync(d => d.Id == dto.BrandId);
                if (brand == null) return EditProductResult.BrandNotFound;
                var oldBrand = await _selectedBrandRepository.GetEntityById((long)dto.BrandId);
                await _selectedBrandRepository.DeletePermanent(oldBrand);
                var newBrand = new ProductSelectedBrand
                {
                    Product = product,
                    Brand = brand,
                    BrandId = brand.Id,
                    ProductId = dto.ProductId,
                };

                await _selectedBrandRepository.AddEntity(newBrand);
                await _selectedBrandRepository.SaveAsync();
            }

            #endregion

            #region Category
            await RemoveProductSelectedCategories(dto.ProductId);
            var addCategoryResult = await AddProductSelectedCategories(dto.Categories, dto.ProductId);
            if (!addCategoryResult) return EditProductResult.CategoryNotFound;
            #endregion

            #region Main Image
            if (dto.MainImage != null)
            {

                var mainImageName = Guid.NewGuid().ToString("N") + Path.GetExtension(dto.MainImage.FileName);
                var res = dto.MainImage.AddImageToServer(mainImageName, PathExtension.ProductImageServer, 150, 150, PathExtension.ProductImageThumbServer, product.MainImageName);
                if (res)
                {
                    product.MainImageName = mainImageName;
                }
                else
                {
                    return EditProductResult.ImageNotSave;
                }

            }
            #endregion

            _productRepository.EditEntity(product);
            await _productRepository.SaveAsync();
            return EditProductResult.Success;
        }

        public async Task<bool> DeleteProduct(long productId)
        {
            #region Order
            var productOrdered = await _orderDetailRepository.GetQuery().AnyAsync(d => d.ProductId == productId);
            if (productOrdered) return false;
            #endregion

            #region Features
            var features = await _featureRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync();
            _featureRepository.DeletePermanentEntities(features);
            await _featureRepository.SaveAsync();
            #endregion

            #region Categories
            var categories = await _selectedCategoryRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync();
            _selectedCategoryRepository.DeletePermanentEntities(categories);
            await _featureRepository.SaveAsync();
            #endregion

            #region Galleties
            var galleries = await _galleryRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync();
            if (galleries.Any())
            {
                foreach (var item in galleries)
                {
                    item.ImageName.DeleteImage(PathExtension.ProductGalleryImage, PathExtension.ProductGalleryThumb);
                }
                _galleryRepository.DeletePermanentEntities(galleries);
                await _galleryRepository.SaveAsync();
            }
            #endregion

            #region Comment
            var Comments = await _commentRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync();
            _commentRepository.DeletePermanentEntities(Comments);
            await _commentRepository.SaveAsync();

            #endregion

            var product = await _productRepository.GetEntityById(productId);
            _productRepository.DeleteEntity(product);
            await _productRepository.SaveAsync();
            return true;
        }


        #endregion

        #region Category
        public async Task<bool> AddProductSelectedCategories(List<long> selectedCategories, long productId)
        {
            foreach (var category in selectedCategories)
            {
                var selectedCategory = await _categoryRepository.GetQuery().FirstOrDefaultAsync(d => d.Id == category);
                if (selectedCategory == null) return false;

                var selected = new ProductSelectedCategory
                {
                    Product = await _productRepository.GetEntityById(productId),
                    Category = selectedCategory,
                    ProductId = productId,
                    CategoryId = category
                };
                await _selectedCategoryRepository.AddEntity(selected);
            }
            await _selectedCategoryRepository.SaveAsync();
            return true;
        }
        public async Task RemoveProductSelectedCategories(long productId)
        {
            var selectedCategories = await _selectedCategoryRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync();
            _selectedCategoryRepository.DeletePermanentEntities(selectedCategories);
            await _selectedCategoryRepository.SaveAsync();
        }
        public async Task<bool> CreateCategory(CreateCategoryDto dto)
        {
            #region Check Url
            var urlInUse = await _categoryRepository.GetQuery().AnyAsync(c => c.Url == dto.Url);
            if (urlInUse) return false;
            #endregion
            var category = new ProductCategory
            {
                Title = dto.Title,
                IsActive = true,
                Order = dto.Order,
                ParentId = dto.ParentId,
                Url = dto.Url,
            };
            await _categoryRepository.AddEntity(category);
            await _categoryRepository.SaveAsync();
            return true;
        }
        public async Task<bool> EditCategory(EditCategoryDto dto)
        {
            #region Check Url
            var urlInUse = await _categoryRepository.GetQuery().AnyAsync(c => c.Url == dto.Url);
            if (urlInUse) return false;
            #endregion

            var data = await _categoryRepository.GetEntityById(dto.CategoryId);
            data.Title = dto.Title;
            data.Url = dto.Url;
            data.ParentId = dto.ParentId;
            data.Order = dto.Order;
            data.IsActive = dto.IsActive;

            _categoryRepository.EditEntity(data);
            await _categoryRepository.SaveAsync();
            return true;
        }
        public async Task<FilterCategoryDto> FilterCategory(FilterCategoryDto filter)
        {
            #region query
            var query = _categoryRepository.GetQuery().OrderByDescending(d => d.CreatDate).AsQueryable();
            #endregion

            #region Switch
            switch (filter.CategoryStatus)
            {
                case FilterCategoryStatus.All:
                    break;
                case FilterCategoryStatus.Active:
                    query = query.Where(d => d.IsActive);
                    break;
                case FilterCategoryStatus.DeActive:
                    query = query.Where(d => !d.IsActive);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion

            #region Filter
            if(string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(c => EF.Functions.Like(c.Title, $"{filter.Title}"));
            }
            #endregion

            #region Paging
            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.HowManyShowPageAfterAndBefore);
            var allEntities = await query.Paging(pager).ToListAsync();
            #endregion

            return filter.SetData(allEntities).SetPaging(pager);
        }

        public async Task<EditCategoryDto> GetEditCategory(long categoryId)
        {
            var data = await _categoryRepository.GetEntityById(categoryId);
            return new EditCategoryDto
            {
                Title = data.Title,
                Order = data.Order,
                Url = data.Url,
                CategoryId = data.Id,
                IsActive = data.IsActive,
                ParentId = data.ParentId
            };
        }
        public async Task<bool> DeleteCategory(long categoryId)
        {
            var categoryInUse = await _selectedCategoryRepository.GetQuery().AnyAsync(d => d.CategoryId == categoryId);
            if (categoryInUse) return false;
            var data = await _categoryRepository.GetEntityById(categoryId);
            _categoryRepository.DeleteEntity(data);
            return true;
        }

        #endregion

        #region Color
        public async Task<FilterColorDto> FilterColor(FilterColorDto filter)
        {
            var query = _colorRepository.GetQuery().OrderByDescending(d => d.CreatDate).AsQueryable();    
            #region Filter
            if (string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(c => EF.Functions.Like(c.Title, $"{filter.Title}"));
            }
            #endregion

            #region Paging
            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.HowManyShowPageAfterAndBefore);
            var allEntities = await query.Paging(pager).ToListAsync();
            #endregion

            return filter.SetData(allEntities).SetPaging(pager);
        }
        public async Task<List<ProductColor>> GetAllProductColors()
        {
           return await _colorRepository.GetQuery().ToListAsync();
        }
        public async Task CreateColor(CreateColorDto dto)
        {
            var color = new ProductColor
            {
                ColorCode = dto.ColorCode,
                Title = dto.Title,
            };
            await _colorRepository.AddEntity(color);
            await _colorRepository.SaveAsync();
        }
        public async Task<EditColorDto> GetEditColor(long colorId)
        {
            var data = await _colorRepository.GetEntityById(colorId);
            return new EditColorDto
            {
                Title = data.Title,
                ColorCode = data.ColorCode,
                ColorId = data.Id
            };
        }

        public async Task EditColor(EditColorDto dto)
        {
            var data = await _colorRepository.GetEntityById(dto.ColorId);
            data.Title = dto.Title;
            data.ColorCode = dto.ColorCode;
            _colorRepository.EditEntity(data);
            await _colorRepository.SaveAsync();
        }

        public async Task<bool> DeleteColor(long colorId)
        {
            var inUse = await _variantRepository.GetQuery().AnyAsync(c => c.ColorId == colorId);
            if(inUse) return false;
            var data = await _colorRepository.GetEntityById(colorId);
            _colorRepository.DeleteEntity(data);
            await _colorRepository.SaveAsync();
            return true;
        }
        #endregion

        #region Gallery
        public async Task CreateGallery(CreateGalleryDto dto)
        {
            var gallery = new ProductGallery
            {
                Order = dto.Order,
                ProductId = dto.ProductId,
            };

            #region Main Image
            var mainImageName = Guid.NewGuid().ToString("N") + Path.GetExtension(dto.ImageName.FileName);
            dto.ImageName.AddImageToServer(mainImageName, PathExtension.ProductGalleryServer, 150, 150, PathExtension.ProductGalleryThumbServer);
            gallery.ImageName = mainImageName;
            #endregion

            await _galleryRepository.AddEntity(gallery);
            await _galleryRepository.SaveAsync();
        }

        public async Task<EditGalleryDto> GetEditGallery(long galleryId)
        {
            var data = await _galleryRepository.GetEntityById(galleryId);
            return new EditGalleryDto
            {
                Order = data.Order,
                GalleryId = data.Id
            };
        }

        public async Task EditGallery(EditGalleryDto dto)
        {
            var data = await _galleryRepository.GetEntityById(dto.GalleryId);
            data.Order = dto.Order;
            _galleryRepository.EditEntity(data);
            await _galleryRepository.SaveAsync();
        }

        public async Task<bool> DeleteGallery(long galleryId)
        {
            var data = await _galleryRepository.GetQuery().FirstOrDefaultAsync(d => d.Id == galleryId);
            if (data == null ) return false;
            data.ImageName.DeleteImage(PathExtension.ProductGalleryImage, PathExtension.ProductGalleryThumb);
            await _galleryRepository.DeletePermanent(data);
            await _galleryRepository.SaveAsync();
            return true;
        }
        #endregion

        #region Feature
        public async Task<bool> DeleteFeature(long featureId)
        {
            var data = await _featureRepository.GetQuery().FirstOrDefaultAsync(d => d.Id == featureId);
            if (data == null) return false;
            await _featureRepository.DeletePermanent(data);
            await _featureRepository.SaveAsync();
            return true;
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
