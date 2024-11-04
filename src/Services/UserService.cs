using AutoMapper;
using NuGet.Common;
using PetProject.Models;
using PetProject.Repositories;
using PetProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

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

        public async Task<IEnumerable<FriendRelationship>> GetUserFriendsByUserId(Guid userId)
        {
            var friends = await _userRepository.GetUserFriendsByUserId(userId);
            return friends;
        }

        public async Task<FriendRelationship> GetUserFriendByUserIdAndFriendId(Guid userId, Guid friendId)
        {
            var friends = await _userRepository.GetUserFriendByUserIdAndFriendId(userId, friendId);
            return friends;
        }

        public async Task<IEnumerable<UserPrivateChat>> GetPrivateChatsByUserId(Guid userId)
        {
            var chats = await _userRepository.GetPrivateChatsByUserId(userId);
            return chats;
        }

        public async Task<IEnumerable<FriendRelationship>> FilterUser(Guid userId, string searchString)
        {
            var users = await _userRepository.FilterUser(userId, searchString);
            return users;
        }

        public async Task<UserMessage> GetUserPrivateMessagesByUserIdAndPartnerId(Guid userId, Guid partnerId)
        {
            var messages = await _userRepository.GetUserPrivateMessagesByUserIdAndPartnerId(userId, partnerId);
            messages.Partner = await GetByIdAsync(partnerId);
            return messages;
        }

        public async Task<Result> UpdateFriendStatus(Guid userId, Guid friendId, FriendStatus status)
        {
            // insert vào DB
            int effectedRow = await _userRepository.UpdateFriendStatus(userId, friendId, status);
            if (effectedRow > 0)
            {
                return new Result
                {
                    Success = true,
                    Message = "Cập nhật trạng thái thành công",
                    Data = userId
                };
            }
            else
            {
                return new Result
                {
                    Success = false,
                    Message = "Thêm dữ liệu không thành công",
                };
            }
        }
    }
}