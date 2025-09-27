namespace ZarmallStore.Data.Entities.Common
{
    public class SocialLink : BaseEntity
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public string IconName { get; set; }
        public string BackgroundColor { get; set; }
    }
}
