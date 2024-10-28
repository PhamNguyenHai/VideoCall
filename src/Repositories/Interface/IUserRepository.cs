using System;
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

        /// <summary>
        /// Hàm thực hiện thêm mới người dùng vào hệ thống
        /// </summary>
        /// <param name="userToAdd">Thông tin người dùng cần thêm mới</param>
        /// <returns>Số bản ghi thêm mới</returns>
        Task<int> InsertAsync(User userToAdd);

        Task<UserModel?> FindUserByToken(string token);
    }
}
