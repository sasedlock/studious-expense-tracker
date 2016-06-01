using System.Linq;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Repository.Interfaces
{
    public interface IExpenseRepository : IExpenseTrackerGenericRepository<Expense>
    {
        Expense GetExpenseByIdAndExpenseGroup(int id, int? expenseGroupId);
        IQueryable<Expense> GetExpensesByExpenseGroupId(int expenseGroupId);
    }
}