using PetProject.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using AutoMapper;

namespace PetProject.Services
{
    public abstract class ReadOnlyService<TEntity, TModel, TViewModel> : IReadOnlyService<TViewModel> where TEntity : IHasKey
    {
        #region Fields
        protected readonly IReadOnlyRepository<TEntity, TModel> _readOnlyRepository;
        protected readonly IMapper _mapper;
        #endregion

        #region Constructor
        protected ReadOnlyService(IReadOnlyRepository<TEntity, TModel> readOnlyRepository, IMapper mapper)
        {
            _readOnlyRepository = readOnlyRepository;
            _mapper = mapper;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Service lấy toàn bộ danh sách đối tượng
        /// </summary>
        /// <returns>Danh sách đối tượng </returns>
        /// Author: PNNHai
        /// Date: 
        public async Task<IEnumerable<TViewModel>> GetAllAsync()
        {
            var entities = await _readOnlyRepository.GetAllAsync();
            var entityDtos = _mapper.Map<IEnumerable<TViewModel>>(entities);
            return entityDtos;
        }

        /// <summary>
        /// Service lấy đối tượng theo id
        /// </summary>
        /// <param name="id">Mã định danh đối tượng</param>
        /// <returns>Đối tượng cần lấy</returns>
        /// Author: PNNHai
        /// Date: 
        public async Task<TViewModel?> GetByIdAsync(Guid id)
        {
            var entity = await _readOnlyRepository.FindByIdAsync(id);
            var entityDto = _mapper.Map<TViewModel>(entity);
            return entityDto;
        }
        #endregion
    }
}
