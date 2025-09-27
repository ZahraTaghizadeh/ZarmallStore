using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
