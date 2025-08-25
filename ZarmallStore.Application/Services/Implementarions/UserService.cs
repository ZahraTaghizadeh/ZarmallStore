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
        private readonly ISmsService _smsService;
        public UserService(IGenericRepository<User> userRepository, ISmsService smsService)
        {
            _userRepository = userRepository;
            _smsService = smsService;
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
                await _smsService.SendVerificationSms(dto.MobileNumber, user.MobileActivationNumber);
            }
            var newUser = new User
            {
                MobileNumber = dto.MobileNumber,
                MobileActivationNumber = new Random().Next(10000, 99999).ToString()
            };
            await _userRepository.AddEntity(newUser);
            await _userRepository.SaveAsync();
            await _smsService.SendVerificationSms(dto.MobileNumber, newUser.MobileActivationNumber);
        }


        public async Task<bool> CheckUserExistByMobile(string mobile)
        {
            return await _userRepository.GetQuery().AnyAsync(u => u.MobileNumber == mobile);
        }

        public Task EditUserDetail(EditUserInfoDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<EditUserInfoDTO> GetEditUserDetail(long userId)
        {
            var user = await _userRepository.GetEntityById(userId);
            return new EditUserInfoDTO
            {
                Address = user.Address,
                Email = user.Email,
                FullName = user.FullName,
                PostCode = user.PostCode
            };
        }

        public Task<UserDetailsDTO> GetUserDetails(long userId)
        {
            throw new NotImplementedException();
        }



        public async Task<bool> SendActivationSms(string mobile)
        {
            var user = await _userRepository.GetQuery().FirstOrDefaultAsync(u => u.MobileNumber == mobile);
            if (user == null)
                return false;
            user.MobileActivationNumber = new Random().Next(10000,99999).ToString();
            await _smsService.SendVerificationSms(mobile,user.MobileActivationNumber);
            return true;
        }

        #endregion
    }
}
