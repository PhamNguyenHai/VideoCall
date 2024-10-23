using Dapper;
using PetProject;
using System.Data;
using System.Threading.Tasks;

namespace PetProject.Repositories
{
    public class SessionRepositoy : ISessionRepository
    {
        protected readonly IUnitOfWork _uow;

        public SessionRepositoy(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> CreateSession(Session session)
        {
            string storedProcedureName = $"Proc_Sessions_Insert";

            // Chuyển entity sang parametters để truyền vào procedure 
            var parametters = Utility.CreateParamettersFromEntity<Session>(session);

            var effectedRows = await _uow.Connection.ExecuteAsync(storedProcedureName, parametters,
                commandType: CommandType.StoredProcedure, transaction: _uow.Transaction);
            return effectedRows;
        }

        public async Task<Session?> GetSessionByToken(string token)
        {
            string sqlCommand = $"SELECT * FROM View_Sessions WHERE Token = @Token";

            var param = new DynamicParameters();
            param.Add($"@Token", token);

            var result = await _uow.Connection.QueryFirstOrDefaultAsync<Session>(sqlCommand, param, transaction: _uow.Transaction);
            return result;
        }
    }
}
