using System.Threading.Tasks;

namespace PetProject.Repositories
{
    public interface IPrivateChatRepository
    {
        Task<int> InsertAsync(PrivateChat privateChat);
    }
}