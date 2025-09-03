namespace ZarmallStore.Data.DTOS.ProductDto
{
    public class EditProductVariantDto
    {
        public long VariantId { get; set; }
        public long ColorId { get; set; }
        public int Price { get; set; }
        public int StockCount { get; set; }
    }
}
