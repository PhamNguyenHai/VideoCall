using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetProject.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PetProject.Controllers
{
    public class BaseController<TViewModel, TEntityCreateDto, TEntityUpdateDto>
        : ReadOnlyController<TViewModel>
    {
        #region Fields
        private readonly IBaseService<TViewModel, TEntityCreateDto, TEntityUpdateDto> _baseService;
        #endregion

        #region Constructor
        public BaseController(IBaseService<TViewModel, TEntityCreateDto, TEntityUpdateDto> baseService)
            : base(baseService)
        {
            _baseService = baseService;
        }
        #endregion

        //#region Methods
        ///// <summary>
        ///// Api thêm mới dữ liệu
        ///// </summary>
        ///// <param name="entityCreateDto">Dữ liệu muốn thêm</param>
        ///// <returns>StatusCode + Message</returns>
        ///// Author: PNNHai
        ///// Date: 
        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] TEntityCreateDto entityCreateDto)
        //{
        //    await _baseService.CreateAsync(entityCreateDto);
        //    return StatusCode(StatusCodes.Status201Created, "Thêm thành công");
        //}

        ///// <summary>
        ///// Api update dữ liệu
        ///// </summary>
        ///// <param name="id">Id của đối tượng muốn update</param>
        ///// <param name="entityUpdateDto">Dữ liệu muốn update</param>
        ///// <returnsStatusCode + Message></returns>
        ///// Author: PNNHai
        ///// Date: 
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Put(Guid id, [FromBody] TEntityUpdateDto entityUpdateDto)
        //{
        //    await _baseService.UpdateAsync(id, entityUpdateDto);
        //    return StatusCode(StatusCodes.Status200OK, APISuccessNotify.UPDATE_SUCCESSFULLY);
        //}

        ///// <summary>
        ///// Api xóa đối tượng
        ///// </summary>
        ///// <param name="id">Id của đối tượng cần xóa</param>
        ///// <returns>StatusCode + Message</returns>
        ///// Author: PNNHai
        ///// Date: 
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    await _baseService.DeleteAsync(id);
        //    return StatusCode(StatusCodes.Status200OK, APISuccessNotify.DELETE_SUCCESSFULLY);
        //}

        ///// <summary>
        ///// Api xóa nhiều đối tượng
        ///// </summary>
        ///// <param name="ids">Danh sách đối tượng muốn xóa</param>
        ///// <returns>StatusCode + Message</returns>
        ///// Author: PNNHai
        ///// Date: 
        //[HttpDelete]
        //public async Task<IActionResult> DeleteMultipal([FromBody] List<Guid> ids)
        //{
        //    await _baseService.DeleteMultipalAsync(ids);
        //    return StatusCode(StatusCodes.Status200OK, APISuccessNotify.DELETE_SUCCESSFULLY);
        //}
        //#endregion
    }
}
