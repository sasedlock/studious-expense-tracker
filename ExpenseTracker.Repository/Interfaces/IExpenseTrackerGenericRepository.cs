using System.Linq;

namespace ExpenseTracker.Repository.Interfaces
{
    public interface IExpenseTrackerGenericRepository<out T>
    {
        T GetById(int id);
        IQueryable<T> GetAllAsQueryable();
    }
}