using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.ProductEntities
{
    public class ProductFeature: BaseEntity
    {
        public long ProductId { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public Product Product { get; set; }
    }
}
