using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Entities;
using ExpenseTracker.Repository.Interfaces;
using Serilog;

namespace ExpenseTracker.API.Loggers
{
    public class RepositoryLogger<T> : IExpenseTrackerGenericRepository<T> where T : class 
    {
        private readonly IExpenseTrackerGenericRepository<T> _inner;

        public RepositoryLogger(IExpenseTrackerGenericRepository<T> inner)
        {
            if (inner == null)
            {
                throw new NullReferenceException(nameof(inner));
            }
            _inner = inner;
        }

        public RepositoryActionResult<T> Delete(int id)
        {
            return this._inner.Delete(id);
        }

        public T GetById(int id)
        {
            using (
                Log.Logger.BeginTimedOperation($"Getting object of type {typeof (T).Name} with id {id}"))
            {
                return this._inner.GetById(id);
            }
        }

        public IQueryable<T> GetAllAsQueryable()
        {
            using (Log.Logger.BeginTimedOperation("Getting queryable of {0}", typeof (T).Name))
            {
                return this._inner.GetAllAsQueryable();
            }
        }

        public RepositoryActionResult<T> Insert(T entity)
        {
            return this._inner.Insert(entity);
        }

        public RepositoryActionResult<T> Update(T entity)
        {
            return this._inner.Update(entity);
        }
    }
}