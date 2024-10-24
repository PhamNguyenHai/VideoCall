using Dapper;
using NuGet.Common;
using PetProject.Services;
using System;
using System.Data;
using System.Threading.Tasks;

namespace PetProject.Repositories
{
    public class UserRepository : BaseRepository<User, UserModel>, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Task<int> ChangePasswordAsync(ChangePasswordDto changePasswordInfor)
        {
            throw new NotImplementedException();
        }

        public async Task<UserModel> FindUserByEmailOrPhoneNumber(string emailOrPhoneNumber)
        {
            string sqlCommand = $"SELECT * FROM View_Users vu WHERE (vu.Email = @EmailOrPhoneNumber OR vu.PhoneNumber = @EmailOrPhoneNumber)";

            var param = new DynamicParameters();
            param.Add($"@EmailOrPhoneNumber", emailOrPhoneNumber);

            var result = await _uow.Connection.QueryFirstOrDefaultAsync<UserModel>(sqlCommand, param, transaction: _uow.Transaction);
            return result;
        }

        public async Task<UserModel?> FindUserByLoginInforAsync(LoginInfor loginInfor)
        {
            string storedProcedureName = $"Proc_Users_CheckLogin";

            // Chuyển entity sang parametters để truyền vào procedure 
            var parametters = Utility.CreateParamettersFromEntity<LoginInfor>(loginInfor);

            var userInfor = await _uow.Connection.QueryFirstOrDefaultAsync<UserModel>(storedProcedureName, parametters,
                commandType: CommandType.StoredProcedure, transaction: _uow.Transaction);
            return userInfor;
        }

        public Task<bool> IsPasswordMatched(UserPasswordDto currentPasswordInfor)
        {
            throw new NotImplementedException();
        }
    }
}
