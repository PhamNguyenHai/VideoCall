using System.Data.Common;
using System.Threading.Tasks;
using System;

namespace PetProject.UnitOfWork
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        DbConnection Connection { get; }
        DbTransaction? Transaction { get; } // Không phải cái nào cũng mở transaction
        void BeginTransaction();
        Task BeginTransactionAsync();
        void Commit();
        Task CommitAsync();
        void Rollback();
        Task RollbackAsync();
    }
}
