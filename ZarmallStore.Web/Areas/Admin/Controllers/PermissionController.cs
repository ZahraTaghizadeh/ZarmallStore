using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.Account;
using ZarmallStore.Web.Accessibility;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Areas.Admin.Controllers
{
    [AccessChecker(4)]
    public class PermissionController : AdminBaseController
    {
        #region CTOR
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        #endregion

        #region Admins
        [HttpGet("admins")]
        public async Task<IActionResult> AdminList()
        {
            var model = await _permissionService.GetAllAdmins();
            return View(model);
        }
        #endregion

        #region Roles
        [HttpGet("roles")]
        public async Task<IActionResult> Roles()
        {
            var model = await _permissionService.GetAllActiveRoles();
            return View(model);
        }
        #endregion

        #region Create Role
        [HttpGet("create-role")]
        public async Task<IActionResult> CreateRole()
        {
            ViewData["Permissions"] = await _permissionService.GetAllPermissions();
            return View();
        }

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole(CreateRoleDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _permissionService.CreateRole(dto);
            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("Roles");
        }
        #endregion

        #region Edit Role
        [HttpGet("edit-role")]
        public async Task<IActionResult> EditRole(long roleId)
        {
            if (roleId == 1)
            {
                TempData[ErrorMessage] = "نقش مدیر قابل ویرایش نمی باشد.";
                return RedirectToAction("Roles");
            }
            ViewData["Permissions"] = await _permissionService.GetAllPermissions();
            var model = await _permissionService.GetEditRole(roleId);
            return View(model);
        }

        [HttpPost("edit-role")]
        public async Task<IActionResult> EditRole(EditRoleDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _permissionService.EditRole(dto);
            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("Roles");
        }
        #endregion

        #region Delete Role
        [HttpGet("delete-role")]
        public async Task<IActionResult> DeleteRole(long roleId)
        {
            if (roleId == 1)
            {
                TempData[ErrorMessage] = "نقش مدیر قابل حذف نمی باشد.";
                return RedirectToAction("Roles");
            }
            await _permissionService.DeleteRole(roleId);
            TempData[SuccessMessage] = DeleteText;
            return RedirectToAction("Roles");
        }
        #endregion

        #region Add Role To User
        [HttpGet("add-role-to-user")]
        public async Task<IActionResult> AddRoleToUser(long userId)
        {
            var roles = await _permissionService.GetAllActiveRoles();
            if (roles.All(r => r.Id == 1))
            {
                TempData[InfoMessage] = "برای اعطای سطح دسترسی به کاربر باید نقش هایی را تعریف کنید.";
                return RedirectToAction("CreateRole");
            }
            ViewData["Roles"] = roles;
            var model = await _permissionService.GetAdmin(userId);
            return View(model);
        }

        [HttpPost("add-role-to-user")]
        public async Task<IActionResult> AddRoleToUser(CreateAdminDto dto)
        {
            ViewData["Roles"] = await _permissionService.GetAllActiveRoles();
            if (ModelState.IsValid)
            {
                await _permissionService.AddUserToRole(dto);
                TempData[SuccessMessage] = SuccessText;
                return RedirectToAction("AdminList");
            }

            TempData[SuccessMessage] = SuccessText;
            return View(dto);
        }
        #endregion
    }
}
