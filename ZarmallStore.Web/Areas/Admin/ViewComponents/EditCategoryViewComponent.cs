using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.ProductCategoryDto;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Areas.Admin.ViewComponents
{
    public class EditCategoryViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public EditCategoryViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IViewComponentResult> InvokeAsync(long? parentId, int level , long thisCategoryId)
        {
            var categories = await _categoryService.GetAllCategoriesForEdit(parentId , thisCategoryId);

            var model = new TreeViewCategoriesDto
            {
                Level = level,
                productCategories = categories,
                ThisCategoryId = thisCategoryId
            };

            return View("EditCategory", model);
        }
    }
}
