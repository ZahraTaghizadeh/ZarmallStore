using ZarmallStore.Application.Extensions;
using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Application.Utils;
using ZarmallStore.Data.DTOs.CommonDto;
using ZarmallStore.Data.DTOs.Paging;
using ZarmallStore.Data.DTOs.ProductDto;
using ZarmallStore.Data.Entities.OrderEntities;
using ZarmallStore.Data.Entities.ProductEntities;
using ZarmallStore.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ZarmallStore.Application.Services.Implementations
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
        private readonly IGenericRepository<ProductSell> _productSellRepository;
        private readonly ICategoryService _categoryService;
        private readonly IMemoryCache _cache;

        public ProductService(IGenericRepository<Product> productRepository, IGenericRepository<ProductCategory> categoryRepository, IGenericRepository<ProductColor> colorRepository, IGenericRepository<ProductVariant> variantRepository, IGenericRepository<ProductComment> commentRepository, IGenericRepository<ProductSelectedCategory> selectedCategoryRepository, IGenericRepository<Brand> brandRepository, IGenericRepository<ProductSelectedBrand> selectedBrandRepository, IGenericRepository<ProductFeature> featureRepository, IGenericRepository<ProductGallery> galleryRepository, IGenericRepository<OrderDetail> orderDetailRepository, IGenericRepository<ProductSell> productSellRepository, ICategoryService categoryService, IMemoryCache cache)
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
            _orderDetailRepository = orderDetailRepository;
            _productSellRepository = productSellRepository;
            _categoryService = categoryService;
            _cache = cache;
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
            await _productSellRepository.DisposeAsync();
        }
        #endregion

        #region Product
        public async Task<FilterProductDto> FilterProduct(FilterProductDto filter)
        {
            #region Query
            var query = _productRepository.GetQuery()
                .Include(d => d.ProductVariants)
                .Include(d => d.SelectedCategories)
                .ThenInclude(d => d.Category).AsQueryable();
            #endregion

            #region Switch
            switch (filter.ProductOrder)
            {
                case FilterProductOrder.Newest:
                    query = query.OrderByDescending(d => d.CreateDate);
                    break;
                case FilterProductOrder.Oldest:
                    query = query.OrderBy(d => d.CreateDate);
                    break;
                case FilterProductOrder.MostExpensive:
                    query = query.OrderByDescending(d => d.BasePrice);
                    break;
                case FilterProductOrder.Cheapest:
                    query = query.OrderBy(d => d.BasePrice);
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
                    query = query.Where(d => d.ProductVariants.Any(v => v.StockCount > 0));
                    break;
                case FilterProductStatus.HasZeroStockCount:
                    query = query.Where(d => !d.ProductVariants.Any(v => v.StockCount > 0));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion

            #region Filter

            #region Title
            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(p => EF.Functions.Like(p.Title, $"%{filter.Title}%"));
            }

            if (!string.IsNullOrEmpty(filter.categoryUrl))
            {
                query = query.Where(d => d.SelectedCategories.Any(s => s.Category.Url == filter.categoryUrl));
            }
            #endregion

            #region Price
            if (filter.StartPrice != null)
            {
                query = query.Where(d => d.BasePrice > filter.StartPrice);
            }

            if (filter.EndPrice != null)
            {
                query = query.Where(d => d.BasePrice < filter.EndPrice);
            }

            if (filter is { StartPrice: not null, EndPrice: not null })
            {
                query = query.Where(d => d.BasePrice > filter.StartPrice && d.BasePrice < filter.EndPrice);
            }

            if (query.Any())
            {
                filter.MostPrice = query.OrderByDescending(d => d.BasePrice).First().BasePrice;
                filter.LeastPrice = query.OrderBy(d => d.BasePrice).First().BasePrice;
            }
            #endregion

            #region Specification Ids
            if (filter.CategoryId is > 0)
            {
                query = query.Where(d => d.SelectedCategories.Any(s => s.CategoryId == filter.CategoryId));
            }

            if (filter.ColorId is > 0)
            {
                query = query.Where(d => d.ProductVariants.Any(v => v.ColorId == filter.ColorId));
            }

            if (filter.BrandId is > 0)
            {
                query = query.Where(d => d.ProductSelectedBrand != null && d.ProductSelectedBrand.BrandId == filter.BrandId);
            }
            #endregion

            #endregion

            #region Paging
            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.HowManyShowPageAfterAndBefore);
            var allEntities = await query.Paging(pager).ToListAsync();
            #endregion

            return filter.SetData(allEntities).SetPaging(pager);
        }

        public async Task<List<ProductItemDto>> GetNewProducts()
        {
            var products = await _cache.GetOrCreateAsync("NewProducts", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                var newProducts = await _productRepository.GetQuery()
                    .OrderByDescending(p => p.CreateDate)
                    .Take(10)
                    .Select(p => new ProductItemDto
                    {
                        ProductId = p.Id,
                        Title = p.Title,
                        Image = p.MainImageName,
                        Price = p.BasePrice,
                    })
                    .ToListAsync();

                return newProducts;
            });
            return products ?? new List<ProductItemDto>();
        }

        public async Task<List<ProductItemDto>> BestSellerProducts()
        {
            var products = await _cache.GetOrCreateAsync("BestSellerProducts", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                var bestSellers = await _productSellRepository.GetQuery()
                    .GroupBy(ps => ps.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalSold = g.Sum(ps => ps.SellCount)
                    })
                    .OrderByDescending(x => x.TotalSold)
                    .Take(10)
                    .Join(_productRepository.GetQuery(),
                        sale => sale.ProductId,
                        product => product.Id,
                        (sale, product) => new ProductItemDto
                        {
                            ProductId = product.Id,
                            Title = product.Title,
                            Image = product.MainImageName,
                            TotalSold = sale.TotalSold,
                            Price = product.BasePrice
                        })
                    .ToListAsync();

                return bestSellers;
            });
            return products ?? new List<ProductItemDto>();
        }

        public async Task<List<Product>> GetSimilarProducts(long productId)
        {
            var product = await _productRepository.GetQuery().Include(d => d.SelectedCategories)
                .FirstAsync(d => d.Id == productId);

            return await _productRepository.GetQuery()
                .Where(p => p.SelectedCategories.Any(c =>
                    c.CategoryId == product.SelectedCategories.First().CategoryId) && p.Id != productId)
                .OrderByDescending(p => p.CreateDate).Take(10).ToListAsync();
        }

        public async Task<ProductDetailDto> ProductDetail(long productId)
        {
            var data = await _productRepository.GetEntityById(productId);

            var model = new ProductDetailDto
            {
                Id = data.Id,
                Title = data.Title,
                IsDeleted = data.IsAvailable,
                LastUpdateDate = data.LastUpdateDate,
                CreateDate = data.CreateDate,
                Description = data.Description,
                MainImageName = data.MainImageName,
                IsAvailable = data.IsAvailable,
                ShortDescription = data.ShortDescription,
                BasePrice = data.BasePrice,
                ProductVariants = await _variantRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync(),
                SelectedCategories = await _selectedCategoryRepository.GetQuery().Include(c => c.Category).Where(d => d.ProductId == productId).ToListAsync(),
                ProductGalleries = await _galleryRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync(),
                ProductComments = await _commentRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync(),
                ProductFeatures = await _featureRepository.GetQuery().Where(d => d.ProductId == productId).ToListAsync(),
            };

            var selectedBrand =
                await _selectedBrandRepository.GetQuery().FirstOrDefaultAsync(b => b.ProductId == productId);
            if (selectedBrand != null)
            {
                model.ProductSelectedBrand = selectedBrand.Brand;
            }

            return model;
        }

        public async Task<CreateProductResult> CreateProduct(CreateProductDto dto)
        {
            #region Create Product
            var convertedPrice = int.Parse(dto.BasePrice.Replace(",", ""));

            var product = new Product
            {
                Title = dto.Title,
                Description = dto.Description,
                ShortDescription = dto.ShortDescription,
                IsAvailable = dto.IsAvailable,
                BasePrice = convertedPrice
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
            var addCategoriesResult = await _categoryService.AddProductSelectedCategories(dto.Categories, product.Id);
            if (!addCategoriesResult) return CreateProductResult.CategoryNotFound;
            #endregion

            #region Brand
            if (dto.BrandId is > 0)
            {
                var brand = await _brandRepository.GetQuery().FirstOrDefaultAsync(d => d.Id == dto.BrandId);
                if (brand == null) return CreateProductResult.BrandNotFound;

                var selectedBrand = new ProductSelectedBrand
                {
                    Product = product,
                    Brand = brand,
                    BrandId = brand.Id,
                    ProductId = product.Id
                };
                await _selectedBrandRepository.AddEntity(selectedBrand);
                await _selectedBrandRepository.SaveAsync();
            }
            #endregion

            #region Galleries
            if (dto.ProductGalleries != null && dto.ProductGalleries.Any())
            {
                var galleryOrder = 1;
                foreach (var item in dto.ProductGalleries)
                {
                    var galleryItem = new ProductGallery
                    {
                        ProductId = product.Id,
                        Order = galleryOrder
                    };

                    //Image
                    var galleryImageName = Guid.NewGuid().ToString("N") + Path.GetExtension(item.FileName);
                    item.AddImageToServer(galleryImageName, PathExtension.ProductGalleryServer, 150, 150, PathExtension.ProductGalleryThumbServer);
                    galleryItem.ImageName = galleryImageName;

                    await _galleryRepository.AddEntity(galleryItem);
                    await _galleryRepository.SaveAsync();
                    galleryOrder++;
                }
            }
            #endregion

            #endregion

            #region Cache new Product
            var cachedProducts = _cache.Get<List<ProductItemDto>>("NewProducts");
            if (cachedProducts == null) return CreateProductResult.Success;

            var newProductDto = new ProductItemDto
            {
                ProductId = product.Id,
                Title = product.Title,
                Image = product.MainImageName,
                Price = product.BasePrice,
                TotalSold = 0 
            };

            cachedProducts.Add(newProductDto);

            cachedProducts = cachedProducts
                .OrderByDescending(p => p.ProductId) 
                .Take(10)
                .ToList();

            _cache.Set("NewProducts", cachedProducts, TimeSpan.FromDays(1));
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
                BasePrice = data.BasePrice.ToString(),
                Categories = await _selectedCategoryRepository.GetQuery().Where(d => d.ProductId == productId)
                    .Select(d => d.CategoryId).ToListAsync()
            };

            if (brand != null)
            {
                model.BrandId = brand.BrandId;
            }

            return model;
        }

        public async Task<EditProductResult> EditProduct(EditProductDto dto)
        {
            #region Edit Product
            var product = await _productRepository.GetQuery().FirstOrDefaultAsync(d => d.Id == dto.ProductId);
            if (product == null) return EditProductResult.Error;

            var convertedPrice = int.Parse(dto.BasePrice.Replace(",", ""));

            product.Title = dto.Title;
            product.Description = dto.Description;
            product.ShortDescription = dto.ShortDescription;
            product.IsAvailable = dto.IsAvailable;
            product.BasePrice = convertedPrice;

            #region Brand
            switch (dto.BrandId)
            {
                case > 0:
                    {
                        var brand = await _brandRepository.GetQuery().FirstOrDefaultAsync(d => d.Id == dto.BrandId);
                        if (brand == null) return EditProductResult.BrandNotFound;

                        var oldBrand = await _selectedBrandRepository.GetQuery().FirstOrDefaultAsync(b => b.ProductId == product.Id);
                        if (oldBrand != null)
                        {
                            await _selectedBrandRepository.DeletePermanent(oldBrand);
                        }

                        var newBrand = new ProductSelectedBrand
                        {
                            Product = product,
                            Brand = brand,
                            BrandId = brand.Id,
                            ProductId = dto.ProductId
                        };
                        await _selectedBrandRepository.AddEntity(newBrand);
                        await _selectedBrandRepository.SaveAsync();
                        break;
                    }
                case 0:
                    {
                        var selectedBrand = await _selectedBrandRepository.GetQuery()
                            .FirstOrDefaultAsync(b => b.ProductId == product.Id);

                        if (selectedBrand != null)
                        {
                            await _selectedBrandRepository.DeletePermanent(selectedBrand);
                            await _selectedBrandRepository.SaveAsync();
                        }
                        break;
                    }
            }
            #endregion

            #region Category
            await _categoryService.RemoveProductSelectedCategories(dto.ProductId);
            var addCategoryResult = await _categoryService.AddProductSelectedCategories(dto.Categories, dto.ProductId);
            if (!addCategoryResult) return EditProductResult.CategoryNotFound;
            #endregion

            #region Main Image
            if (dto.MainImage != null)
            {
                #region Main Image
                var mainImageName = Guid.NewGuid().ToString("N") + Path.GetExtension(dto.MainImage.FileName);
                var res = dto.MainImage.AddImageToServer(mainImageName, PathExtension.ProductImageServer, 150, 150, PathExtension.ProductImageThumbServer, product.MainImageName);
                if (res)
                {
                    product.MainImageName = mainImageName;
                }
                else
                {
                    return EditProductResult.ImageNotSaved;
                }
                #endregion
            }
            #endregion

            _productRepository.EditEntity(product);
            await _productRepository.SaveAsync();
            #endregion

            #region Cahce Edited Product If it is in BestSeller Product or new Products
            // NewProducts Cached List
            var newProductsCache = _cache.Get<List<ProductItemDto>>("NewProducts");
            if (newProductsCache != null)
            {
                var productInNewProducts = newProductsCache.FirstOrDefault(p => p.ProductId == product.Id);
                if (productInNewProducts != null)
                {
                    productInNewProducts.Title = product.Title;
                    productInNewProducts.Image = product.MainImageName;
                    productInNewProducts.Price = product.BasePrice;
                    _cache.Set("NewProducts", newProductsCache, TimeSpan.FromDays(1));
                }
            }

            //BestSeller Products Cached List
            var bestSellerCache = _cache.Get<List<ProductItemDto>>("BestSellerProducts");
            if (bestSellerCache != null)
            {
                var productInBestSellers = bestSellerCache.FirstOrDefault(p => p.ProductId == product.Id);
                if (productInBestSellers != null)
                {
                    productInBestSellers.Title = product.Title;
                    productInBestSellers.Image = product.MainImageName;
                    _cache.Set("BestSellerProducts", bestSellerCache, TimeSpan.FromDays(1));
                }
            }
            #endregion

            return EditProductResult.Success;
        }

        public async Task<bool> DeleteProduct(long productId)
        {
            #region Check Cached Products
            // New Products
            var cachedNewProducts = _cache.Get<List<ProductItemDto>>("NewProducts");

            if (cachedNewProducts != null && cachedNewProducts.Any(p => p.ProductId == productId))
            {
                _cache.Remove("NewProducts");
            }

            // Best Seller Products
            var cachedBestSellerProducts = _cache.Get<List<ProductItemDto>>("BestSellerProducts");

            if (cachedBestSellerProducts != null && cachedBestSellerProducts.Any(p => p.ProductId == productId))
            {
                _cache.Remove("BestSellerProducts");
            }
            #endregion

            var product = await _productRepository.GetEntityById(productId);

            #region Order
            var productOrdered = await _orderDetailRepository.GetQuery().Include(d => d.ProductVariant)
               .AnyAsync(d => d.ProductVariant.ProductId == productId);
            if (productOrdered) return false;
            #endregion

            #region Features
            var features = await _featureRepository.GetQuery().Where(d => d.ProductId == productId)
                .ToListAsync();
            _featureRepository.DeletePermanentEntities(features);
            await _featureRepository.SaveAsync();
            #endregion

            #region Categories
            var categories = await _selectedCategoryRepository.GetQuery().Where(d => d.ProductId == productId)
                .ToListAsync();
            _selectedCategoryRepository.DeletePermanentEntities(categories);
            await _selectedCategoryRepository.SaveAsync();
            #endregion

            #region Galleries
            var galleries = await _galleryRepository.GetQuery().Where(d => d.ProductId == productId)
                .ToListAsync();
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

            #region MainImage
            product.MainImageName.DeleteImage(PathExtension.ProductImageImage, PathExtension.ProductImageThumb);
            #endregion

            #region Comment
            var comments = await _commentRepository.GetQuery().Where(d => d.ProductId == productId)
                .ToListAsync();
            _commentRepository.DeletePermanentEntities(comments);
            await _commentRepository.SaveAsync();
            #endregion

            _productRepository.DeleteEntity(product);
            await _productRepository.SaveAsync();
            return true;
        }
        #endregion

        #region Color
        public async Task<FilterColorDto> FilterColor(FilterColorDto filter)
        {
            var query = _colorRepository.GetQuery().OrderByDescending(d => d.CreateDate)
                .AsQueryable();

            #region Filter
            if (!string.IsNullOrEmpty(filter.Title))
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

        public async Task CreateColor(CreateColorDTo dto)
        {
            var color = new ProductColor
            {
                ColorCode = dto.ColorCode,
                Title = dto.Title
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
            if (inUse) return false;

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
                ProductId = dto.ProductId
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
            if (data == null) return false;

            data.ImageName.DeleteImage(PathExtension.ProductGalleryImage, PathExtension.ProductGalleryThumb);
            await _galleryRepository.DeletePermanent(data);
            await _galleryRepository.SaveAsync();
            return true;
        }



        #endregion

        #region Feature
        public async Task CreateProductFeatures(List<CreateFeatureItemDTo> features, long productId)
        {
            var product = await _productRepository.GetEntityById(productId);

            var output = features.Select(item => new ProductFeature
            {
                Order = item.Order,
                Title = item.Title,
                Value = item.Value,
                ProductId = productId,
                Product = product
            })
                .ToList();

            await _featureRepository.AddRangeEntities(output);
            await _featureRepository.SaveAsync();
        }

        public async Task<EditFeaturesDto> GetEditFeature(long featureId)
        {
            var data = await _featureRepository.GetEntityById(featureId);
            return new EditFeaturesDto
            {
                Order = data.Order,
                Title = data.Title,
                FeatureId = data.Id,
                Value = data.Value
            };
        }

        public async Task EditFeature(EditFeaturesDto dto)
        {
            var data = await _featureRepository.GetEntityById(dto.FeatureId);
            data.Title = dto.Title;
            data.Order = dto.Order;
            data.Value = dto.Value;
            _featureRepository.EditEntity(data);
            await _featureRepository.SaveAsync();
        }

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
        public async Task CreateProductVariant(List<CreateVariantItemDto> variants, long productId)
        {
            var product = await _productRepository.GetEntityById(productId);
            var output = new List<ProductVariant>();

            foreach (var item in variants)
            {
                var color = await _colorRepository.GetEntityById(item.ColorId);
                var variant = new ProductVariant
                {
                    ColorId = item.ColorId,
                    Price = int.Parse(item.Price.Replace(",", "")),
                    StockCount = item.StockCount,
                    ProductId = productId,
                    Product = product,
                    ColorCode = color.ColorCode,
                    ColorTitle = color.Title
                };
                output.Add(variant);
            }

            await _variantRepository.AddRangeEntities(output);
            await _variantRepository.SaveAsync();
        }

        public async Task<EditProductVariantDto> GetEditProductVariant(long variantId)
        {
            var data = await _variantRepository.GetEntityById(variantId);
            return new EditProductVariantDto
            {
                Price = data.Price.ToString(),
                ColorId = data.ColorId,
                StockCount = data.StockCount,
                VariantId = data.Id
            };
        }

        public async Task<bool> EditProductVariant(EditProductVariantDto dto)
        {
            var data = await _variantRepository.GetQuery().FirstOrDefaultAsync(v => v.Id == dto.VariantId);
            var color = await _colorRepository.GetQuery().FirstOrDefaultAsync(c => c.Id == dto.ColorId);
            if (data == null || color == null) return false;

            data.Price = int.Parse(dto.Price.Replace(",", ""));
            data.ColorId = dto.ColorId;
            data.StockCount = dto.StockCount;
            data.ColorCode = color.ColorCode;
            data.ColorTitle = color.Title;

            _variantRepository.EditEntity(data);
            await _variantRepository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteProductVariant(long variantId)
        {
            var inUse = await _orderDetailRepository.GetQuery().AnyAsync(v => v.ProductVariantId == variantId);
            if (inUse) return false;

            var data = await _variantRepository.GetEntityById(variantId);
            await _variantRepository.DeletePermanent(data);
            await _variantRepository.SaveAsync();
            return true;
        }
        #endregion

        #region Brands
        public async Task<List<Brand>> GetAllBrands()
        {
            return await _brandRepository.GetQuery().ToListAsync();
        }

        public async Task<FilterBrandDto> FilterBrand(FilterBrandDto filter)
        {
            #region query
            var query = _brandRepository.GetQuery()
                .OrderByDescending(d => d.CreateDate).AsQueryable();
            #endregion

            #region Filter
            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(c => EF.Functions.Like(c.Title, $"{filter.Title}"));
            }

            if (!string.IsNullOrEmpty(filter.Url))
            {
                query = query.Where(c => EF.Functions.Like(c.Url, $"{filter.Url}"));
            }
            #endregion

            #region Paging
            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.HowManyShowPageAfterAndBefore);
            var allEntities = await query.Paging(pager).ToListAsync();
            #endregion

            return filter.SetData(allEntities).SetPaging(pager);
        }

        public async Task<bool> CreateBrand(CreateBrandDto dto)
        {
            var data = new Brand
            {
                Url = dto.Url,
                Order = dto.Order,
                Title = dto.Title,
            };

            #region Main Image
            var mainImageName = Guid.NewGuid().ToString("N") + Path.GetExtension(dto.ImageName.FileName);
            var res = dto.ImageName.AddImageToServer(mainImageName, PathExtension.BrandServer, 150, 150, PathExtension.BrandThumbServer);
            if (res)
            {
                data.ImageName = mainImageName;
            }
            else
            {
                return false;
            }
            #endregion

            await _brandRepository.AddEntity(data);
            await _brandRepository.SaveAsync();
            return true;
        }

        public async Task<bool> EditBrand(EditBrandDto dto)
        {
            var data = await _brandRepository.GetEntityById(dto.BranId);

            data.Title = dto.Title;
            data.Order = dto.Order;
            data.Url = dto.Url;

            if (dto.ImageName != null)
            {
                #region Main Image
                var mainImageName = Guid.NewGuid().ToString("N") + Path.GetExtension(dto.ImageName.FileName);
                var res = dto.ImageName.AddImageToServer(mainImageName, PathExtension.BrandServer, 150, 150, PathExtension.BrandThumbServer);
                if (res)
                {
                    data.ImageName = mainImageName;
                }
                else
                {
                    return false;
                }
                #endregion
            }

            _brandRepository.EditEntity(data);
            await _brandRepository.SaveAsync();
            return true;
        }

        public async Task<EditBrandDto> GetEditBrand(long brandId)
        {
            var data = await _brandRepository.GetEntityById(brandId);
            return new EditBrandDto
            {
                Order = data.Order,
                Title = data.Title,
                Url = data.Url,
                BranId = data.Id
            };
        }

        public async Task<bool> DeleteBrand(long brandId)
        {
            var inUser = await _selectedBrandRepository.GetQuery().AnyAsync(d => d.BrandId == brandId);
            if (inUser) return false;

            var data = await _brandRepository.GetEntityById(brandId);
            _brandRepository.DeleteEntity(data);
            await _brandRepository.SaveAsync();
            return true;
        }



        #endregion
    }
}
