using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZarmallStore.Application.Services.Interface;
using ZarmallStore.Data.DTOS.Account;

namespace ZarmallStore.Web.Controllers
{
    public class AccountController : SiteBaseController
    {
        #region CTOR
        private readonly IUserService _userService;
        private readonly  ICaptchaValidator _captchaValidator;
        public AccountController(IUserService userService, ICaptchaValidator captchaValidator)
        {
            _userService = userService;
            _captchaValidator = captchaValidator;
        }
        #endregion

        #region Register Or Login
        [HttpGet("register")]
        public async Task<IActionResult> RegisterOrLogin(string? returnUrl =null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost("register"),ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterOrLogin(RegisterUserDTO dto)
        {
             await _userService.RegisterOrLoginUser(dto);
            return RedirectToAction("MobileAuthorization" , new { returnUrl = dto.ReturnUrl ,mobile = dto.MobileNumber});
        }
        #endregion

        #region MobileAuthorization
        [HttpGet("authorization")]
        public async Task<IActionResult> MobileAuthorization(string mobile , string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Mobile"] = mobile;
            return View();
        }
        [HttpPost("authorization"), ValidateAntiForgeryToken]
        public async Task<IActionResult> MobileAuthorization(MobileActivationDTO dto)
        {
            #region Captcha Validation
            if(!await _captchaValidator.IsCaptchaPassedAsync(dto.Token))
            {
                TempData[ErrorMessage] = "اعتبارسنجی کپچا موفقیت آمیز نبود. لطفا vpn خود را خاموش کنید.";
                return View();
            }
            #endregion

            if(ModelState.IsValid)
            {
                var res = await _userService.CheckMobileAuthorization(dto);
                if(!res)
                {
                    TempData[ErrorMessage] = "کد اعتبارسنجی صحیح نمی باشد.";
                    return View();
                }
                var user = await _userService.GetUserByMobile(dto.Mobile);
                if (user == null) return NotFound();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.MobileNumber),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var properties = new AuthenticationProperties
                {
                    IsPersistent = true,
                };
                await HttpContext.SignInAsync(principal, properties);
                TempData[SuccessMessage] = "خوش آمدی!";

                if(!string.IsNullOrEmpty(dto.ReturnUrl) && Url.IsLocalUrl(dto.ReturnUrl))
                {
                    return Redirect(dto.ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData[ErrorMessage] = "لطفا خطاهای زیر را رفع کنید.";
            return View();
        }
        #endregion

        #region Resend Verification Code
        [HttpGet("resend-verification-code")]
        public async  Task<IActionResult> ResendVerificationCode(string mobileNumber)
        {
            var res = await _userService.SendActivationSms(mobileNumber);
            if (res) return RedirectToAction("MobileAuthorization");
            TempData[ErrorMessage] = "کاربری یافت نشد.";
            return RedirectToAction("MobileAuthorization");

        }
        #endregion


        #region Log Out

        [Route("log-out")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        #endregion

    }
}
