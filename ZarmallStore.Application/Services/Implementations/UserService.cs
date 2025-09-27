using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.Account;
using ZarmallStore.Data.DTOs.Paging;
using ZarmallStore.Data.Entities.Account;
using ZarmallStore.Data.Entities.OrderEntities;
using ZarmallStore.Data.Entities.ProductEntities;
using ZarmallStore.Data.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ZarmallStore.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        #region CTOR
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<FavoriteProduct> _favoriteRepository;
        private readonly ISmsService _smsService;

        public UserService(IGenericRepository<User> userRepository, IGenericRepository<Order> orderRepository, IGenericRepository<FavoriteProduct> favoriteRepository, ISmsService smsService)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _favoriteRepository = favoriteRepository;
            _smsService = smsService;
        }
        public async ValueTask DisposeAsync()
        {
            await _userRepository.DisposeAsync();
            await _orderRepository.DisposeAsync();
            await _favoriteRepository.DisposeAsync();
        }
        #endregion

        #region Register Methods
        public async Task RegisterOrLoginUser(RegisterUserDTO dto)
        {
            var checkUser = await CheckUserExistByMobile(dto.MobileNumber);

            if (checkUser)
            {
                var user = await _userRepository.GetQuery().FirstAsync(u => u.MobileNumber == dto.MobileNumber);
                user.MobileActivationNumer = new Random().Next(10000, 99999).ToString();
                _userRepository.EditEntity(user);
                await _userRepository.SaveAsync();
                await _smsService.SendVerificationSms(dto.MobileNumber, user.MobileActivationNumer);
            }
            else
            {
                var newUser = new User
                {
                    MobileNumber = dto.MobileNumber,
                    MobileActivationNumer = new Random().Next(10000, 99999).ToString()
                };
                await _userRepository.AddEntity(newUser);
                await _userRepository.SaveAsync();
                await _smsService.SendVerificationSms(dto.MobileNumber, newUser.MobileActivationNumer);
            }
        }

        public async Task<UserDashboardDetailDto> UserDashboardDetail(long userId)
        {
            var orders = await _orderRepository.GetQuery().Where(o => o.UserId == userId)
                .ToListAsync();
            return new UserDashboardDetailDto
            {
                User = await _userRepository.GetEntityById(userId),
                CanceledOrderCount = orders.Count(o => o.OrderState == OrderState.Canceled),
                PendingOrderCount = orders.Count(o => o.OrderState == OrderState.Pending),
                ReturnedOrderCount = orders.Count(o => o.OrderState == OrderState.Returned),
                SentOrderCount = orders.Count(o => o.OrderState == OrderState.Sent),
                PendingOrders = orders.Where(o => o.OrderState == OrderState.Pending).ToList()
            };
        }

        public async Task<bool> CheckUserExistByMobile(string mobile)
        {
            return await _userRepository.GetQuery().AnyAsync(u => u.MobileNumber == mobile);
        }

        public async Task<User> GetUserById(long userId)
        {
            return await _userRepository.GetEntityById(userId);
        }

        public async Task<bool> CheckMobileAuthorization(MobileActivationDTO dto)
        {
            var user = await GetUserByMobile(dto.Mobile);
            if (user == null) return false;

            var activationCode = $"{dto.ActivationCodePart1}{dto.ActivationCodePart2}{dto.ActivationCodePart3}{dto.ActivationCodePart4}{dto.ActivationCodePart5}";
            return activationCode == user.MobileActivationNumer;
        }

        public async Task<User?> GetUserByMobile(string mobile)
        {
            return await _userRepository.GetQuery().FirstOrDefaultAsync(u => u.MobileNumber == mobile);
        }

        public async Task<EditUserInfoDTO> GetEditUserDetail(long userId)
        {
            var user = await _userRepository.GetEntityById(userId);
            return new EditUserInfoDTO
            {
                UserId = userId,
                Address = user.Address,
                Email = user.Email,
                FullName = user.FullName,
                PostCode = user.PostCode
            };
        }

        public async Task EditUserDetail(EditUserInfoDTO dto)
        {
            var user = await _userRepository.GetEntityById(dto.UserId);

            user.Address = dto.Address;
            user.Email = dto.Email;
            user.FullName = dto.FullName;
            user.PostCode = dto.PostCode;
            user.UserCity = dto.UserCity;

            _userRepository.EditEntity(user);
            await _userRepository.SaveAsync();
        }

        public async Task<UserDetailDTO> GetUserDetail(long userId)
        {
            var user = await _userRepository.GetEntityById(userId);
            return new UserDetailDTO
            {
                Id = userId,
                Address = user.Address,
                Email = user.Email,
                FullName = user.FullName,
                PostCode = user.PostCode,
                MobileNumber = user.MobileNumber,
                CreateDate = user.CreateDate,
                LastUpdateDate = user.LastUpdateDate,
                IsDeleted = user.IsDeleted,
                MobileActivationNumer = user.MobileActivationNumer,
            };
        }

        public async Task<bool> SendActivationSms(string mobile)
        {
            var user = await _userRepository.GetQuery().FirstOrDefaultAsync(u => u.MobileNumber == mobile);
            if (user == null) return false;

            user.MobileActivationNumer = new Random().Next(10000, 99999).ToString();
            await _smsService.SendVerificationSms(mobile, user.MobileActivationNumer);
            return true;
        }
        #endregion

        #region User Management

        public async Task<FilterUserDto> FilterUser(FilterUserDto filter)
        {
            var query = _userRepository.GetQuery()
                .OrderByDescending(u => u.CreateDate)
                .AsQueryable();

            filter.UserCount = query.Count();

            switch (filter.UserState)
            {
                case FilterUserState.All:
                    break;
                case FilterUserState.BLocked:
                    query = query.Where(s => s.BlockUser);
                    break;
            }
            #region filter

            if (!string.IsNullOrEmpty(filter.Mobile))
                query = query.Where(s => EF.Functions.Like(s.MobileNumber.Trim(), $"%{filter.Mobile.Trim()}%"));

            if (!string.IsNullOrEmpty(filter.FullName))
                query = query.Where(s => EF.Functions.Like(s.FullName.Trim(), $"%{filter.FullName.Trim()}%"));

            if (!string.IsNullOrEmpty(filter.City))
                query = query.Where(s => EF.Functions.Like(s.UserCity.Trim(), $"%{filter.City.Trim()}%"));

            if (filter.UserId is > 0)
                query = query.Where(s => s.Id == filter.UserId.Value);
            #endregion

            #region paging

            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.HowManyShowPageAfterAndBefore);
            var allEntities = await query.Paging(pager).ToListAsync();

            #endregion

            return filter.SetData(allEntities).SetPaging(pager);
        }

        public async Task<UserDetailDTO> UserDetail(long userId)
        {
            var data = await _userRepository.GetEntityById(userId);
            return new UserDetailDTO
            {
                FullName = data.FullName,
                Id = data.Id,
                CreateDate = data.CreateDate,
                LastUpdateDate = data.LastUpdateDate,
                MobileNumber = data.MobileNumber,
                MobileActivationNumer = data.MobileActivationNumer,
                City = data.UserCity,
                Address = data.Address,
                PostCode = data.PostCode,
                Email = data.Email,
            };
        }

        public async Task<bool> BlockUser(long userId)
        {
            var user = await _userRepository.GetQuery().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            user.BlockUser = true;
            _userRepository.EditEntity(user);
            await _userRepository.SaveAsync();
            return true;
        }

        public async Task<bool> UnBlockUser(long userId)
        {
            var user = await _userRepository.GetQuery().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            user.BlockUser = false;
            _userRepository.EditEntity(user);
            await _userRepository.SaveAsync();
            return true;
        }
        #endregion

        #region Favorite Product
        public async Task<FilterFavoriteProductDto> FilterFavoriteProducts(FilterFavoriteProductDto filter)
        {
            #region query
            var query = _favoriteRepository.GetQuery()
                .Include(f => f.Product)
                .OrderByDescending(d => d.CreateDate).AsQueryable();
            #endregion

            #region Filter
            if (!string.IsNullOrEmpty(filter.ProductTitle))
            {
                query = query.Where(c => EF.Functions.Like(c.Product.Title, $"{filter.ProductTitle}"));
            }

            if (filter.UserId is > 0)
            {
                query = query.Where(f => f.UserId == filter.UserId);
            }
            #endregion

            #region Paging
            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.HowManyShowPageAfterAndBefore);
            var allEntities = await query.Paging(pager).ToListAsync();
            #endregion

            return filter.SetData(allEntities).SetPaging(pager);
        }
        public async Task<bool> ToggleFavoriteProduct(long userId, long productId)
        {
            var favorite = await IsProductFavorite(productId , userId);

            if (favorite)
            {
                var favoriteProduct = await _favoriteRepository.GetQuery()
                    .FirstAsync(f => f.UserId == userId && f.ProductId == productId);
                await _favoriteRepository.DeletePermanent(favoriteProduct);
                await _favoriteRepository.SaveAsync();
                return false; // Product removed from favorite list
            }

            var data = new FavoriteProduct
            {
                ProductId = productId,
                UserId = userId,
            };
            await _favoriteRepository.AddEntity(data);
            await _favoriteRepository.SaveAsync();
            return true; // Product Is Favorite Now
        }
        public async Task<bool> IsProductFavorite(long productId, long userId)
        {
            return await _favoriteRepository.GetQuery().AnyAsync(f => f.UserId == userId && f.ProductId == productId);
        }
        #endregion
    }
}
