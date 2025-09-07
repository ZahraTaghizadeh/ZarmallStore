using ZarmallStore.Data.Entities.Common;
using ZarmallStore.Data.Entities.ProductEntities;

namespace ZarmallStore.Data.Entities.OrderEntities
{
    public class OrderDetail : BaseEntity
    {
        public long ProductId { get; set; }
        public Product Product { get; set; }
    }
}
