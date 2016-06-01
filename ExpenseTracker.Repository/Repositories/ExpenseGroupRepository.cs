using System;
using System.Data.Entity;
using System.Linq;
using ExpenseTracker.Repository.Entities;
using ExpenseTracker.Repository.Interfaces;

namespace ExpenseTracker.Repository.Repositories
{
    public class ExpenseGroupRepository : IExpenseGroupRepository
    {
        private IExpenseTrackerDbContext _ctx;

        public ExpenseGroupRepository(IExpenseTrackerDbContext ctx)
        {
            _ctx = ctx;
        }

        public ExpenseGroup GetById(int id)
        {
            return _ctx.ExpenseGroups.FirstOrDefault(eg => eg.Id == id);
        }

        public IQueryable<ExpenseGroup> GetAllAsQueryable()
        {
            return _ctx.ExpenseGroups.AsQueryable();
        }

        public RepositoryActionResult<ExpenseGroup> Insert(ExpenseGroup eg)
        {
            try
            {
                _ctx.ExpenseGroups.Add(eg);
                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.Created);
                }
                else
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.NothingModified, null);
                }
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public RepositoryActionResult<ExpenseGroup> Update(ExpenseGroup eg)
        {
            try
            {

                // you can only update when an expensegroup already exists for this id

                var existingEG = _ctx.ExpenseGroups.FirstOrDefault(exg => exg.Id == eg.Id);

                if (existingEG == null)
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.NotFound);
                }

                // change the original entity status to detached; otherwise, we get an error on attach
                // as the entity is already in the dbSet

                // set original entity state to detached
                _ctx.Entry(existingEG).State = EntityState.Detached;

                // attach & save
                _ctx.ExpenseGroups.Attach(eg);

                // set the updated entity state to modified, so it gets updated.
                _ctx.Entry(eg).State = EntityState.Modified;


                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.Updated);
                }
                else
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.NothingModified, null);
                }

            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public RepositoryActionResult<ExpenseGroup> Delete(int id)
        {
            try
            {

                var eg = _ctx.ExpenseGroups.Where(e => e.Id == id).FirstOrDefault();
                if (eg != null)
                {
                    // also remove all expenses linked to this expensegroup

                    _ctx.ExpenseGroups.Remove(eg);

                    _ctx.SaveChanges();
                    return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.Deleted);
                }
                return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.NotFound);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public ExpenseGroup GetExpenseGroupByIdAndUserId(int id, string userId)
        {
            return _ctx.ExpenseGroups.FirstOrDefault(eg => eg.Id == id && eg.UserId == userId);
        }

        public ExpenseGroup GetExpenseGroupWithExpenses(int id, string userId)
        {
            return _ctx.ExpenseGroups.Include("Expenses").FirstOrDefault(eg => eg.Id == id && eg.UserId == userId);
        }

        public ExpenseGroup GetExpenseGroupWithExpenses(int id)
        {
            return _ctx.ExpenseGroups.Include("Expenses").FirstOrDefault(eg => eg.Id == id);
        }

        public IQueryable<ExpenseGroup> GetExpenseGroupsWithExpenses()
        {
            return _ctx.ExpenseGroups.Include("Expenses");
        }

        public IQueryable<ExpenseGroup> GetExpenseGroupsByUserId(string userId)
        {
            return _ctx.ExpenseGroups.Where(eg => eg.UserId == userId);
        }
    }
}