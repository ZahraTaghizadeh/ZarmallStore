using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.ProductEntities
{
    public class ProductSelectedCategory : BaseEntity
    {
        public long ProductId { get; set; }
        public long CategoryId { get; set; }
        public Product Product { get; set; }
        public ProductCategory ProductCategory { get; set; }
    }
}
