using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PetProject.Services
{
    public interface IBaseService<TViewModel, TEntityCreateDto, TEntityUpdateDto>
        : IReadOnlyService<TViewModel>
    {
        /// <summary>
        /// Service thêm mới đối tượng
        /// </summary>
        /// <param name="entityCreateDto">Phần tử cần thêm mới</param>
        /// Author: PNNHai
        /// Date: 
        Task<Result> CreateAsync(TEntityCreateDto entityCreateDto);

        /// <summary>
        /// Service cập nhật đối tượng
        /// </summary>
        /// <param name="id">Mã định danh cần cập nhật</param>
        /// <param name="entityUpdateDto">Thông tin đối tượng update</param>
        /// Author: PNNHai
        /// Date: 
        Task<Result> UpdateAsync(Guid id, TEntityUpdateDto entityUpdateDto);

        /// <summary>
        /// Service xóa đối tượng
        /// </summary>
        /// <param name="id">Mã định danh của đối tượng cần xóa</param>
        /// Author: PNNHai
        /// Date: 
        Task<Result> DeleteAsync(Guid id);
    }
}
