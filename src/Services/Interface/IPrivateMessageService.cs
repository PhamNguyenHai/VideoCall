using System;
using System.Threading.Tasks;

namespace PetProject.Services
{
    public interface IPrivateMessageService
    {
        Task<Result> CreateAsync(Guid chatId, PrivateMessageCreateDto privateMessageCreate);
        Task<Result> CreateChatMessageAndSendMessageAsync(PrivateMessageCreateDto privateMessageCreate);
    }
}
