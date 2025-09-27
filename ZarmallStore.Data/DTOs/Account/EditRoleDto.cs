namespace ZarmallStore.Data.DTOs.Account
{
    public class EditRoleDto
    {
        public long RoleId { get; set; }
        public string Title { get; set; }
        public List<long> Permissions { get; set; }
    }
}
