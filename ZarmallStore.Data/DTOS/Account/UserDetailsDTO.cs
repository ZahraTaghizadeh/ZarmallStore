namespace ZarmallStore.Data.DTOS.Account
{
    public class UserDetailsDTO
    {
        public long Id { get; set; }
        public DateTime CreatDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public string MobileNumber { get; set; }
        public string MobileActivationNumber { get; set; }
        public string? FullName { get; set; }

        public string Password { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }
    }
}
