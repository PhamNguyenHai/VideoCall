using AutoMapper;
using NuGet.Common;
using PetProject.Models;
using PetProject.Repositories;
using System;
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

        public override Task<Result> ValidateForInserting(UserCreateDto entityCreateDto)
        {
            throw new NotImplementedException();
        }

        public override Task<Result> ValidateForUpdating(Guid id, UserUpdateDto entityUpdateDto)
        {
            throw new NotImplementedException();
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
                Data = session.Token
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
    }
}