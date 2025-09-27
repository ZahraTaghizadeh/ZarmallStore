using ZarmallStore.Data.DTOs.Account;
using ZarmallStore.Data.Entities.Account;

namespace ZarmallStore.Application.Services.Interfaces
{
    public interface IPermissionService : IAsyncDisposable
    {
        #region Admin Management
        Task<List<long>> GetAdminPermissions(long userId);
        bool CheckPermission(long permissionId, string mobile);
        Task<CreateAdminDto> GetAdmin(long userId);
        Task RemoveAllUserSelectedRole(long userId);
        Task AddUserToRole(CreateAdminDto dto);
        Task<List<UserRole>> GetAllAdmins();
        #endregion

        #region Role
        Task<List<Role>> GetAllActiveRoles();
        Task<List<Permission>> GetAllPermissions();
        Task RemoveAllRolePermissions(long roleId);
        Task AddRolePermissions(List<long> permissions, long roleId);
        Task CreateRole(CreateRoleDto dto);
        Task<EditRoleDto> GetEditRole(long roleId);
        Task EditRole(EditRoleDto dto);
        Task<bool> DeleteRole(long roleId);
        #endregion
    }
}
