using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PetProject.Repositories
{
    public interface IReadOnlyRepository<TEntity, TModel>
    {
        /// <summary>
        /// Lấy toàn bộ danh sách
        /// </summary>
        /// <returns>Danh sách phần tử</returns>
        /// Author: PNNHai
        /// Date: 
        Task<IEnumerable<TModel>> GetAllAsync();

        /// <summary>
        /// Lấy phần tử theo id
        /// </summary>
        /// <param name="id">Mã định danh của phần tử</param>
        /// <returns>Phần tử tương ứng</returns>
        /// Author: PNNHai
        /// Date: 
        Task<TModel> GetByIdAsync(Guid id);

        /// <summary>
        /// Tìm kiếm phần tử theo id (Nếu ko có -> null)
        /// </summary>
        /// <param name="id">Mã định danh của phần tử</param>
        /// <returns>Phần tử tương ứng</returns>
        /// Author: PNNHai
        /// Date: 
        Task<TModel?> FindByIdAsync(Guid id);

        /// <summary>
        /// Lọc dữ liệu kết hợp phân trang và tìm kiếm
        /// </summary>
        /// <param name="filterInput">Tham số lọc</param>
        /// <returns>Kết quả lọc đã phân trang</returns>
        /// Author: PNNHai
        /// Date: 
        Task<FilterResult<TModel>> FilterPagingAsync(FilterInput filterInput);
    }
}
