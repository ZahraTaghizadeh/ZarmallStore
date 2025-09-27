using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.ProductCategoryDto;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Areas.Admin.ViewComponents
{
    public class CreateCategoryViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public CreateCategoryViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IViewComponentResult> InvokeAsync(long? parentId , int level)
        {
            var categories = await _categoryService.GetAllCategories(parentId);

            var model = new TreeViewCategoriesDto
            {
                Level = level,
                productCategories = categories
            };

            return View("CreateCategory"  , model);
        }
    }
}
