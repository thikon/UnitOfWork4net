using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitOfWork.Common
{
    public interface IUnitOfWork : IDisposable
    {
        int Commit();
        UnitOfWork BeginTransaction();
        UnitOfWork DoInsert<TEntity>(TEntity entity) where TEntity : class;
        UnitOfWork DoInsert<TEntity>(TEntity entity, out TEntity inserted) where TEntity : class;
        UnitOfWork DoUpdate<TEntity>(TEntity entity) where TEntity : class;
        UnitOfWork DoUpdate<TEntity>(TEntity entity, int key) where TEntity : class;
        void Dispose();
        bool EndTransaction();
        UnitOfWork SaveAndContinue();
    }
}
