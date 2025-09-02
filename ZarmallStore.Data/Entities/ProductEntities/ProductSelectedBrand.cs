using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.ProductEntities
{
    public class ProductSelectedBrand : BaseEntity
    {
        public long ProductId { get; set; }
        public long BrandId { get; set; }
        public Product Product { get; set; }
        public Brand Brand { get; set; }
    }
}
