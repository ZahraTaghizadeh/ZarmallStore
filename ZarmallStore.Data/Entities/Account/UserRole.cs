using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.Account
{
    public class UserRole : BaseEntity
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
