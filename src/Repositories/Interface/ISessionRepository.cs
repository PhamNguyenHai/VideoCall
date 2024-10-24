using System;
using System.Threading.Tasks;

namespace PetProject.Repositories
{
    public interface ISessionRepository
    {
        Task<int> CreateSession(Session session);
        Task<int> DeleteSession(Guid sessionId);
        Task<Session?> GetSessionByToken(string token);
    }
}
