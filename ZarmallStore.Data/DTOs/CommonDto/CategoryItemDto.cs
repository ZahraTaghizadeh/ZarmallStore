namespace ZarmallStore.Data.DTOs.CommonDto
{
    public class CategoryItemDto
    {
        public long CategoryId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public string MainImage { get; set; }
    }
}
