using ZarmallStore.Application.Services.Interface;
using ZarmallStore.Data.DTOS.Account;
using ZarmallStore.Data.Entities.Account;
using ZarmallStore.Data.Repository;
using Microsoft.EntityFrameworkCore;


namespace ZarmallStore.Application.Services.Implementarions
{
    public class UserService : IUserService
    {
        #region CTOR
        private readonly IGenericRepository<User> _userRepository;
        public UserService(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        public async ValueTask DisposeAsync()
        {
            await _userRepository.DisposeAsync();
        }

        #endregion


        #region Register Methods

        public async Task RegisterOrLoginUser(RegisterUserDTO dto)
        {
            var checkUser = await CheckUserExistByMobile(dto.MobileNumber);
            if (checkUser)
            {
                var user = await _userRepository.GetQuery().FirstAsync(u => u.MobileNumber == dto.MobileNumber);
                user.MobileActivationNumber = new Random().Next(10000, 99999).ToString();
                _userRepository.EditEntity(user);
                await _userRepository.SaveAsync();
            }
            var newuser = new User
            {
                MobileNumber = dto.MobileNumber,
                MobileActivationNumber = new Random().Next(10000, 99999).ToString()
            };
            await _userRepository.AddEntity(newuser);
            await _userRepository.SaveAsync();
        }


        public async Task<bool> CheckUserExistByMobile(string mobile)
        {
            return await _userRepository.GetQuery().AnyAsync(u => u.MobileNumber == mobile);
        }

        public Task EditUserDetail(EditUserInfoDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<EditUserInfoDTO> GetEditUserDetail(long userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserDetailsDTO> GetUserDetails(long userId)
        {
            throw new NotImplementedException();
        }



        public Task<bool> SendActivationSms(string mobile)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
