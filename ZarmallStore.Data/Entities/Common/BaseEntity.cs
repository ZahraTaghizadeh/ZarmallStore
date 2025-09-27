using System.ComponentModel.DataAnnotations;

namespace ZarmallStore.Data.Entities.Common
{
    public class BaseEntity
    {
        [Key]
        public long Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
