using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PetProject.Services
{
    public interface IUserService : IBaseService<UserViewModel, UserCreateDto, UserUpdateDto>
    {
        /// <summary>
        /// Thực hiện đăng nhập tài khoản
        /// </summary>
        /// <param name="userToLogin">Thông tin đăng nhập người dùng</param>
        /// <returns>Thông tin người dùng đăng nhập thành công</returns>
        /// Author: PNNHai
        /// Date:
        Task<Result> LoginAsync(LoginInfor userToLogin);

        /// <summary>
        /// Thực hiện đăng xuất
        /// </summary>
        /// <returns></returns>
        /// Author: PNNHai
        /// Date:
        Task<Result> LogoutAsync(string token);

        /// <summary>
        /// Thực hiện đổi mật khẩu cho tài khoản
        /// </summary>
        /// <param name="id">Mã định danh của người dùng cần thay mật khẩu</param>
        /// <param name="userPasswordChange">Thông tin tài khoản cần thực hiện thay đổi và mật khẩu thay đổi</param>
        /// <returns></returns>
        /// Author: PNNHai
        /// Date:
        Task<Guid> ChangePasswordAsync(Guid id, ChangePasswordDto userPasswordChange);

        /// <summary>
        /// Thực hiện reset mật khẩu người dùng
        /// </summary>
        /// <param name="id">Id tài khoản cần reset</param>
        /// <returns></returns>
        Task<UserViewModel> ResetPassword(Guid id);

        /// <summary>
        /// Thực hiện lấy thông tin người dùng thông qua token
        /// </summary>
        /// <param name="token">token người dùng</param>
        /// <returns></returns>
        Task<UserViewModel?> FindUserByToken(string token);
        Task<IEnumerable<FriendRelationship>> GetUserFriendsByUserId(Guid userId);
        Task<IEnumerable<UserPrivateChat>> GetPrivateChatsByUserId(Guid userId);

        Task<IEnumerable<FriendRelationship>> FilterUser(Guid userId, string searchString);

    }
}