using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.ProductEntities
{
    public class ProductCategory : BaseEntity
    {
        public long? ParentId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public ProductCategory Parent { get; set; }
        public ICollection<ProductSelectedCategory> ProductSelectedCategories { get; set; }
    }
}
