using ZarmallStore.Data.Entities.Account;
using ZarmallStore.Data.Entities.OrderEntities;

namespace ZarmallStore.Data.DTOs.Account
{
    public class UserDashboardDetailDto
    {
        public User User { get; set; }
        public List<Order> PendingOrders { get; set; }
        public int SentOrderCount { get; set; }
        public int PendingOrderCount { get; set; }
        public int ReturnedOrderCount { get; set; }
        public int CanceledOrderCount { get; set; }
    }
}
