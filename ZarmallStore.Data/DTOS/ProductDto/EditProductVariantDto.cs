namespace ZarmallStore.Data.DTOs.ProductDto
{
    public class EditProductVariantDto
    {
        public long VariantId { get; set; }
        public long ColorId { get; set; }
        public string Price { get; set; }
        public int StockCount { get; set; }
    }
}
