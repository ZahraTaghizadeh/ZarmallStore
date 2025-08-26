using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.ViewComponents
{
    public class HomeCartSectionViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("HomeCartSection");
        }
    }
}
