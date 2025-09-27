using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.Account;
using ZarmallStore.Web.UserExtensions;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ZarmallStore.Web.Areas.User.Controllers
{
    public class AccountController : UserBaseController
    {
        #region CTOR
        private readonly IUserService _userService;
        private readonly ICaptchaValidator _captchaValidator;

        public AccountController(IUserService userService, ICaptchaValidator captchaValidator)
        {
            _userService = userService;
            _captchaValidator = captchaValidator;
        }

        #endregion

        #region Edit Detail
        [HttpGet("edit-user-detail")]
        public async Task<IActionResult> EditUserDetail(bool returnToCheckout)
        {
            ViewData["User"] = await _userService.GetUserById(User.GetUserId());
            var model = await _userService.GetEditUserDetail(User.GetUserId());
            model.ReturnToCheckout = returnToCheckout;
            return View(model);
        }

        [HttpPost("edit-user-detail"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserDetail(EditUserInfoDTO dto)
        {
            ViewData["User"] = await _userService.GetUserById(User.GetUserId());

            #region Captcha Validation
            if (!await _captchaValidator.IsCaptchaPassedAsync(dto.Token))
            {
                TempData[ErrorMessage] = "اعتبارسنجی کپچا موفقیت آمیز نبود.لطفا VPN خود را خاموش کنید.";
                return View(dto);
            }
            #endregion

            if (!ModelState.IsValid) return View(dto);
            await _userService.EditUserDetail(dto);
            TempData[SuccessMessage] = SuccessText;

            if (dto.ReturnToCheckout) return RedirectToAction("Checkout", "Order", new { area = "User" });
            return RedirectToAction("EditUserDetail", "Account", new { area = "User" });
        }
        #endregion

        #region Favorite Product
        [HttpGet("favorite-products")]
        public async Task<IActionResult> FavoriteProducts(FilterFavoriteProductDto filter)
        {
            filter.TakeEntity = 14;
            filter.UserId = User.GetUserId();
            var model = await _userService.FilterFavoriteProducts(filter);
            return View(model);
        }

        [HttpGet("is-favorite")]
        public async Task<IActionResult> IsFavorite(long productId)
        {
            var userId = User.GetUserId();
            var isFavorite = false;
            if (userId == 0) return Json(new { isFavorite });

            isFavorite = await _userService.IsProductFavorite(productId, userId);
            return Json(new { isFavorite });
        }

        [Authorize]
        [HttpPost("toggle-favorite-product")]
        public async Task<IActionResult> ToggleFavoriteProduct(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return RedirectToAction("RegisterOrLogin" , new {area=""});

            var isNowFavorite = await _userService.ToggleFavoriteProduct(long.Parse(userId), productId);

            return Json(new { isFavorite = isNowFavorite });
        }
        #endregion
    }
}
