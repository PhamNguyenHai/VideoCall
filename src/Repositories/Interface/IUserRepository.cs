using PetProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetProject.Repositories
{
    public interface IUserRepository : IBaseRepository<User, UserModel>
    {
        /// <summary>
        /// Thực hiện lấy thông tin user thông qua thông tin đăng nhập người dùng 
        /// </summary>
        /// <param name="loginInfor">Thông tin đăng nhập</param>
        /// <returns>Người dùng thỏa mãn</returns>
        /// Author: PNNHai
        /// Date:
        Task<UserModel?> FindUserByLoginInforAsync(LoginInfor loginInfor);

        /// <summary>
        /// Thực hiện đổi mật khẩu cho tài khoản
        /// </summary>
        /// <param name="changePasswordInfor">Thông tin đổi mật khẩu</param>
        /// <returns></returns>
        /// Author: PNNHai
        /// Date:
        Task<int> ChangePasswordAsync(ChangePasswordDto changePasswordInfor);

        /// <summary>
        /// Thực hiện kiểm tra xem mật khẩu truyền vào có phải là mật khẩu hiện tại của người dùng với id không
        /// </summary>
        /// <param name="currentPasswordInfor">Thông tin kiểm tra mật khẩu</param>
        /// <returns>true (nếu mật khẩu match) || false (nếu mật khẩu ko match)</returns>
        Task<bool> IsPasswordMatched(UserPasswordDto currentPasswordInfor);

        /// <summary>
        /// Thực hiện tìm người dùng thông qua email
        /// </summary>
        /// <param name="email">Email của người dùng muốn thực hiện tìm kiếm</param>
        /// <returns>Người dùng || null</returns>
        /// Author: PNNHai
        /// Date:
        Task<UserModel?> FindUserByEmailOrPhoneNumber(string emailOrPhoneNumber);

        Task<int> UpdateFriendStatus(Guid userId, Guid friendId, FriendStatus status);

        Task<UserModel?> FindUserByToken(string token);

        Task<IEnumerable<FriendRelationship>> GetUserFriendsByUserId(Guid userId);
        Task<FriendRelationship> GetUserFriendByUserIdAndFriendId(Guid userId, Guid friendId);
        Task<IEnumerable<UserPrivateChat>> GetPrivateChatsByUserId(Guid userId);
        Task<IEnumerable<FriendRelationship>> FilterUser(Guid userId, string searchString);
        Task<UserMessage> GetUserPrivateMessagesByUserIdAndPartnerId(Guid userId, Guid partnerId);

        //Task<IEnumerable<FriendRelationship>> GetMakeFriendByUserId(Guid userId);

    }
}
