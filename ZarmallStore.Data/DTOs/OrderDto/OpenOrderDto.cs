using ZarmallStore.Data.Entities.Account;
using ZarmallStore.Data.Entities.OrderEntities;

namespace ZarmallStore.Data.DTOs.OrderDto
{
    public class OpenOrderDto
    {
        public User User { get; set; }
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public int TotalCartPrice()
        {
            return OrderDetails.Select(item => (item.ProductVariant.Product.BasePrice + item.ProductVariant.Price) * item.Count).Aggregate(0, (current, price) => current + price);
        }
    }
}
