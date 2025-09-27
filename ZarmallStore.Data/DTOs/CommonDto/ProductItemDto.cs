namespace ZarmallStore.Data.DTOs.CommonDto
{
    public class ProductItemDto
    {
        public long ProductId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int Price { get; set; }
        public int StockCount { get; set; }
        public int TotalSold { get; set; }
    }
}
