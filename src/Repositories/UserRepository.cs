using Dapper;
using NuGet.Common;
using PetProject.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetProject.Repositories
{
    public class UserRepository : BaseRepository<User, UserModel>, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork, ILoggerCustom logger) : base(unitOfWork, logger)
        {
        }

        public Task<int> ChangePasswordAsync(ChangePasswordDto changePasswordInfor)
        {
            throw new NotImplementedException();
        }

        public async Task<UserModel> FindUserByEmailOrPhoneNumber(string emailOrPhoneNumber)
        {
            string sqlCommand = "SELECT * FROM View_Users vu WHERE (vu.Email = @EmailOrPhoneNumber OR vu.PhoneNumber = @EmailOrPhoneNumber)";

            var param = new DynamicParameters();
            param.Add($"@EmailOrPhoneNumber", emailOrPhoneNumber);

            var result = await _uow.Connection.QueryFirstOrDefaultAsync<UserModel>(sqlCommand, param, transaction: _uow.Transaction);
            return result;
        }

        public async Task<UserModel?> FindUserByLoginInforAsync(LoginInfor loginInfor)
        {
            string storedProcedureName = "Proc_Users_CheckLogin";

            // Chuyển entity sang parametters để truyền vào procedure 
            var parametters = Utility.CreateParamettersFromEntity<LoginInfor>(loginInfor);

            var userInfor = await _uow.Connection.QueryFirstOrDefaultAsync<UserModel>(storedProcedureName, parametters,
                commandType: CommandType.StoredProcedure, transaction: _uow.Transaction);
            return userInfor;
        }

        public async Task<UserModel?> FindUserByToken(string token)
        {
            string storedProcedureName = "Proc_Users_GetUserByToken";

            var param = new DynamicParameters();
            param.Add("@Token", token);

            var userInfor = await _uow.Connection.QueryFirstOrDefaultAsync<UserModel>(storedProcedureName, param,
                commandType: CommandType.StoredProcedure, transaction: _uow.Transaction);
            return userInfor;
        }

        public Task<bool> IsPasswordMatched(UserPasswordDto currentPasswordInfor)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserWithFriendsModel>> GetUserFriendsByUserId(Guid userId)
        {
            string sqlCommand = "SELECT * FROM Users u JOIN Friends f ON (f.UserId = @UserId AND f.FriendUserId = u.UserId) OR (f.FriendUserId = @UserId AND f.UserId = u.UserId) WHERE f.UserId = @UserId OR f.FriendUserId = @UserId";

            var param = new DynamicParameters();
            param.Add($"@UserId", userId);

            var result = await _uow.Connection.QueryAsync<UserWithFriendsModel>(sqlCommand, param, transaction: _uow.Transaction);
            return result;
        }
    }
}
