using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Web.UserExtensions;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Areas.Admin.ViewComponents
{
    public class AdminHeaderViewComponent : ViewComponent
    {
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;

        public AdminHeaderViewComponent(IPermissionService permissionService, IUserService userService)
        {
            _permissionService = permissionService;
            _userService = userService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userService.GetUserById(User.GetUserId());
            var permissions = await _permissionService.GetAdminPermissions(User.GetUserId());
            ViewData["Permissions"] = permissions;
            return View("AdminHeader" , user);
        }
    }
}
