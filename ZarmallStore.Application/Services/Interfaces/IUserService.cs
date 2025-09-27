using ZarmallStore.Data.DTOs.Account;
using ZarmallStore.Data.Entities.Account;

namespace ZarmallStore.Application.Services.Interfaces
{
    public interface IUserService : IAsyncDisposable
    {
        #region Register & Login
        Task RegisterOrLoginUser(RegisterUserDTO dto);
        Task<UserDashboardDetailDto> UserDashboardDetail(long userId);
        Task<bool> CheckUserExistByMobile(string mobile);
        Task<User> GetUserById(long userId);
        Task<bool> CheckMobileAuthorization(MobileActivationDTO dto);
        Task<User?> GetUserByMobile(string mobile);
        Task<EditUserInfoDTO> GetEditUserDetail(long userId);
        Task EditUserDetail(EditUserInfoDTO dto);
        Task<UserDetailDTO> GetUserDetail(long userId);
        Task<bool> SendActivationSms(string mobile);
        #endregion

        #region User Management
        Task<FilterUserDto> FilterUser(FilterUserDto filter);
        Task<UserDetailDTO> UserDetail(long userId);
        Task<bool> BlockUser(long userId);
        Task<bool> UnBlockUser(long userId);
        #endregion

        #region Favorite Products
        Task<FilterFavoriteProductDto> FilterFavoriteProducts(FilterFavoriteProductDto filter);
        Task<bool> ToggleFavoriteProduct(long userId, long productId);
        Task<bool> IsProductFavorite(long productId, long userId);
        #endregion
    }
}
