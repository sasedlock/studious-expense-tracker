using System.Linq;
using ExpenseTracker.Repository.Entities;
using ExpenseTracker.Repository.Interfaces;

namespace ExpenseTracker.Repository.Repositories
{
    public class ExpenseGroupStatusRepository : IExpenseGroupStatusRepository
    {
        private readonly IExpenseTrackerDbContext _ctx;

        public ExpenseGroupStatusRepository(IExpenseTrackerDbContext ctx)
        {
            _ctx = ctx;
        }

        public ExpenseGroupStatus GetById(int id)
        {
            return _ctx.ExpenseGroupStatusses.FirstOrDefault(egs => egs.Id == id);
        }

        public IQueryable<ExpenseGroupStatus> GetAllAsQueryable()
        {
            return _ctx.ExpenseGroupStatusses.AsQueryable();
        }

        public RepositoryActionResult<ExpenseGroupStatus> Insert(ExpenseGroupStatus entity)
        {
            throw new System.NotImplementedException();
        }

        public RepositoryActionResult<ExpenseGroupStatus> Update(ExpenseGroupStatus entity)
        {
            throw new System.NotImplementedException();
        }

        public RepositoryActionResult<ExpenseGroupStatus> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}