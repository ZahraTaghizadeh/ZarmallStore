using ZarmallStore.Data.DTOs.CommonDto;
using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Application.Services.Interfaces
{
    public interface ICommonService : IAsyncDisposable
    {
        #region Banner
        Task<List<Banner>> GetAllBanners();
        Task<List<BannerItemDto>> GetHomeBanners();
        Task<bool> CreateBanner(CreateBannerDto dto);
        Task<EditBannerDto> GetEditBanner(long bannerId);
        Task<bool> EdiBannerDto(EditBannerDto dto);
        Task DeleteBanner(long bannerId);
        #endregion

        #region SiteInfo

        Task<SiteInfo> GetSiteInfo();

        #endregion
    }
}
