using Dapper;
using PetProject.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetProject.Services
{
    public abstract class BaseRepository<TEntity, TViewModel> : ReadOnlyRepository<TEntity, TViewModel>, IBaseRepository<TEntity, TViewModel>
        where TEntity : IHasKey
    {

        #region Constructor
        protected BaseRepository(IUnitOfWork unitOfWork, ILoggerCustom logger) : base(unitOfWork, logger)
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
            string storedProcedureName = $"Proc_{EntityName}s_Insert";

            try
            {
                // Chuyển entity sang parametters để truyền vào procedure 
                //var parametters = CreateParamettersFromEntity(entity);
                var parametters = Utility.CreateParamettersFromEntity<TEntity>(entity);

                var effectedRows = await _uow.Connection.ExecuteAsync(storedProcedureName, parametters,
                    commandType: CommandType.StoredProcedure, transaction: _uow.Transaction);

                // Log vào hệ thống
                _logger.LogInfo($"Thêm mới {EntityName}: {entity.GetKey()}");
                return effectedRows;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi thêm {EntityName}: {ex.Message}", ex);
                throw; // Ném lại ngoại lệ sau khi ghi log
            }
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
            string storedProcedureName = $"Proc_{EntityName}s_Update";

            try
            {
                // Chuyển entity sang parametters để truyền vào procedure
                //var parametters = CreateParamettersFromEntity(entity);
                var parametters = Utility.CreateParamettersFromEntity<TEntity>(entity);


                var effectedRows = await _uow.Connection.ExecuteAsync(storedProcedureName, parametters,
                    commandType: CommandType.StoredProcedure, transaction: _uow.Transaction);

                // Log vào hệ thống
                _logger.LogInfo($"Cập nhật {EntityName}: {entity.GetKey()}");
                return effectedRows;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi cập nhật {EntityName}: {ex.Message}", ex);
                throw; // Ném lại ngoại lệ sau khi ghi log
            }
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
            string storedProcedureName = $"Proc_{EntityName}s_Delete";

            try
            {
                var param = new DynamicParameters();
                param.Add($"i_{EntityId}", entity.GetKey());

                var effectedRows = await _uow.Connection.ExecuteAsync(storedProcedureName, param,
                    commandType: CommandType.StoredProcedure, transaction: _uow.Transaction);

                // Log vào hệ thống
                _logger.LogInfo($"Xóa {EntityName} với ID: {entity.GetKey()}");
                return effectedRows;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa {EntityName}: {ex.Message}", ex);
                throw; // Ném lại ngoại lệ sau khi ghi log
            }
        }
        #endregion
    }
}
