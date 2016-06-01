using System;
using System.Linq;
using ExpenseTracker.Repository.Entities;
using ExpenseTracker.Repository.Interfaces;

namespace ExpenseTracker.Repository.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly IExpenseTrackerDbContext _ctx;

        public ExpenseRepository(IExpenseTrackerDbContext ctx)
        {
            _ctx = ctx;
        }

        public Expense GetById(int id)
        {
            return _ctx.Expenses.FirstOrDefault(e => e.Id == id);
        }

        public IQueryable<Expense> GetAllAsQueryable()
        {
            return _ctx.Expenses.AsQueryable();
        }

        public RepositoryActionResult<Expense> Insert(Expense e)
        {
            try
            {
                _ctx.Expenses.Add(e);
                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.Created);
                }
                else
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.NothingModified, null);
                }

            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public RepositoryActionResult<Expense> Update(Expense e)
        {
            try
            {

                // you can only update when an expense already exists for this id

                var existingExpense = _ctx.Expenses.FirstOrDefault(exp => exp.Id == e.Id);

                if (existingExpense == null)
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.NotFound);
                }

                // change the original entity status to detached; otherwise, we get an error on attach
                // as the entity is already in the dbSet

                // set original entity state to detached
                //_ctx.Entry(existingExpense).State = EntityState.Detached;
                _ctx.SetDetached(existingExpense);

                // attach & save
                _ctx.Expenses.Attach(e);

                // set the updated entity state to modified, so it gets updated.
                //_ctx.Entry(e).State = EntityState.Modified;
                _ctx.SetModified(e);


                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.Updated);
                }
                else
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.NothingModified, null);
                }
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public RepositoryActionResult<Expense> Delete(int id)
        {
            try
            {
                var exp = _ctx.Expenses.Where(e => e.Id == id).FirstOrDefault();
                if (exp != null)
                {
                    _ctx.Expenses.Remove(exp);
                    _ctx.SaveChanges();
                    return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.Deleted);
                }
                return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.NotFound);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public Expense GetExpenseByIdAndExpenseGroup(int id, int? expenseGroupId)
        {
            if (expenseGroupId == null)
            {
                return GetById(id);
            }

            return _ctx.Expenses.FirstOrDefault(e => e.Id == id && e.ExpenseGroupId == expenseGroupId.Value);
        }

        public IQueryable<Expense> GetExpensesByExpenseGroupId(int expenseGroupId)
        {
            var expenseGroup = _ctx.ExpenseGroups.FirstOrDefault(eg => eg.Id == expenseGroupId);
            if (expenseGroup != null)
            {
                return _ctx.Expenses.Where(e => e.ExpenseGroupId == expenseGroupId);
            }
            return null;
        }
    }
}