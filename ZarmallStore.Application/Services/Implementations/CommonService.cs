using ZarmallStore.Application.Extensions;
using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Application.Utils;
using ZarmallStore.Data.DTOs.CommonDto;
using ZarmallStore.Data.Entities.Common;
using ZarmallStore.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ZarmallStore.Application.Services.Implementations
{
    public class CommonService : ICommonService
    {
        #region CTOR
        private readonly IGenericRepository<Banner> _bannerRepository;
        private readonly IGenericRepository<SiteInfo> _infoRepository;
        private readonly IMemoryCache _cache;

        public CommonService(IGenericRepository<Banner> bannerRepository, IGenericRepository<SiteInfo> infoRepository, IMemoryCache cache)
        {
            _bannerRepository = bannerRepository;
            _infoRepository = infoRepository;
            _cache = cache;
        }
        public async ValueTask DisposeAsync()
        {
            await _bannerRepository.DisposeAsync();
            await _infoRepository.DisposeAsync();
        }
        #endregion

        #region Banner
        public async Task<List<Banner>> GetAllBanners()
        {
            return await _bannerRepository.GetQuery().ToListAsync();
        }

        public async Task<List<BannerItemDto>> GetHomeBanners()
        {
            var banners = await _cache.GetOrCreateAsync("AllBanners", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                var bannerEntities = await _bannerRepository.GetQuery()
                    .OrderBy(b => b.Order)
                    .Select(b => new BannerItemDto
                    {
                        Title = b.Title,
                        Url = b.Url,
                        ImageName = b.ImageName,
                        Order = b.Order,
                        BannerLocation = b.BannerLocation
                    })
                    .ToListAsync();

                return bannerEntities;
            });

            return banners ?? new List<BannerItemDto>();
        }

        public async Task<bool> CreateBanner(CreateBannerDto dto)
        {
            #region Create Banner
            var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(dto.ImageFile.FileName);
            var res = dto.ImageFile.AddImageToServer(imageName, PathExtension.BannerServer, 300, 150, PathExtension.BannerThumbServer);
            if (!res) return false;
            var newBanner = new Banner
            {
                BannerLocation = dto.BannerLocation,
                Order = dto.Order,
                Url = dto.Url,
                Title = dto.Title,
                ImageName = imageName
            };

            await _bannerRepository.AddEntity(newBanner);
            await _bannerRepository.SaveAsync();
            #endregion

            #region Cache new banner
            var cachedBanners = _cache.Get<List<BannerItemDto>>("AllBanners");
            if (cachedBanners == null) return true;

            var newBannerDto = new BannerItemDto
            {
                Title = newBanner.Title,
                Url = newBanner.Url,
                ImageName = newBanner.ImageName,
                Order = newBanner.Order,
                BannerLocation = newBanner.BannerLocation
            };

            cachedBanners.Add(newBannerDto);
            cachedBanners = cachedBanners.OrderBy(b => b.Order).ToList();
            _cache.Set("AllBanners", cachedBanners, TimeSpan.FromDays(1));
            #endregion

            return true;
        }

        public async Task<EditBannerDto> GetEditBanner(long bannerId)
        {
            var data = await _bannerRepository.GetEntityById(bannerId);
            return new EditBannerDto
            {
                Title = data.Title,
                Order = data.Order,
                Url = data.Url,
                BannerId = data.Id,
                BannerLocation = data.BannerLocation
            };
        }

        public async Task<bool> EdiBannerDto(EditBannerDto dto)
        {
            var banner = await _bannerRepository.GetQuery().FirstOrDefaultAsync(b => b.Id == dto.BannerId);
            if (banner == null) return false;

            banner.Title = dto.Title;
            banner.BannerLocation = dto.BannerLocation;
            banner.Order = dto.Order;
            banner.Url = dto.Url;

            if (dto.ImageFile != null)
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(dto.ImageFile.FileName);
                dto.ImageFile.AddImageToServer(imageName, PathExtension.BannerServer, 300, 150, PathExtension.BannerThumbServer, banner.ImageName);
                banner.ImageName = imageName;
            }

            _bannerRepository.EditEntity(banner);
            await _bannerRepository.SaveAsync();

            _cache.Remove("AllBanners");

            return true;
        }

        public async Task DeleteBanner(long bannerId)
        {
            var banner = await _bannerRepository.GetEntityById(bannerId);
            banner.ImageName.DeleteImage(PathExtension.BannerImage, PathExtension.BannerThumb);
            await _bannerRepository.DeletePermanent(banner);
            await _bannerRepository.SaveAsync();

            _cache.Remove("AllBanners");
        }
        #endregion

        #region Site Info
        public async Task<SiteInfo> GetSiteInfo()
        {
            return await _infoRepository.GetQuery().FirstAsync();
        }
        #endregion
    }
}
