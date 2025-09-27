using ZarmallStore.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Accessibility
{
    public class AccessChecker : AuthorizeAttribute, IAuthorizationFilter
    {
        #region constractor
        private IPermissionService _permissionService;
        private long _permissionId = 0;

        public AccessChecker(long permissionId)
        {
            _permissionId = permissionId;
        }
        #endregion
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _permissionService = (IPermissionService)context.HttpContext.RequestServices.GetService(typeof(IPermissionService));

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var phoneNumber = context.HttpContext.User.Identity.Name;

                if (!_permissionService.CheckPermission(_permissionId, phoneNumber))
                {
                    context.Result = new RedirectResult("/access-denied");
                }
            }
            else
            {
                context.Result = new RedirectResult("/access-denied");
            }
        }
    }
}
