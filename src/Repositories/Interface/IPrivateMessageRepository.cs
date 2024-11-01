using System.Threading.Tasks;

namespace PetProject.Repositories
{
    public interface IPrivateMessageRepository
    {
        Task<int> InsertAsync(PrivateMessage privateChat);
    }
}