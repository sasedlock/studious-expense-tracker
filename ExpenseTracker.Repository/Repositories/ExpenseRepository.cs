using System;
using System.Linq;
using ExpenseTracker.Repository.Entities;
using ExpenseTracker.Repository.Interfaces;

namespace ExpenseTracker.Repository.Repositories
{
    public class ExpenseRepository : ExpenseTrackerRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(IExpenseTrackerDbContext ctx) : base(ctx)
        {
        }

        public Expense GetExpenseByIdAndExpenseGroup(int id, int? expenseGroupId)
        {
            if (expenseGroupId == null)
            {
                return GetById(id);
            }

            return _dbSet.FirstOrDefault(e => e.Id == id && e.ExpenseGroupId == expenseGroupId.Value);
        }

        public IQueryable<Expense> GetExpensesByExpenseGroupId(int expenseGroupId)
        {
            var expenseGroup = _ctx.ExpenseGroups.FirstOrDefault(eg => eg.Id == expenseGroupId);
            if (expenseGroup != null)
            {
                return _dbSet.Where(e => e.ExpenseGroupId == expenseGroupId);
            }
            return null;
        }
    }
}