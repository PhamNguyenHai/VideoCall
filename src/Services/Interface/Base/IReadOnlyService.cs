using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetProject.Services
{
    public interface IReadOnlyService<TViewModel>
    {
        /// <summary>
        /// Service lấy toàn bộ đối tượng
        /// </summary>
        /// <returns>Danh sách đối tượng</returns>
        /// Author: PNNHai
        /// Date: 
        Task<IEnumerable<TViewModel>> GetAllAsync();

        /// <summary>
        /// Service lấy thông tin đối tượng theo id
        /// </summary>
        /// <param name="id">Mã định danh của đối tượng</param>
        /// <returns>Đối tượng cần lấy</returns>
        /// Author: PNNHai
        /// Date: 
        Task<TViewModel> GetByIdAsync(Guid id);
    }
}
