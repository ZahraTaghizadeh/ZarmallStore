using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.Account;
using ZarmallStore.Web.Accessibility;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Areas.Admin.Controllers
{
    [AccessChecker(2)]
    public class UserController : AdminBaseController
    {
        #region CTOR
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        #endregion

        #region User Management
        public async Task<IActionResult> FilterUser(FilterUserDto filter)
        {
            var data = await _userService.FilterUser(filter);
            return View(data);
        }

        public async Task<IActionResult> UserDetail(long userId)
        {
            var data = await _userService.UserDetail(userId);
            return View(data);
        }
        #endregion
    }
}
