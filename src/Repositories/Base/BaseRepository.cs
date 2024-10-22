using Dapper;
using PetProject.Repositories;
using PetProject.UnitOfWork;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetProject.Services
{
    public abstract class BaseRepository<TEntity, TModel> : ReadOnlyRepository<TEntity, TModel>, IBaseRepository<TEntity, TModel>
        where TEntity : IHasKey
    {
        #region Constructor
        protected BaseRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Hàm thực hiện thêm mới đối tượng
        /// </summary>
        /// <param name="entityCreateDto">Đối tượng cần thêm mới</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        /// Author: PNNHai
        /// Date: 
        public async Task<int> InsertAsync(TEntity entity)
        {
            string storedProcedureName = $"Proc_{EntityName}_Insert";

            // Chuyển entity sang parametters để truyền vào procedure 
            var parametters = CreateParamettersFromEntity(entity);

            var effectedRows = await _uow.Connection.ExecuteAsync(storedProcedureName, parametters,
                commandType: CommandType.StoredProcedure, transaction: _uow.Transaction);
            return effectedRows;
        }

        /// <summary>
        /// Hàm cập nhật đối tượng
        /// </summary>
        /// <param name="id">Id của đối tượng cần cập nhật</param>
        /// <param name="entityUpdateDto">Thông tin đối tượng cần cập nhật</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        /// Author: PNNHai
        /// Date: 
        public async Task<int> UpdateAsync(TEntity entity)
        {
            string storedProcedureName = $"Proc_{EntityName}_Update";

            // Chuyển entity sang parametters để truyền vào procedure
            var parametters = CreateParamettersFromEntity(entity);

            var effectedRows = await _uow.Connection.ExecuteAsync(storedProcedureName, parametters,
                commandType: CommandType.StoredProcedure, transaction: _uow.Transaction);
            return effectedRows;
        }

        /// <summary>
        /// Hàm xóa đối tượng
        /// </summary>
        /// <param name="id">Id của đối tượng cần xóa</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        /// Author: PNNHai
        /// Date: 
        public async Task<int> DeleteAsync(TEntity entity)
        {
            string storedProcedureName = $"Proc_{EntityName}_Delete";

            var param = new DynamicParameters();
            param.Add($"i_{EntityId}", entity.GetKey());

            var effectedRows = await _uow.Connection.ExecuteAsync(storedProcedureName, param,
                commandType: CommandType.StoredProcedure, transaction: _uow.Transaction);
            return effectedRows;
        }

        /// <summary>
        /// Hàm tạo param
        /// </summary>
        /// <param name="entity">Đối tượng cần tạo</param>
        /// <returns>DynamicParameters</returns>
        /// Author: PNNHai
        /// Date: 
        private DynamicParameters CreateParamettersFromEntity(TEntity entity)
        {
            var parameters = new DynamicParameters();
            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyName = $"i_{property.Name}";
                var propertyValue = property.GetValue(entity);
                parameters.Add(propertyName, propertyValue);
            }
            return parameters;
        }
        #endregion
    }
}
