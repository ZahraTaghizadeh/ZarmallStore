using System.CodeDom;
using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Application.Utils;
using ZarmallStore.Data.DTOs.ProductCategoryDto;
using ZarmallStore.Data.DTOs.ProductDto;
using ZarmallStore.Web.Accessibility;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Areas.Admin.Controllers
{
    [AccessChecker(3)]
    public class ProductController : AdminBaseController
    {
        #region CTOR
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        #endregion

        #region Product
        [HttpGet("filter-products")]
        public async Task<IActionResult> FilterProducts(FilterProductDto filter)
        {
            var model = await _productService.FilterProduct(filter);
            ViewData["Categories"] = await _categoryService.GetAllActiveCategories();
            ViewData["Brands"] = await _productService.GetAllBrands();
            return View(model);
        }

        [HttpGet("product-detail-{productId}")]
        public async Task<IActionResult> ProductDetail(long productId)
        {
            var model = await _productService.ProductDetail(productId);
            return View(model);
        }

        [HttpGet("create-product")]
        public async Task<IActionResult> CreateProduct()
        {
            ViewData["Brands"] = await _productService.GetAllBrands();
            return View();
        }

        [HttpPost("create-product"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductDto dto)
        {
            ViewData["Brands"] = await _productService.GetAllBrands();

            if (!dto.MainImage.IsImage())
            {
                TempData[ErrorMessage] = "فرمت تصویر اصلی محصول قابل قبول نیست";
                return View(dto);
            }

            if (dto.ProductGalleries != null)
            {
                if (dto.ProductGalleries.Any(image => !image.IsImage()))
                {
                    TempData[ErrorMessage] = "فرمت تصویر انتخاب شده برای گالری محصول قابل قبول نیست";
                    return View(dto);
                }
            }

            if (!ModelState.IsValid) return View(dto);

            var res = await _productService.CreateProduct(dto);
            switch (res)
            {
                   
                case CreateProductResult.Error:
                    TempData[ErrorMessage] = "عملیات با خطا مواجه شد";
                    break;
                case CreateProductResult.SavingMainImageFailed:
                    TempData[ErrorMessage] = "ذخیره ی تصویر محصول با خطا مواجه شد";
                    break;
                case CreateProductResult.BrandNotFound:
                    TempData[ErrorMessage] = "برند یافت نشد";
                    break;
                case CreateProductResult.CategoryNotFound:
                    TempData[ErrorMessage] = "دسته بندی انتخاب شده یافت نشد";
                    break;
                case CreateProductResult.Success:
                    TempData[SuccessMessage] = "محصول با موفقیت ایجاد شد.";
                    return RedirectToAction("FilterProducts");
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return View(dto);
        }

        [HttpGet("edit-product")]
        public async Task<IActionResult> EditProduct(long productId)
        {
            ViewData["Brands"] = await _productService.GetAllBrands();

            var model = await _productService.GetEditProduct(productId);
            return View(model);
        }

        [HttpPost("edit-product")]
        public async Task<IActionResult> EditProduct(EditProductDto dto)
        {
            ViewData["Brands"] = await _productService.GetAllBrands();

            if (dto.MainImage != null && !dto.MainImage.IsImage())
            {
                TempData[ErrorMessage] = "فرمت تصویر اصلی محصول قابل قبول نیست";
                return View(dto);
            }

            if (!ModelState.IsValid) return View();

            var res = await _productService.EditProduct(dto);
            switch (res)
            {
                    
                case EditProductResult.Error:
                    TempData[ErrorMessage] = "عملیات با خطا مواجه شد";
                    break;
                case EditProductResult.ImageNotSaved:
                    TempData[ErrorMessage] = "در ذخیره سازی تصویر خطایی رخ داده است";
                    break;
                case EditProductResult.BrandNotFound:
                    TempData[ErrorMessage] = "برند یافت نشد";
                    break;
                case EditProductResult.CategoryNotFound:
                    TempData[ErrorMessage] = "دسته بندی انتخاب شده یافت نشد";
                    break;
                case EditProductResult.Success:
                    TempData[SuccessMessage] = "عملیات با موفقیت انجام شد.";
                    return RedirectToAction("ProductDetail", new {area="Admin" , productId = dto.ProductId});
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return View();
        }

        [Route("delete-product")]
        public async Task<IActionResult> DeleteProduct(long productId)
        {
            var res = await _productService.DeleteProduct(productId);
            if (res)
            {
                TempData[SuccessMessage] = DeleteText;
                return RedirectToAction("FilterProducts");
            }

            TempData[ErrorMessage] = "محصولاتی که سابقا خریداری شده اند امکان حذف شدن ندارند";
            return RedirectToAction("FilterProducts");
        }
        #endregion

        #region Category
        [HttpGet("filter-category")]
        public async Task<IActionResult> FilterCategories(FilterCategoryDto filter)
        {
            var model = await _categoryService.FilterCategory(filter);
            ViewData["ParentCategories"] = await _categoryService.GetAllActiveCategories();
            return View(model);
        }

        [HttpGet("create-category")]
        public async Task<IActionResult> CreateCategory()
        {
            ViewData["ParentCategories"] = await _categoryService.GetAllActiveCategories();
            return View();
        }

        [HttpPost("create-category")]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
        {
            ViewData["ParentCategories"] = await _categoryService.GetAllActiveCategories();

            if (!ModelState.IsValid) return View(dto);

            var res = await _categoryService.CreateCategory(dto);
            if (res)
            {
                TempData[SuccessMessage] = SuccessText;
                return RedirectToAction("FilterCategories");
            }

            TempData[ErrorMessage] = "Url وارد شده تکراری می باشد";
            return View(dto);
        }

        [HttpGet("edit-category")]
        public async Task<IActionResult> EditCategory(long categoryId)
        {
            ViewData["ParentCategories"] = await _categoryService.GetAllActiveCategories();
            var model = await _categoryService.GetEditCategory(categoryId);
            return View(model);
        }

        [HttpPost("edit-category")]
        public async Task<IActionResult> EditCategory(EditCategoryDto dto)
        {
            ViewData["ParentCategories"] = await _categoryService.GetAllActiveCategories();

            if (!ModelState.IsValid) return View(dto);

            var res = await _categoryService.EditCategory(dto);
            if (res)
            {
                TempData[SuccessMessage] = SuccessText;
                return RedirectToAction("FilterCategories");
            }

            TempData[ErrorMessage] = "Url وارد شده تکراری می باشد";
            return View(dto);
        }

        [Route("delete-category")]
        public async Task<IActionResult> DeleteCategory(long categoryId)
        {
            var res = await _categoryService.DeleteCategory(categoryId);
            if (res)
            {
                TempData[SuccessMessage] = DeleteText;
                return RedirectToAction("FilterCategories");
            }

            TempData[ErrorMessage] = "دسته بندی ای که محصولی را شامل میشود امکان حذف شدن ندارد";
            return RedirectToAction("FilterCategories");
        }
        #endregion

        #region Color
        [HttpGet("filter-color")]
        public async Task<IActionResult> FilterColors(FilterColorDto filter)
        {
            var model = await _productService.FilterColor(filter);
            return View(model);
        }

        [HttpGet("create-color")]
        public async Task<IActionResult> CreateColor()
        {
            return View();
        }

        [HttpPost("create-color") , ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateColor(CreateColorDTo dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _productService.CreateColor(dto);
            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("FilterColors");
        }

        [HttpGet("edit-color")]
        public async Task<IActionResult> EditColor(long colorId)
        {
            var model = await _productService.GetEditColor(colorId);
            return View(model);
        }

        [HttpPost("edit-color"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditColor(EditColorDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _productService.EditColor(dto);
            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("FilterColors");
        }

        [Route("delete-color")]
        public async Task<IActionResult> DeleteColor(long colorId)
        {
            var res = await _productService.DeleteColor(colorId);
            if (res)
            {
                TempData[SuccessMessage] = SuccessText;
                return RedirectToAction("FilterColors");
            }

            TempData[ErrorMessage] = "رنگی که در یک نمونه محصول به کار رفته قابل حذف نمی باشد";
            return RedirectToAction("FilterColors");
        }
        #endregion

        #region Features
        [HttpGet("create-feature")]
        public IActionResult CreateFeature(long productId)
        {
            ViewData["ProductId"] = productId;
            return View();
        }

        [HttpPost("create-feature"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFeature(List<CreateFeatureItemDTo> features, long productId)
        {
            if (!ModelState.IsValid) return View();
            await _productService.CreateProductFeatures(features , productId);
            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("ProductDetail", new { area = "Admin", productId = productId });
        }

        [HttpPost("edit-feature"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFeature(EditFeaturesDto dto , long productId)
        {
            if (!ModelState.IsValid) return View(dto);
            await _productService.EditFeature(dto);
            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("ProductDetail", new { area = "Admin", productId = productId });
        }

        [Route("delete-feature")]
        public async Task<IActionResult> DeleteFeature(long featureId , long productId)
        {
            var res = await _productService.DeleteFeature(featureId);
            if (res)
            {
                TempData[SuccessMessage] = SuccessText;
                return RedirectToAction("ProductDetail" , new { area = "Admin",productId = productId});
            }

            TempData[ErrorMessage] = ErrorText;
            return RedirectToAction("ProductDetail", new { area = "Admin", productId = productId });
        }
        #endregion

        #region Variant

        [HttpGet("create-variant")]
        public async Task<IActionResult> CreateVariant(long productId)
        {
            ViewData["ProductId"] = productId;
            var colors = await _productService.GetAllProductColors();
            ViewData["Colors"] = colors;
            if (!colors.Any())
            {
                TempData[InfoMessage] = "پیش از ثبت موجودی محصول باید حداقل یک رنگ اضافه کنید.";
                return RedirectToAction("ProductDetail", new { area="Admin" , productId = productId });
            }

            return View();
        }

        [HttpPost("create-variant"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVariant(List<CreateVariantItemDto> variants, long productId)
        {
            var colors = await _productService.GetAllProductColors();
            ViewData["Colors"] = colors;
            if (!colors.Any())
            {
                TempData[InfoMessage] = "پیش از ثبت موجودی محصول باید حداقل یک رنگ اضافه کنید.";
                return View();
            }

            if (!ModelState.IsValid) return View();

            await _productService.CreateProductVariant(variants , productId);
            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("ProductDetail", new { area = "Admin" , productId = productId });
        }

        [HttpGet("Edit-variant")]
        public async Task<IActionResult> EditVariant(long variantId , long productId)
        {
            var colors = await _productService.GetAllProductColors();
            ViewData["Colors"] = colors;
            if (!colors.Any())
            {
                TempData[InfoMessage] = "پیش از ثبت موجودی محصول باید حداقل یک رنگ اضافه کنید.";
                return View();
            }

            ViewData["ProductId"] = productId;
            var model = await _productService.GetEditProductVariant(variantId);
            return View(model);
        }

        [HttpPost("Edit-variant"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVariant(EditProductVariantDto dto , long productId)
        {
            var colors = await _productService.GetAllProductColors();
            ViewData["Colors"] = colors;
            if (!colors.Any())
            {
                TempData[InfoMessage] = "پیش از ثبت موجودی محصول باید حداقل یک رنگ اضافه کنید.";
                return View();
            }

            if (!ModelState.IsValid) return View(dto);

            var res = await _productService.EditProductVariant(dto);
            if(res)
            {
                TempData[SuccessMessage] = SuccessText;
                return RedirectToAction("ProductDetail", new { area = "Admin",  productId = productId });
            }
            TempData[ErrorMessage] = ErrorText;
            return View(dto);
        }

        [Route("delete-variant")]
        public async Task<IActionResult> DeleteVariant(long variantId , long productId)
        {
            var res = await _productService.DeleteProductVariant(variantId);
            if (res)
            {
                TempData[SuccessMessage] = SuccessText;
                return RedirectToAction("ProductDetail", new { productId = productId });
            }

            TempData[ErrorMessage] = "گونه ای از محصول که سابقا خریداری شده قابل حذف نمی باشد.";
            return RedirectToAction("ProductDetail", new { area = "Admin", productId = productId });
        }
        #endregion

        #region Gallery
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGallery(CreateGalleryDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData[ErrorMessage] = ErrorText;
                return RedirectToAction("ProductDetail", new { area = "Admin", productId = dto.ProductId });
            }

            await _productService.CreateGallery(dto);
            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("ProductDetail", new { area = "Admin", productId = dto.ProductId });
        }

        [HttpPost("Edit-gallery"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGallery(EditGalleryDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            await _productService.EditGallery(dto);
            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("ProductDetail", new { area = "Admin", productId = dto.ProductId });
        }

        [Route("delete-gallery")]
        public async Task<IActionResult> DeleteGallery(long galleryId, long productId)
        {
            var res = await _productService.DeleteGallery(galleryId);
            if (res)
            {
                TempData[SuccessMessage] = SuccessText;
                return RedirectToAction("ProductDetail", new { area = "Admin", productId = productId });
            }

            TempData[ErrorMessage] = "تصویر گالری یافت نشد.";
            return RedirectToAction("ProductDetail", new { area = "Admin", productId = productId });
        }
        #endregion

        #region Brand

        [HttpGet("filter-brand")]
        public async Task<IActionResult> FilterBrand(FilterBrandDto filter)
        {
            var model = await _productService.FilterBrand(filter);
            return View(model);
        }

        [HttpGet("create-brand")]
        public IActionResult CreateBrand()
        {
            return View();
        }

        [HttpPost("create-brand")]
        public async Task<IActionResult> CreateBrand(CreateBrandDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var res = await _productService.CreateBrand(dto);
            if (res)
            {
                TempData[SuccessMessage] = SuccessText;
                return RedirectToAction("FilterBrand");
            }

            TempData[ErrorMessage] = "Url وارد شده تکراری می باشد";
            return View(dto);
        }

        [HttpGet("edit-brand")]
        public async Task<IActionResult> EditBrand(long brandId)
        {
            var model = await _productService.GetEditBrand(brandId);
            return View(model);
        }

        [HttpPost("edit-brand")]
        public async Task<IActionResult> EditBrand(EditBrandDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var res = await _productService.EditBrand(dto);
            if (res)
            {
                TempData[SuccessMessage] = SuccessText;
                return RedirectToAction("FilterBrand");
            }

            TempData[ErrorMessage] = "Url وارد شده تکراری می باشد";
            return View(dto);
        }

        [Route("delete-brand")]
        public async Task<IActionResult> DeleteBrand(long brandId)
        {
            var res = await _productService.DeleteBrand(brandId);
            if (res)
            {
                TempData[SuccessMessage] = DeleteText;
                return RedirectToAction("FilterBrand");
            }

            TempData[ErrorMessage] = "محصولی شامل این برند میشود و امکان حذف برند وجود ندارد.";
            return RedirectToAction("FilterBrand");
        }
        #endregion
    }
}
