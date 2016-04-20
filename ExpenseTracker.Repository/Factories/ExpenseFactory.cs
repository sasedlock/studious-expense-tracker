using ExpenseTracker.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository.Factories
{
    public class ExpenseFactory
    {

        public ExpenseFactory()
        {

        }

        public DTO.Expense CreateExpense(Expense expense)
        {
            return new DTO.Expense()
            {
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description,
                ExpenseGroupId = expense.ExpenseGroupId,
                Id = expense.Id
            };
        }



        public Expense CreateExpense(DTO.Expense expense)
        {
            return new Expense()
            {
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description,
                ExpenseGroupId = expense.ExpenseGroupId,
                Id = expense.Id
            };
        }

        public IEnumerable<Expense> CreateExpenses(IEnumerable<DTO.Expense> expenses)
        {
            return expenses.Select(CreateExpense).ToList();
        }

        public IEnumerable<DTO.Expense> CreateExpenses(IEnumerable<Expense> expenses)
        {
            return expenses.Select(CreateExpense).ToList();
        }

        public object CreateDataShapedObject(Expense expense, List<string> fieldList)
        {
            return CreateDataShapedObject(CreateExpense(expense), fieldList);
        }

        public object CreateDataShapedObject(DTO.Expense expense, List<string> fieldList)
        {
            if (!fieldList.Any())
            {
                return expense;
            }
            else
            {
                ExpandoObject objectToReturn = new ExpandoObject();

                foreach (var field in fieldList)
                {
                    var fieldValue =
                        expense.GetType()
                            .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                            .GetValue(expense, null);

                    ((IDictionary<string, object>) objectToReturn).Add(field, fieldValue);
                }

                return objectToReturn;
            }
        }
    }
}
