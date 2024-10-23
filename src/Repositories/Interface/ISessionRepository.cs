using System.Threading.Tasks;

namespace PetProject.Repositories
{
    public interface ISessionRepository
    {
        Task<int> CreateSession(Session session);
        Task<Session?> GetSessionByToken(string token);
    }
}
