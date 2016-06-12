using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Repository.Entities;
using ExpenseTracker.Repository.Interfaces;

namespace ExpenseTracker.Repository.Repositories
{
    public class ExpenseTrackerRepository<T> : IExpenseTrackerGenericRepository<T> where T : ExpenseTrackerModel
    {
        protected readonly IExpenseTrackerDbContext _ctx;
        protected readonly IDbSet<T> _dbSet; 

        public ExpenseTrackerRepository(IExpenseTrackerDbContext ctx)
        {
            _ctx = ctx;
            _dbSet = ctx.Set<T>();
        } 

        public virtual T GetById(int id)
        {
            return _dbSet.FirstOrDefault(x => x.Id == id);
        }

        public virtual IQueryable<T> GetAllAsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public virtual RepositoryActionResult<T> Insert(T entity)
        {
            try
            {
                _dbSet.Add(entity);
                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<T>(entity, RepositoryActionStatus.Created);
                }
                return new RepositoryActionResult<T>(entity, RepositoryActionStatus.NothingModified, null);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<T>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public virtual RepositoryActionResult<T> Update(T entity)
        {
            try
            {
                var existingEntity = _dbSet.FirstOrDefault(e => e.Id == entity.Id);

                if (existingEntity == null)
                {
                    return new RepositoryActionResult<T>(entity, RepositoryActionStatus.NotFound);
                }

                _ctx.SetDetached(existingEntity);

                _dbSet.Attach(entity);

                _ctx.SetModified(entity);

                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<T>(entity, RepositoryActionStatus.Updated);
                }
                return new RepositoryActionResult<T>(entity, RepositoryActionStatus.NothingModified, null);
            }
            catch (Exception ex)
            { 
                return new RepositoryActionResult<T>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public virtual RepositoryActionResult<T> Delete(int id)
        {
            try
            {
                var entity = _dbSet.FirstOrDefault(e => e.Id == id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    _ctx.SaveChanges();
                    return new RepositoryActionResult<T>(null, RepositoryActionStatus.Deleted);
                }
                return new RepositoryActionResult<T>(null, RepositoryActionStatus.NotFound);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<T>(null, RepositoryActionStatus.Error, ex);
            }
        }
    }
}
