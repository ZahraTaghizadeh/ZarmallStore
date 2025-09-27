using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.ProductDto;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Controllers
{
    public class ProductController : SiteBaseController
    {
        #region CTOR
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        #endregion

        #region Filter Products
        [HttpGet("products-list")]
        public async Task<IActionResult> FilterProducts(FilterProductDto filter , string? url)
        {
            filter.TakeEntity = 6;
            if (url != null) filter.categoryUrl = url;
            var model = await _productService.FilterProduct(filter);
            ViewData["Colors"] = await _productService.GetAllProductColors();
            return View(model);
        }
        #endregion

        #region Product Detail
        [HttpGet("product")]
        public async Task<IActionResult> ProductDetail(long productId)
        {
            var model = await _productService.ProductDetail(productId);
            ViewData["SimilarProducts"] = await _productService.GetSimilarProducts(productId);
            
            return View(model);
        }
        #endregion
    }
}
