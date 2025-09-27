using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Web.UserExtensions;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Areas.User.ViewComponents
{
    public class UserMenuViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public UserMenuViewComponent(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["User"] = await _userService.GetUserById(User.GetUserId());
            return View("UserMenu");
        }
    }
}
