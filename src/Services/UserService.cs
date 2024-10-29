using AutoMapper;
using NuGet.Common;
using PetProject.Models;
using PetProject.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetProject.Services
{
    public class UserService : BaseService<User, UserModel, UserViewModel, UserCreateDto, UserUpdateDto>, IUserService
    {
        protected readonly IUserRepository _userRepository;
        protected readonly ISessionRepository _sessionRepository;

        public UserService(IMapper mapper, IUserRepository userRepository, ISessionRepository sessionRepository) : base(userRepository, mapper)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
        }

        public override async Task<Result> ValidateForInserting(UserCreateDto entityCreateDto)
        {
            return await Task.FromResult(new Result() { Success = true });
        }

        public override async Task<Result> ValidateForUpdating(Guid id, UserUpdateDto entityUpdateDto)
        {
            return await Task.FromResult(new Result() { Success = true });

        }

        public Task<Guid> ChangePasswordAsync(Guid id, ChangePasswordDto userPasswordChange)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> LoginAsync(LoginInfor userToLogin)
        {
            var loginUser = await _userRepository.FindUserByLoginInforAsync(userToLogin);
            if(loginUser == null)
            {
                return new Result 
                { 
                    Success = false, 
                    Message = "Thông tin đăng nhập không chính xác" 
                };
            }

            // Kiểm tra xem tài khoản có bị block ko
            if (loginUser.IsBlocked == true)
            {
                return new Result { Success = false, Message = "Tài khoản đã bị chặn." };
            }

            // Tạo phiên làm việc
            var session = new Session
            {
                SessionId = Guid.NewGuid(),
                UserId = loginUser.UserId,
                Token = Guid.NewGuid().ToString(), // Tạo token mới
                ExpirationDate = DateTime.UtcNow.AddHours(1), // Ví dụ: thời gian sống 1 giờ
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            await _sessionRepository.CreateSession(session);

            return new Result
            {
                Success = true,
                Message = "Đăng nhập thành công.",
                Data = session
            };
        }

        public async Task<Result> LogoutAsync(string token)
        {
            // Xóa hoặc vô hiệu hóa phiên làm việc
            var session = await _sessionRepository.GetSessionByToken(token);
            if (session != null)
            {
                // Có thể xóa phiên hoặc đánh dấu là không hợp lệ
                int effectedRow = await _sessionRepository.DeleteSession(session.SessionId);
                if (effectedRow > 0)
                {
                    return new Result
                    {
                        Success = true,
                        Message = "Đăng xuất thành công"
                    };
                }
                else
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Đăng xuất không thành công",
                        Data = token
                    };
                }
            }
            return new Result
            {
                Success = false,
                Message = "Phiên đăng nhập không tồn tại"
            };
        }

        public Task<UserViewModel> ResetPassword(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserViewModel?> FindUserByToken(string token)
        {
            var entity = await _userRepository.FindUserByToken(token);
            var entityDto = _mapper.Map<UserViewModel>(entity);
            return entityDto;
        }

        public async Task<IEnumerable<UserWithFriendsModel>> GetUserFriendsByUserId(Guid userId)
        {
            var friends = await _userRepository.GetUserFriendsByUserId(userId);
            return friends;
        }
    }
}