using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using static Dapper.SqlMapper;

namespace PetProject.Repositories
{
    public abstract class ReadOnlyRepository<TEntity, TModel> : IReadOnlyRepository<TEntity, TModel> where TEntity : IHasKey
    {
        #region Fields
        protected readonly IUnitOfWork _uow;
        protected readonly ILoggerCustom _logger;


        // Tên bảng (Phương thức virtual có thể ghi đè lại. Mặc định lấy tên bảng là entity đó)
        public virtual string EntityName { get; protected set; } = typeof(TEntity).Name;

        // Id của bảng ứng với entity đó
        public virtual string EntityId { get; protected set; } = typeof(TEntity).Name + "Id";
        #endregion

        #region Constructor
        public ReadOnlyRepository(IUnitOfWork unitOfWork, ILoggerCustom logger)
        {
            _uow = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Hàm lấy toàn bộ danh sách đối tượng
        /// </summary>
        /// <returns>Danh sách đối tượng </returns>
        /// Author: PNNHai
        /// Date: 
        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            string sqlCommand = $"SELECT * FROM {EntityName}s";
            var result = await _uow.Connection.QueryAsync<TModel>(sqlCommand, transaction: _uow.Transaction);
            _logger.LogInfo($"Lấy danh sách dữ liệu {EntityName}");

            return result;
        }

        /// <summary>
        /// Hàm lấy đối tượng theo id
        /// </summary>
        /// <param name="id">Mã định danh đối tượng</param>
        /// <returns>Đối tượng cần lấy</returns>
        /// Author: PNNHai
        /// Date: 
        public async Task<TModel> GetByIdAsync(Guid id)
        {
            var entity = await FindByIdAsync(id);
            if (entity == null)
                throw new NotFoundException("Không tìm thấy dữ liệu");
            return entity;
        }

        /// <summary>
        /// Tìm kiếm đối tượng theo id
        /// </summary>
        /// <param name="id">id đối tượng</param>
        /// <returns>Đối tượng cần tìm</returns>
        /// Author: PNNHai
        /// Date: 
        public async Task<TModel?> FindByIdAsync(Guid id)
        {
            string sqlCommand = $"SELECT * FROM {EntityName}s WHERE {EntityId} = @EntityId";

            var param = new DynamicParameters();
            param.Add($"@EntityId", id);

            var result = await _uow.Connection.QueryFirstOrDefaultAsync<TModel>(sqlCommand, param, transaction: _uow.Transaction);
            _logger.LogInfo($"Lấy {EntityName} by id: {id}");
            return result;
        }
        #endregion
    }
}
