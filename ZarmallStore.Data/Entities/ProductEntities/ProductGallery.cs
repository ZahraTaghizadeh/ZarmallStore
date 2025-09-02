using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.ProductEntities
{
    public class ProductGallery : BaseEntity
    {
        public long ProductId { get; set; }
        public string ImageName { get; set; }
        public int Order { get; set; }
    }
}
