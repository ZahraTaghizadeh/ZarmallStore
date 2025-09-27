using ZarmallStore.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Controllers;

public class HomeController : SiteBaseController
{
    #region CTOR
    private readonly ICommonService _commonService;
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;

    public HomeController(ICommonService commonService, ICategoryService categoryService, IProductService productService)
    {
        _commonService = commonService;
        _categoryService = categoryService;
        _productService = productService;
    }
    #endregion

    public async Task<IActionResult> Index()
    {
        ViewData["Banners"] = await _commonService.GetHomeBanners();
        ViewData["Categories"] = await _categoryService.GetProductsCategoryForHome();
        ViewData["NewProducts"] = await _productService.GetNewProducts();
        ViewData["BestSellerProducts"] = await _productService.BestSellerProducts();
        return View();
    }
}
