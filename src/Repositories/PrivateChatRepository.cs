﻿using Dapper;
using System.Data;
using System;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace PetProject.Repositories
{
    public class PrivateChatRepository : IPrivateChatRepository
    {
        protected readonly IUnitOfWork _uow;

        public PrivateChatRepository(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> InsertAsync(PrivateChat privateChat)
        {
            string storedProcedureName = "Proc_PrivateChats_Insert";

            // Chuyển entity sang parametters để truyền vào procedure 
            //var parametters = CreateParamettersFromEntity(entity);
            var parametters = Utility.CreateParamettersFromEntity<PrivateChat>(privateChat);

            var effectedRows = await _uow.Connection.ExecuteAsync(storedProcedureName, parametters,
                commandType: CommandType.StoredProcedure, transaction: _uow.Transaction);

            return effectedRows;
        }
    }
}
