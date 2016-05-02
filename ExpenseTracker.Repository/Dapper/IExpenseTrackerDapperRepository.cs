using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Repository.Dapper
{
    public interface IExpenseTrackerDapperRepository
    {
        IEnumerable<ExpenseGroup> GetAllExpenseGroups();
    }
}
