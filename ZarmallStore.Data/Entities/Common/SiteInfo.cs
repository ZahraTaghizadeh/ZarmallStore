namespace ZarmallStore.Data.Entities.Common
{
    public class SiteInfo : BaseEntity
    {
        public bool CategorySliderActivation { get; set; }
        public bool NewProductSliderActivation { get; set; }
        public bool BestSellerSliderActivation { get; set; }
        public bool BlogSliderActivation { get; set; }

        public string ShopTitle { get; set; }
        public string PhoneNumber { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public string? Email { get; set; }
        public string WorkTime { get; set; }
        public string BusinessId { get; set; }
        public string NationalBusinessNumber { get; set; }
        public string FooterTitle { get; set; }
        public string FooterDescription { get; set; }
        public string CopyRightText { get; set; }
    }
}
