using System.ComponentModel.DataAnnotations;
using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.Account
{
    public class Role : BaseEntity
    {
        [Display(Name = "عنوان نقش")]
        public string RoleTitle { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
