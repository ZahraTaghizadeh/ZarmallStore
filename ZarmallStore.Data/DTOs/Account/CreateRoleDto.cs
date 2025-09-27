namespace ZarmallStore.Data.DTOs.Account
{
    public class CreateRoleDto
    {
        public string Title { get; set; }
        public List<long> Permissions { get; set; }
    }
}
