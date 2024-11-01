using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace PetProject.Repositories
{
    public class PrivateMessageRepository : IPrivateMessageRepository
    {
        protected readonly IUnitOfWork _uow;

        public PrivateMessageRepository(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> InsertAsync(PrivateMessage privateMessage)
        {
            string storedProcedureName = "Proc_PrivateMessages_Insert";

            // Chuyển entity sang parametters để truyền vào procedure
            var parametters = Utility.CreateParamettersFromEntity<PrivateMessage>(privateMessage);

            var effectedRows = await _uow.Connection.ExecuteAsync(storedProcedureName, parametters,
                commandType: CommandType.StoredProcedure, transaction: _uow.Transaction);

            return effectedRows;
        }
    }
}
