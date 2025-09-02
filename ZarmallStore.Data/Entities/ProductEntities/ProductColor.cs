using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.ProductEntities
{
    public class ProductColor : BaseEntity
    {
        public string Title { get; set; }
        public string ColorCode { get; set; }
        public ICollection<ProductVariant> ProductVariants4 { get; set; }
    }
}
