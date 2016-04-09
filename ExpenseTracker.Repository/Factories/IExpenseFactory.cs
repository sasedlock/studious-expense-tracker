using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Repository.Factories
{
    public interface IExpenseFactory
    {
        DTO.Expense CreateExpense(Expense expense);
        Expense CreateExpense(DTO.Expense expense);
        IEnumerable<DTO.Expense> CreateExpenses(IEnumerable<Expense> expenses);
        IEnumerable<Expense> CreateExpenses(IEnumerable<DTO.Expense> expenses);
    }
}
