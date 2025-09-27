using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.Account;
using ZarmallStore.Data.Entities.Account;
using ZarmallStore.Data.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ZarmallStore.Application.Services.Implementations
{
    public class PermissionService : IPermissionService
    {
        #region CTOR
        private readonly IGenericRepository<RolePermission> _rolePermissionRepository;
        private readonly IGenericRepository<Permission> _permissionRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<UserRole> _userRoleRepository;
        private readonly IGenericRepository<User> _userRepository;

        public PermissionService(IGenericRepository<RolePermission> rolePermissionRepository, IGenericRepository<Permission> permissionRepository, IGenericRepository<Role> roleRepository, IGenericRepository<UserRole> userRoleRepository, IGenericRepository<User> userRepository)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _permissionRepository = permissionRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
        }

        public async ValueTask DisposeAsync()
        {
            await _userRepository.DisposeAsync();
            await _rolePermissionRepository.DisposeAsync();
            await _roleRepository.DisposeAsync();
            await _userRoleRepository.DisposeAsync();
            await _permissionRepository.DisposeAsync();
        }
        #endregion

        #region Admin Management
        public async Task<List<Permission>> GetAllPermissions()
        {
            return await _permissionRepository.GetQuery().ToListAsync();
        }

        public bool CheckPermission(long permissionId, string mobile)
        {
            var userId = _userRepository.GetQuery().Single(c => c.MobileNumber == mobile).Id;

            var userRole = _userRoleRepository.GetQuery().AsQueryable()
                .Where(c => c.UserId == userId).Select(r => r.RoleId).ToList();

            if (!userRole.Any())
                return false;

            var permissions = _rolePermissionRepository.GetQuery().AsQueryable()
                .Where(c => c.PermissionId == permissionId).Select(c => c.RoleId).ToList();


            return permissions.Any(c => userRole.Contains(c));
        }

        public async Task<CreateAdminDto> GetAdmin(long userId)
        {
            return new CreateAdminDto
            {
                UserId = userId,
                Roles = await _userRoleRepository.GetQuery().Where(r => r.UserId == userId)
                    .Select(r => r.RoleId).ToListAsync()
            };
        }

        public async Task RemoveAllUserSelectedRole(long userId)
        {
            var allUserRoles = await _userRoleRepository.GetQuery().
                Where(c => c.UserId == userId).ToListAsync();

            if (allUserRoles.Any())
            {
                _userRoleRepository.DeletePermanentEntities(allUserRoles);
                await _userRoleRepository.SaveAsync();
            }
        }

        public async Task AddUserToRole(CreateAdminDto dto)
        {
            await RemoveAllUserSelectedRole(dto.UserId);

            if (dto.Roles.Any())
            {
                var userRoles = dto.Roles.Select(roleId => new UserRole { RoleId = roleId, UserId = dto.UserId }).ToList();
                await _userRoleRepository.AddRangeEntities(userRoles);
                await _userRoleRepository.SaveAsync();
            }
        }

        public async Task<List<UserRole>> GetAllAdmins()
        {
            return await _userRoleRepository.GetQuery().Include(ur => ur.User)
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ToListAsync();
        }
        #endregion

        #region Roles
        public async Task<List<Role>> GetAllActiveRoles()
        {
            return await _roleRepository.GetQuery().Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ToListAsync();
        }
      
        public async Task<List<long>> GetAdminPermissions(long userId)
        {
            var permissions = await _rolePermissionRepository.GetQuery()
                .Where(rp => _userRoleRepository.GetQuery()
                .Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).Contains(rp.RoleId))
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            return permissions;
        }

        public async Task RemoveAllRolePermissions(long roleId)
        {
            var data = await _rolePermissionRepository.GetQuery().Where(p => p.RoleId == roleId)
                .ToListAsync();
            _rolePermissionRepository.DeletePermanentEntities(data);
            await _rolePermissionRepository.SaveAsync();
        }

        public async Task AddRolePermissions(List<long> permissions, long roleId)
        {
            var roles = permissions.Select(item => new RolePermission {RoleId = roleId, PermissionId = item}).ToList();

            var basePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = 1
            };

            roles.Add(basePermission);

            await _rolePermissionRepository.AddRangeEntities(roles);
            await _rolePermissionRepository.SaveAsync();
        }

        public async Task CreateRole(CreateRoleDto dto)
        {
            var role = new Role
            {
                RoleTitle = dto.Title,
                RolePermissions = new List<RolePermission>()
            };
            await _roleRepository.AddEntity(role);
            await _roleRepository.SaveAsync();
            await AddRolePermissions(dto.Permissions, role.Id);
        }

        public async Task<EditRoleDto> GetEditRole(long roleId)
        {
            var role = await _roleRepository.GetEntityById(roleId);
            return new EditRoleDto
            {
                Title = role.RoleTitle,
                RoleId = roleId,
                Permissions = await _rolePermissionRepository.GetQuery().Where(p => p.RoleId == roleId)
                    .Select(r => r.PermissionId).ToListAsync()
            };
        }

        public async Task EditRole(EditRoleDto dto)
        {
            await RemoveAllRolePermissions(dto.RoleId);
            await AddRolePermissions(dto.Permissions, dto.RoleId);

            var role = await _roleRepository.GetEntityById(dto.RoleId);
            role.RoleTitle = dto.Title;
            _roleRepository.EditEntity(role);
            await _roleRepository.SaveAsync();
        }

        public async Task<bool> DeleteRole(long roleId)
        {
            var isRoleInUse = await _userRoleRepository.GetQuery().AnyAsync(r => r.RoleId == roleId);
            if (isRoleInUse) return false;

            var rolePermissions = await _rolePermissionRepository.GetQuery().Where(p => p.RoleId == roleId)
                .ToListAsync();
            if (rolePermissions.Any())
                _rolePermissionRepository.DeletePermanentEntities(rolePermissions);
            await _rolePermissionRepository.SaveAsync();

            var role = await _roleRepository.GetEntityById(roleId);
            _roleRepository.DeleteEntity(role);
            await _roleRepository.SaveAsync();
            return true;
        }
        #endregion
    }
}
