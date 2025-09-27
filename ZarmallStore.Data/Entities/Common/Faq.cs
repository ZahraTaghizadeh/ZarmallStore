namespace ZarmallStore.Data.Entities.Common
{
    public class Faq : BaseEntity
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Order { get; set; }
    }
}
