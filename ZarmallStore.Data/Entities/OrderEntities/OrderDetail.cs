using ZarmallStore.Data.Entities.Common;
using ZarmallStore.Data.Entities.ProductEntities;

namespace ZarmallStore.Data.Entities.OrderEntities
{
    public class OrderDetail : BaseEntity
    {
        public long OrderId { get; set; }
        public long ProductVariantId { get; set; }
        public int TotalPrice { get; set; }
        public int ProductPrice { get; set; }
        public int VariantPrice { get; set; }
        public int Count { get; set; }
        public Order Order { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
