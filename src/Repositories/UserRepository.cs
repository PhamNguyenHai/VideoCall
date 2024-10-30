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

        public async Task<IEnumerable<FriendRelationship>> GetUserFriendsByUserId(Guid userId)
        {
            string sqlCommand = "SELECT u.*, f.Status FROM View_Users u JOIN Friends f ON (f.UserId = @UserId AND f.FriendUserId = u.UserId) OR (f.FriendUserId = @UserId AND f.UserId = u.UserId) WHERE f.UserId = @UserId OR f.FriendUserId = @UserId";

            var param = new DynamicParameters();
            param.Add($"@UserId", userId);

            var result = await _uow.Connection.QueryAsync<UserViewModel, FriendStatus, FriendRelationship>(
                                sqlCommand,
                                (user, status) =>
                                {
                                    return new FriendRelationship
                                    {
                                        User = user,
                                        Status = status
                                    };
                                },
                                param,
                                splitOn: "Status",
                                transaction: _uow.Transaction
                            );
            return result;
        }

        public async Task<IEnumerable<UserPrivateChat>> GetPrivateChatsByUserId(Guid userId)
        {
            string sqlCommand = "SELECT u.*, pc.ChatId FROM dbo.PrivateChats pc JOIN view_Users u ON (pc.UserId = @UserId AND pc.PartnerId = u.UserId) OR (pc.PartnerId = @UserId AND pc.UserId = u.UserId) WHERE pc.UserId = @UserId OR pc.PartnerId = @UserId ORDER BY pc.ModifiedDate;";

            var param = new DynamicParameters();
            param.Add($"@UserId", userId);

            var result = await _uow.Connection.QueryAsync<UserViewModel, Guid, UserPrivateChat>(
                                sqlCommand,
                                (user, chatId) =>
                                {
                                    return new UserPrivateChat
                                    {
                                        User = user,
                                        ChatId = chatId
                                    };
                                },
                                param,
                                splitOn: "ChatId",
                                transaction: _uow.Transaction
                            );
            return result;
        }

        public async Task<IEnumerable<FriendRelationship>> FilterUser(Guid userId, string searchString)
        {
            string sqlCommand = "SELECT u.*, f.Status FROM View_Users u LEFT JOIN Friends f ON (f.UserId = @UserId AND f.FriendUserId = u.UserId) OR (f.FriendUserId = @UserId AND f.UserId = u.UserId)" +
                                " WHERE u.UserId <> @UserId AND u.FullName COLLATE Vietnamese_CI_AI LIKE '%' + @SearchString + '%' ORDER BY CASE WHEN f.Status IS NOT NULL THEN 0 ELSE 1 END, u.FullName;";


            var param = new DynamicParameters();
            param.Add($"@UserId", userId);
            param.Add($"@SearchString", searchString);

            var result = await _uow.Connection.QueryAsync<UserViewModel, FriendStatus?, FriendRelationship>(
                                sqlCommand,
                                (user, status) =>
                                {
                                    return new FriendRelationship
                                    {
                                        User = user,
                                        Status = status
                                    };
                                },
                                param,
                                splitOn: "Status",
                                transaction: _uow.Transaction
                            );
            return result;
        }
    }
}
