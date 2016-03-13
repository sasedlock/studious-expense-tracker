using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Repository.Factories
{
    public interface IExpenseGroupFactory
    {
        ExpenseGroup CreateExpenseGroup(DTO.ExpenseGroup expenseGroup);
        DTO.ExpenseGroup CreateExpenseGroup(ExpenseGroup expenseGroup);
        IEnumerable<ExpenseGroup> CreateExpenseGroups(IEnumerable<DTO.ExpenseGroup> expenseGroups);
        IEnumerable<DTO.ExpenseGroup> CreateExpenseGroups(IEnumerable<ExpenseGroup> expenseGroups);
    }
}
