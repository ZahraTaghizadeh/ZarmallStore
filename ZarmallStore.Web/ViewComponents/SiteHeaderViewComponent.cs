using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.ViewComponents
{
    public class SiteHeaderViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("SiteHeader");
        }
    }
}
