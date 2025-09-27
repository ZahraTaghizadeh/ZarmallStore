using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.CommonDto;
using ZarmallStore.Web.Accessibility;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Areas.Admin.Controllers
{
    [AccessChecker(6)]
    public class CommonController : AdminBaseController
    {
        #region CTOR
        private readonly ICommonService _commonService;
        public CommonController(ICommonService commonService)
        {
            _commonService = commonService;
        }
        #endregion

        #region Banner
        [HttpGet("banners-list")]
        public async Task<IActionResult> BannersList()
        {
            var model = await _commonService.GetAllBanners();
            return View(model);
        }

        [HttpGet("create-banner")]
        public IActionResult CreateBanner()
        {
            return View();
        }

        [HttpPost("create-banner")]
        public async Task<IActionResult> CreateBanner(CreateBannerDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _commonService.CreateBanner(dto);
            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("BannersList");
        }

        [HttpGet("edit-banner")]
        public async Task<IActionResult> EditBanner(long bannerId)
        {
            var model = await _commonService.GetEditBanner(bannerId);
            return View(model);
        }

        [HttpPost("edit-banner")]
        public async Task<IActionResult> EditBanner(EditBannerDto dto)
        {
            if (ModelState.IsValid) return RedirectToAction("BannersList");
            await _commonService.EdiBannerDto(dto);
            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("BannersList");
        }

        [Route("delete-banner")]
        public async Task<IActionResult> DeleteBanner(long bannerId)
        {
            await _commonService.DeleteBanner(bannerId);
            TempData[SuccessMessage] = DeleteText;
            return RedirectToAction("BannersList");
        }
        #endregion
    }
}
