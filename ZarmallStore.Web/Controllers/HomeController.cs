using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Controllers;

public class HomeController : SiteBaseController
{
    public async Task<IActionResult> Index()
    {
        return View();
    }
}
