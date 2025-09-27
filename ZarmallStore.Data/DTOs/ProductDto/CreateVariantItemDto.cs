namespace ZarmallStore.Data.DTOs.ProductDto
{
    public class CreateVariantItemDto
    {
        public long ColorId { get; set; }
        public string? Price { get; set; }
        public int StockCount { get; set; }
    }
}
