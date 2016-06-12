using System.Linq;
using ExpenseTracker.Repository.Entities;
using ExpenseTracker.Repository.Interfaces;

namespace ExpenseTracker.Repository.Repositories
{
    public class ExpenseGroupStatusRepository : ExpenseTrackerRepository<ExpenseGroupStatus>, IExpenseGroupStatusRepository
    {
        public ExpenseGroupStatusRepository(IExpenseTrackerDbContext ctx) : base(ctx)
        {
        }

        public override RepositoryActionResult<ExpenseGroupStatus> Insert(ExpenseGroupStatus entity)
        {
            throw new System.NotImplementedException();
        }

        public override RepositoryActionResult<ExpenseGroupStatus> Update(ExpenseGroupStatus entity)
        {
            throw new System.NotImplementedException();
        }

        public override RepositoryActionResult<ExpenseGroupStatus> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}