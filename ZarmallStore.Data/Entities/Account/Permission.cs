using ZarmallStore.Data.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace ZarmallStore.Data.Entities.Account
{
    public class Permission : BaseEntity
    {
        [Display(Name = "عنوان دسترسی")]
        public string Title { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
