using ZarmallStore.Data.Entities.Common;
using ZarmallStore.Data.Entities.ProductEntities;

namespace ZarmallStore.Data.Entities.Account
{
    public class FavoriteProduct : BaseEntity
    {
        public long UserId { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
    }
}
