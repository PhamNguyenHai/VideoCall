using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using static Dapper.SqlMapper;
using System.Linq;

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

        public async Task<FilterResult<TModel>> FilterPagingAsync(FilterInput filterInput)
        {
            // Khởi tạo tham số cho Dapper
            var param = new DynamicParameters();
            param.Add("@SearchString", filterInput.SearchString ?? string.Empty);
            param.Add("@PageNumber", filterInput.PageNumber);
            param.Add("@PageSize", filterInput.PageSize);

            // Tính tổng số bản ghi
            string countSql = $@"
                SELECT COUNT(*) 
                FROM View_{EntityName}s 
                WHERE " + GenerateSearchCondition(filterInput.SearchColumns, "@SearchString") + ";";

            // Thực hiện truy vấn để lấy tổng số bản ghi
            int totalRecords = await _uow.Connection.ExecuteScalarAsync<int>(countSql, param);

            // Xác định cột sắp xếp
            string orderColumn = string.IsNullOrWhiteSpace(filterInput.OrderColumn) ? "ModifiedDate" : filterInput.OrderColumn;
            string orderDirection = filterInput.IsSortDesc == true ? "DESC" : "ASC";

            // Tạo câu lệnh SQL cho dữ liệu phân trang hoặc lấy tất cả nếu PageNumber = -1
            string dataSql;

            if (filterInput.PageNumber == -1)
            {
                dataSql = $@"SELECT * FROM View_{EntityName}s  WHERE " + GenerateSearchCondition(filterInput.SearchColumns, "@SearchString") + 
                    $@" ORDER BY {orderColumn} {orderDirection};";
            }
            else
            {
                // Tạo câu lệnh SQL cho dữ liệu phân trang
                dataSql = $@"SELECT * FROM View_{EntityName}s WHERE " + GenerateSearchCondition(filterInput.SearchColumns, "@SearchString") + 
                    $@" ORDER BY {orderColumn} {orderDirection} OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;";
            }
            
            // Thực hiện truy vấn để lấy dữ liệu
            var data = await _uow.Connection.QueryAsync<TModel>(dataSql, param);

            // Tính toán tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalRecords / filterInput.PageSize);

            // Trả về kết quả
            return new FilterResult<TModel>(
                filterInput.PageNumber,
                filterInput.PageSize,
                totalPages,
                totalRecords,
                data);
        }

        // Phương thức để tạo điều kiện tìm kiếm
        private string GenerateSearchCondition(string[] searchColumns, string paramName)
        {
            if (searchColumns == null || searchColumns.Length == 0)
            {
                return "1=1"; // Trả về điều kiện luôn đúng nếu không có cột nào
            }

            var conditions = searchColumns
                .Select(column => $"{column} LIKE '%' + {paramName} + '%'");

            return string.Join(" OR ", conditions);
        }

        #endregion
    }
}
