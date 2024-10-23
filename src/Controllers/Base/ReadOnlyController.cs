using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetProject.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PetProject.Controllers
{
    public class ReadOnlyController<TViewModel> : Controller
    {
        #region Fields
        private readonly IReadOnlyService<TViewModel> _readOnlyService;
        #endregion

        #region Constructor
        public ReadOnlyController(IReadOnlyService<TViewModel> readOnlyService)
        {
            _readOnlyService = readOnlyService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Api lấy danh sách tất cả đối tượng
        /// </summary>
        /// <returns>Danh sách đối tượng</returns>
        /// Author: PNNHai
        /// Date: 
        [HttpGet]
        public virtual async Task<IEnumerable<TViewModel>> GetAll()
        {
            var result = await _readOnlyService.GetAllAsync();
            return result;
        }

        /// <summary>
        /// Api lấy đối tượng theo id
        /// </summary>
        /// <param name="id">Mã định danh đối tượng</param>
        /// <returns>Đối tượng cần lấy</returns>
        /// Author: PNNHai
        /// Date: 
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(Guid id)
        {
            var result = await _readOnlyService.GetByIdAsync(id);
            return StatusCode(StatusCodes.Status200OK, result);
        }
        #endregion
    }
}
