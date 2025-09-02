using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.ProductEntities
{
    public class Brand : BaseEntity
    {
        public string Title { get; set; }
        public string ImageName { get; set; }
        public int Order { get; set; }
        public ICollection<ProductSelectedBrand> ProductSelectedBrands { get; set; }
    }
}
