using System.ComponentModel.DataAnnotations;
using ZarmallStore.Data.Entities.Account;
using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.ProductEntities
{
    public class ProductComment : BaseEntity
    {
        public long ProductId { get; set; }
        public long UserId { get; set; }
        [Range(1, 5)]
        public int Rate { get; set; }
        public CommentState CommentState { get; set; }
        public string Description { get; set; }
        public Product Product { get; set; }
        public User USer { get; set; }
    }
    public enum CommentState
    {
        Submitted,
        Rejected,
        Accepted
    }
}
