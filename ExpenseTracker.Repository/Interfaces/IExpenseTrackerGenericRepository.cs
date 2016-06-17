using System.Linq;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Repository.Interfaces
{
    public interface IExpenseTrackerGenericRepository<T> where T : ExpenseTrackerModel
    {
        T GetById(int id);
        IQueryable<T> GetAllAsQueryable();
        RepositoryActionResult<T> Insert(T entity);
        RepositoryActionResult<T> Update(T entity);
        RepositoryActionResult<T> Delete(int id);
    }
}