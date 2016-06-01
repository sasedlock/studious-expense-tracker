using System.Linq;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Repository.Interfaces
{
    public interface IExpenseGroupRepository : IExpenseTrackerGenericRepository<ExpenseGroup>
    {
        ExpenseGroup GetExpenseGroupByIdAndUserId(int id, string userId);
        ExpenseGroup GetExpenseGroupWithExpenses(int id, string userId);
        ExpenseGroup GetExpenseGroupWithExpenses(int id);
        IQueryable<ExpenseGroup> GetExpenseGroupsWithExpenses();
        IQueryable<ExpenseGroup> GetExpenseGroupsByUserId(string userId);
    }
}