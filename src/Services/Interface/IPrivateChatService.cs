using System;
using System.Threading.Tasks;

namespace PetProject.Services
{
    public interface IPrivateChatService
    {
        Task<Result> CreateAsync(PrivateMessageCreateDto privateMessageCreate);
        Task<PrivateChatViewModel?> GetPrivateChatByUserIdAndPartnerId(Guid userId, Guid partnerId);
    }
}
