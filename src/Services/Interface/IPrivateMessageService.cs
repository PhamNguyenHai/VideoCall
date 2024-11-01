using System.Threading.Tasks;

namespace PetProject.Services
{
    public interface IPrivateMessageService
    {
        Task<Result> CreateAsync(PrivateMessageCreateDto privateMessageCreate);

    }
}
