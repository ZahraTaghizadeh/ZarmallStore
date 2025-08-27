using ZarmallStore.Data.DTOS.Account;
using ZarmallStore.Data.Entities.Account;

namespace ZarmallStore.Application.Services.Interface
{
    public interface IUserService : IAsyncDisposable
    {
        #region Register & Login
        Task RegisterOrLoginUser(RegisterUserDTO dto);
        Task<bool> CheckUserExistByMobile(string mobile);
        Task<bool> CheckMobileAuthorization(MobileActivationDTO dto);
        Task<User?> GetUserByMobile(string mobile);
        Task<EditUserInfoDTO> GetEditUserDetail(long userId);
        Task EditUserDetail(EditUserInfoDTO dto);
        Task<UserDetailsDTO> GetUserDetails(long userId);
        Task<bool> SendActivationSms(string mobile);

        #endregion
    }
}
