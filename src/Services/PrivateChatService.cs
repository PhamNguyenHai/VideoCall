using AutoMapper;
using PetProject.Repositories;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace PetProject.Services
{
    public class PrivateChatService : IPrivateChatService
    {
        private readonly IMapper _mapper;
        private readonly IPrivateChatRepository _privateChatRepository;

        public PrivateChatService(IMapper mapper, IPrivateChatRepository privateChatRepository)
        {
            _mapper = mapper;
            _privateChatRepository = privateChatRepository;
        }

        public async Task<Result> CreateAsync(PrivateMessageCreateDto privateMessageCreate)
        {
            var entity = _mapper.Map<PrivateChat>(privateMessageCreate);
            entity.SetKey(Guid.NewGuid());
            if (entity is BaseAuditEntity entityAuditEntity)
            {
                entityAuditEntity.CreatedDate = DateTime.UtcNow;
                entityAuditEntity.ModifiedDate = DateTime.UtcNow;
            }

            // insert vào DB
            int effectedRow = await _privateChatRepository.InsertAsync(entity);
            if (effectedRow > 0)
            {
                return new Result
                {
                    Success = true,
                    Message = "Thêm dữ liệu thành công",
                    Data = privateMessageCreate
                };
            }
            else
            {
                return new Result
                {
                    Success = false,
                    Message = "Thêm dữ liệu không thành công",
                    Data = privateMessageCreate
                };
            }
        }

        public async Task<PrivateChatViewModel> GetPrivateChatByUserIdAndPartnerId(Guid userId, Guid partnerId)
        {
            var privateChat = await _privateChatRepository.GetPrivateChatByUserIdAndPartnerId(userId, partnerId);
            var result = _mapper.Map<PrivateChatViewModel>(privateChat);
            return result;
        }
    }
}
