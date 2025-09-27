using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.OrderEntities
{
    public class ProductSell : BaseEntity
    {
        public long ProductId { get; set; }
        public int SellCount { get; set; }
        public long OrderId { get; set; }
        public int ProductPrice { get; set; }
        public DateTime SellDate { get; set; }
    }
}
