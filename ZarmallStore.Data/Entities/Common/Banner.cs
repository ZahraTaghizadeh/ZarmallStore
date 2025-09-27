using System.ComponentModel.DataAnnotations;

namespace ZarmallStore.Data.Entities.Common
{
    public class Banner : BaseEntity
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string ImageName { get; set; }
        public int Order { get; set; }
        public BannerLocation BannerLocation { get; set; }
    }
    public enum BannerLocation
    {
        [Display(Name = "اسلایدر")]
        Slider,
        [Display(Name = "سطر اول")]
        FirstRow,
        [Display(Name = "سطر دوم")]
        SecondRow
    }
}
