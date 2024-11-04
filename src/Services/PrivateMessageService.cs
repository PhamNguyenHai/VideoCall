using AutoMapper;
using PetProject.Models;
using PetProject.Repositories;
using System;
using System.Threading.Tasks;

namespace PetProject.Services
{
    public class PrivateMessageService : IPrivateMessageService
    {
        protected readonly IMapper _mapper;
        protected readonly IPrivateMessageRepository _privateMessageRepository;
        protected readonly IPrivateChatRepository _privateChatRepository;
        private readonly IEncryptionHelper _encryptionHelper;
        protected readonly IUnitOfWork _unitOfWork;

        public PrivateMessageService(IMapper mapper, IPrivateMessageRepository privateMessageRepository, IEncryptionHelper encryptionHelper, IPrivateChatRepository privateChatRepository, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _privateMessageRepository = privateMessageRepository;
            _encryptionHelper = encryptionHelper;
            _privateChatRepository = privateChatRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> CreateAsync(Guid chatId, PrivateMessageCreateDto privateMessageCreate)
        {
            var privateMessage = _mapper.Map<PrivateMessage>(privateMessageCreate);
            privateMessage.ChatId = chatId;
            privateMessage.MessageId = Guid.NewGuid();
            privateMessage.TimeStamp = DateTime.UtcNow;
            privateMessage.Content = _encryptionHelper.Encrypt(privateMessage.Content);

            // insert vào DB
            int effectedRow = await _privateMessageRepository.InsertAsync(privateMessage);
            if (effectedRow > 0)
            {
                return new Result
                {
                    Success = true,
                    Message = "Thêm dữ liệu thành công",
                    Data = privateMessage
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

        public async Task<Result> CreateChatMessageAndSendMessageAsync(PrivateMessageCreateDto privateMessageCreate)
        {
            var privateChatToCreate = new PrivateChat()
            {
                ChatId = Guid.NewGuid(),
                UserId = privateMessageCreate.SenderId,
                PartnerId = privateMessageCreate.ReceiverId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
            };
            try
            {
                // Tiến hành lưu thông tin vào db
                _unitOfWork.BeginTransaction();
                var result = new Result()
                {
                    Success = false,
                    Message = "Thêm dữ liệu không thành công",
                    Data = privateMessageCreate
                };
                var effectedRows = await _privateChatRepository.InsertAsync(privateChatToCreate);
                if (effectedRows > 0)
                {
                    var privateMessageToCreate = _mapper.Map<PrivateMessage>(privateMessageCreate);
                    privateMessageToCreate.ChatId = privateChatToCreate.ChatId;
                    privateMessageToCreate.MessageId = Guid.NewGuid();
                    privateMessageToCreate.TimeStamp = DateTime.UtcNow;
                    privateMessageToCreate.Content = _encryptionHelper.Encrypt(privateMessageToCreate.Content);

                    var createMessageEffectedRows = await _privateMessageRepository.InsertAsync(privateMessageToCreate);

                    if(createMessageEffectedRows > 0)
                    {
                        result.Success = true;
                        result.Message = "Thêm dữ liệu thành công";
                        result.Data = privateMessageToCreate;
                    }
                }

                // Commit
                _unitOfWork.Commit();
                return result;
            }
            catch
            {
                _unitOfWork.Rollback();
                var result = new Result
                {
                    Success = false,
                    Message = "Thêm dữ liệu không thành công",
                    Data = privateMessageCreate
                };
                return result;
                throw;
            }
        }
    }
}
