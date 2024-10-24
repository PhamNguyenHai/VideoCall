using PetProject.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using AutoMapper;
using System.Reflection;

namespace PetProject.Services
{
    public abstract class BaseService<TEntity, TModel, TViewModel, TEntityCreateDto, TEntityUpdateDto>
        : ReadOnlyService<TEntity, TModel, TViewModel>,
        IBaseService<TViewModel, TEntityCreateDto, TEntityUpdateDto>
        where TEntity : IHasKey
    {
        #region Fields
        protected readonly IBaseRepository<TEntity, TModel> _baseRepository;
        #endregion

        #region Constructor
        protected BaseService(IBaseRepository<TEntity, TModel> baseRepository, IMapper mapper) : base(baseRepository, mapper)
        {
            _baseRepository = baseRepository;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Service thực hiện thêm mới đối tượng
        /// </summary>
        /// <param name="entityCreateDto">Đối tượng cần thêm mới</param>
        /// <returns></returns>
        /// Author: PNNHai
        /// Date: 
        public async Task<Result> CreateAsync(TEntityCreateDto entityCreateDto)
        {
            // Validate nghiệp vụ trước khi thêm mới
            var validationResult = await ValidateForInserting(entityCreateDto);
            if (!validationResult.Success)
            {
                return validationResult; // Trả về kết quả xác thực không thành công ngay lập tức
            }

            var entity = _mapper.Map<TEntity>(entityCreateDto);
            entity.SetKey(Guid.NewGuid());
            if (entity is BaseAuditEntity entityAuditEntity)
            {
                entityAuditEntity.CreatedDate = DateTime.UtcNow;
                entityAuditEntity.ModifiedDate = DateTime.UtcNow;
            }

            // insert vào DB
            int effectedRow = await _baseRepository.InsertAsync(entity);
            if(effectedRow > 0)
            {
                return new Result
                {
                    Success = true,
                    Message = "Thêm dữ liệu thành công",
                    Data = entityCreateDto
                };
            }
            else
            {
                return new Result
                {
                    Success = false,
                    Message = "Thêm dữ liệu không thành công",
                    Data = entityCreateDto
                };
            }
        }

        /// <summary>
        /// Service cập nhật đối tượng
        /// </summary>
        /// <param name="id">Id của đối tượng cần cập nhật</param>
        /// <param name="entityUpdateDto">Thông tin đối tượng cần cập nhật</param>
        /// <returns></returns>
        /// Author: PNNHai
        /// Date: 
        public async Task<Result> UpdateAsync(Guid id, TEntityUpdateDto entityUpdateDto)
        {
            // Kiểm tra id truyền vào có chuẩn không. Nếu ko thì báo lỗi
            await _baseRepository.GetByIdAsync(id);

            // Sau khi id hợp lệ thì validate nghiệp vụ
            // Validate nghiệp vụ trước khi update
            var validationResult = await ValidateForUpdating(id, entityUpdateDto);
            if (!validationResult.Success)
            {
                return validationResult; // Trả về kết quả xác thực không thành công ngay lập tức
            }

            var entity = _mapper.Map<TEntity>(entityUpdateDto);
            entity.SetKey(id);
            if (entity is BaseAuditEntity entityAuditEntity)
            {
                entityAuditEntity.ModifiedDate = DateTime.UtcNow;
            }

            // update vào DB
            int effectedRow = await _baseRepository.UpdateAsync(entity);

            if (effectedRow > 0)
            {
                return new Result
                {
                    Success = true,
                    Message = "Cập nhật dữ liệu thành công",
                    Data = entityUpdateDto
                };
            }
            else
            {
                return new Result
                {
                    Success = false,
                    Message = "Cập nhật dữ liệu không thành công",
                    Data = entityUpdateDto
                };
            }
        }

        /// <summary>
        /// Service xóa đối tượng
        /// </summary>
        /// <param name="id">Id của đối tượng cần xóa</param>
        /// <returns></returns>
        /// Author: PNNHai
        /// Date: 
        public async Task<Result> DeleteAsync(Guid id)
        {
            // Lấy entity theo id
            var model = await _baseRepository.FindByIdAsync(id);

            if(model == null)
            {
                return new Result
                {
                    Success = false,
                    Message = "Không tìm thấy dữ liệu để xóa theo định danh",
                    Data = null
                };
            }

            var entity = _mapper.Map<TEntity>(model);
            // Xóa 
            int effectedRow = await _baseRepository.DeleteAsync(entity);
            if (effectedRow > 0)
            {
                return new Result
                {
                    Success = true,
                    Message = "Xóa dữ liệu thành công",
                    Data = id
                };
            }
            else
            {
                return new Result
                {
                    Success = false,
                    Message = "Xóa dữ liệu không thành công",
                    Data = id
                };
            }
        }

        /// <summary>
        /// Yêu cầu ghi đè lại ở class khởi tạo đối tượng
        /// Yêu cầu ghi đè lại tùy nghiệp vụ ở từng service
        /// </summary>
        /// <param name="entityCreateDto">Thông tin phần tử cần thêm</param>
        /// <returns></returns>
        /// author: PNNHai
        /// date: 
        public abstract Task<Result> ValidateForInserting(TEntityCreateDto entityCreateDto);

        /// <summary>
        /// Yêu cầu ghi đè lại ở class khởi tạo đối tượng
        /// Validate nghiệp vụ để update tùy service
        /// </summary>
        /// <param name="id">Mã định danh của phần tử cần update</param>
        /// <param name="entityUpdateDto">Thông tin update</param>
        /// <returns></returns>
        /// author: PNNHai
        /// date: 
        public abstract Task<Result> ValidateForUpdating(Guid id, TEntityUpdateDto entityUpdateDto);
        #endregion
    }
}
