using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.ProductEntities
{
    public class ProductVariant : BaseEntity
    {
        public long ProductId { get; set; }
        public long ColorId { get; set; }
        public int Price { get; set; }
        public int StockCount { get; set; }
        public Product Product { get; set; }
        public ProductColor ProductColor { get; set; }
    }
}
