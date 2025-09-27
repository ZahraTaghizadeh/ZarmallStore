using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.DTOs.CommonDto
{
    public class BannerItemDto
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string ImageName { get; set; }
        public int Order { get; set; }
        public BannerLocation BannerLocation { get; set; }
    }
}
