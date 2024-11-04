using Dapper;
using Microsoft.Extensions.Configuration;
using PetProject.Models;
using System;
using System.Data;
using System.Threading.Tasks;

namespace PetProject.Repositories
{
    public class PrivateMessageRepository : IPrivateMessageRepository
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IConfiguration _config;

        public PrivateMessageRepository(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> DeleteMessageWithTime()
        {
            // Sử dụng UTC cho thời gian hiện tại
            double.TryParse(_config["AppSettings:DeleteMessageAfterHour"], out var deleteHours);
            var expirationDate = DateTime.UtcNow.AddHours(deleteHours);
            string sqlCommand = "DELETE FROM PrivateMessages WHERE CreatedAt < @ExpirationDate";

            var result = await _uow.Connection.ExecuteAsync(sqlCommand, transaction: _uow.Transaction);
            return result;
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
