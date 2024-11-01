using AutoMapper;
using PetProject.Repositories;
using System;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace PetProject.Services
{
    public class PrivateMessageService : IPrivateMessageService
    {
        protected readonly IMapper _mapper;
        protected readonly IPrivateMessageRepository _privateMessageRepository;

        public PrivateMessageService(IMapper mapper, IPrivateMessageRepository privateMessageRepository)
        {
            _mapper = mapper;
            _privateMessageRepository = privateMessageRepository;
        }

        public async Task<Result> CreateAsync(PrivateMessageCreateDto privateMessageCreate)
        {
            var privateMessage = _mapper.Map<PrivateMessage>(privateMessageCreate);
            privateMessage.SetKey(Guid.NewGuid());

            // insert vào DB
            int effectedRow = await _privateMessageRepository.InsertAsync(privateMessage);
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
    }
}
