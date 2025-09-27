using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.ProductCategoryDto;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Areas.Admin.ViewComponents
{
    public class SelectCategoriesViewComponent : ViewComponent
    {
        private readonly ICategoryService  _categoryService;

        public SelectCategoriesViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IViewComponentResult> InvokeAsync(long? parentId, int level , List<long>? selectedCategoriesIds)
        {
            var categories = await _categoryService.GetAllCategories(parentId);

            var model = new TreeViewCategoriesDto
            {
                Level = level,
                productCategories = categories,
                SelectedCategoriesIds = selectedCategoriesIds
            };

            return View("SelectCategories", model);
        }
    }
}
