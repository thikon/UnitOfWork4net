using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitOfWork.Common
{ 

    public class UnitOfWork : IUnitOfWork
    {
        private DbContext _dbContext;

        private DbContextTransaction Transaction { get; set; }

        public UnitOfWork(DbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public UnitOfWork BeginTransaction()
        {
            Transaction = _dbContext.Database.BeginTransaction();
            return this;
        }

        public int Commit()
        {
            return _dbContext.SaveChanges();
        }

        public UnitOfWork DoInsert<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Set<TEntity>().Add(entity);
            return this;
        }

        public UnitOfWork DoInsert<TEntity>(TEntity entity, out TEntity inserted) where TEntity : class
        {
            inserted = _dbContext.Set<TEntity>().Add(entity);
            return this;
        }

        public UnitOfWork DoUpdate<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            return this;
        }

        public UnitOfWork DoUpdate<TEntity>(TEntity entity, int key) where TEntity : class
        {
            if (entity == null) return null;

            TEntity existing = _dbContext.Set<TEntity>().Find(key);
            if (existing != null)
            {
                _dbContext.Entry(existing).CurrentValues.SetValues(entity);
                _dbContext.Entry(existing).State = EntityState.Modified;
            }

            return this;
        }


        public UnitOfWork SaveAndContinue()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                // add your exception handling code here
                Rollback();
                Dispose();
            }
            return this;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool EndTransaction()
        {
            try
            {
                _dbContext.SaveChanges();
                Transaction.Commit();
            }
            catch (DbEntityValidationException dbEx)
            {
                // add your exception handling code here
                Rollback();
                Dispose();
                return false;
            }
            return true;
        }

        public void Rollback()
        {
            Transaction.Rollback();
            Dispose();
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dbContext != null)
                {
                    Transaction?.Dispose();
                    _dbContext?.Dispose();
                }
            }
        }
    }

}
