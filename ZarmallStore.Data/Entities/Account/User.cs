using System.ComponentModel.DataAnnotations;
using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.Account
{
    public class User : BaseEntity
    {
        public string MobileNumber { get; set; }
        public string MobileActivationNumber { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }
    }
}
