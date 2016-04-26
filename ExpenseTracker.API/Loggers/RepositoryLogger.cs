using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.API.Loggers
{
    public class RepositoryLogger : IExpenseTrackerRepository
    {
        private readonly IExpenseTrackerRepository _inner;

        public RepositoryLogger(IExpenseTrackerRepository inner)
        {
            if (inner == null)
            {
                throw new NullReferenceException(nameof(inner));
            }
            _inner = inner;
        }

        public RepositoryActionResult<Expense> DeleteExpense(int id)
        {
            return this._inner.DeleteExpense(id);
        }

        public RepositoryActionResult<ExpenseGroup> DeleteExpenseGroup(int id)
        {
            return this._inner.DeleteExpenseGroup(id);
        }

        public Expense GetExpense(int id, int? expenseGroupId = null)
        {
            return this._inner.GetExpense(id, expenseGroupId);
        }

        public ExpenseGroup GetExpenseGroup(int id)
        {
            return this._inner.GetExpenseGroup(id);
        }

        public ExpenseGroup GetExpenseGroup(int id, string userId)
        {
            return this._inner.GetExpenseGroup(id, userId);
        }

        public IQueryable<ExpenseGroup> GetExpenseGroups()
        {
            return this._inner.GetExpenseGroups();
        }

        public IQueryable<ExpenseGroup> GetExpenseGroups(string userId)
        {
            return this._inner.GetExpenseGroups(userId);
        }

        public ExpenseGroupStatus GetExpenseGroupStatus(int id)
        {
            return this._inner.GetExpenseGroupStatus(id);
        }

        public IQueryable<ExpenseGroupStatus> GetExpenseGroupStatusses()
        {
            return this._inner.GetExpenseGroupStatusses();
        }

        public IQueryable<ExpenseGroup> GetExpenseGroupsWithExpenses()
        {
            return this._inner.GetExpenseGroupsWithExpenses();
        }

        public ExpenseGroup GetExpenseGroupWithExpenses(int id)
        {
            return this._inner.GetExpenseGroupWithExpenses(id);
        }

        public ExpenseGroup GetExpenseGroupWithExpenses(int id, string userId)
        {
            return this._inner.GetExpenseGroupWithExpenses(id, userId);
        }

        public IQueryable<Expense> GetExpenses()
        {
            return this._inner.GetExpenses();
        }

        public IQueryable<Expense> GetExpenses(int expenseGroupId)
        {
            return this._inner.GetExpenses(expenseGroupId);
        }

        public RepositoryActionResult<Expense> InsertExpense(Expense e)
        {
            return this._inner.InsertExpense(e);
        }

        public RepositoryActionResult<ExpenseGroup> InsertExpenseGroup(ExpenseGroup eg)
        {
            return this._inner.InsertExpenseGroup(eg);
        }

        public RepositoryActionResult<Expense> UpdateExpense(Expense e)
        {
            return this._inner.UpdateExpense(e);
        }

        public RepositoryActionResult<ExpenseGroup> UpdateExpenseGroup(ExpenseGroup eg)
        {
            return this._inner.UpdateExpenseGroup(eg);
        }
    }
}