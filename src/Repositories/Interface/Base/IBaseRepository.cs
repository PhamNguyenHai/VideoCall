using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetProject.Repositories
{
    public interface IBaseRepository<TEntity, TModel> : IReadOnlyRepository<TEntity, TModel>
    {
        /// <summary>
        /// Thêm mới dữ liệu
        /// </summary>
        /// <param name="entity">Đối tượng cần thêm mới</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> InsertAsync(TEntity entity);

        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="entity">Thông tin sẽ sửa</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> UpdateAsync(TEntity entity);

        /// <summary>
        /// Xóa dữ liệu
        /// </summary>
        /// <param name="entity">Đối tượng cần xóa</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> DeleteAsync(TEntity entity);
    }
}
