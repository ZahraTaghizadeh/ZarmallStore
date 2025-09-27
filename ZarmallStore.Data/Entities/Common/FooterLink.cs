using System.ComponentModel.DataAnnotations;

namespace ZarmallStore.Data.Entities.Common
{
    public class FooterLink : BaseEntity
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public FooterLinkLocation LinkLocation { get; set; }
    }

    public enum FooterLinkLocation
    {
        [Display(Name = "راهنمای خرید")]
        PurchaseGuid,
        [Display(Name = "با ما")]
        WithUs,
        [Display(Name = "باشگاه مشتریان")]
        CustomersCommunity
    }
}
