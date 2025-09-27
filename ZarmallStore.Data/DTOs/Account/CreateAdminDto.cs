namespace ZarmallStore.Data.DTOs.Account
{
    public class CreateAdminDto
    {
        public long UserId { get; set; }
        public List<long> Roles { get; set; }
    }
}
