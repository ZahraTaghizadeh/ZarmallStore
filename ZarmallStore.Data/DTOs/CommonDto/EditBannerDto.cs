using ZarmallStore.Data.Entities.Common;
using Microsoft.AspNetCore.Http;

namespace ZarmallStore.Data.DTOs.CommonDto
{
    public class EditBannerDto
    {
        public long BannerId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public IFormFile? ImageFile { get; set; }
        public int Order { get; set; }
        public BannerLocation BannerLocation { get; set; }
    }
}
