using System;
using System.Data.Entity;
using System.Linq;
using ExpenseTracker.Repository.Entities;
using ExpenseTracker.Repository.Interfaces;

namespace ExpenseTracker.Repository.Repositories
{
    public class ExpenseGroupRepository : ExpenseTrackerRepository<ExpenseGroup>, IExpenseGroupRepository
    {
        public ExpenseGroupRepository(IExpenseTrackerDbContext ctx) : base(ctx)
        {
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