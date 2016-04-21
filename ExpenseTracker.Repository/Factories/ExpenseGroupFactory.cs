using ExpenseTracker.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Repository.Helpers;

namespace ExpenseTracker.Repository.Factories
{
    public class ExpenseGroupFactory : IExpenseGroupFactory
    {
        ExpenseFactory expenseFactory = new ExpenseFactory();

        public ExpenseGroupFactory()
        {

        }

        public ExpenseGroup CreateExpenseGroup(DTO.ExpenseGroup expenseGroup)
        {
            return new ExpenseGroup()
            {
                Description = expenseGroup.Description,
                ExpenseGroupStatusId = expenseGroup.ExpenseGroupStatusId,
                Id = expenseGroup.Id,
                Title = expenseGroup.Title,
                UserId = expenseGroup.UserId,
                Expenses = expenseGroup.Expenses == null ? new List<Expense>() : expenseGroup.Expenses.Select(e => expenseFactory.CreateExpense(e)).ToList()
            };
        }


        public DTO.ExpenseGroup CreateExpenseGroup(ExpenseGroup expenseGroup)
        {
            return new DTO.ExpenseGroup()
            {
                Description = expenseGroup.Description,
                ExpenseGroupStatusId = expenseGroup.ExpenseGroupStatusId,
                Id = expenseGroup.Id,
                Title = expenseGroup.Title,
                UserId = expenseGroup.UserId,
                Expenses = expenseGroup.Expenses.Select(e => expenseFactory.CreateExpense(e)).ToList()
            };
        }

        public IEnumerable<ExpenseGroup> CreateExpenseGroups(IEnumerable<DTO.ExpenseGroup> expenseGroups)
        {
            return expenseGroups.Select(this.CreateExpenseGroup);
        }

        public IEnumerable<DTO.ExpenseGroup> CreateExpenseGroups(IEnumerable<ExpenseGroup> expenseGroups)
        {
            return expenseGroups.Select(this.CreateExpenseGroup);
        }

        public object CreateDataShapedObject(ExpenseGroup expenseGroup, List<string> fieldList)
        {
            return CreateDataShapedObject(CreateExpenseGroup(expenseGroup), fieldList);
        }

        public object CreateDataShapedObject(DTO.ExpenseGroup expenseGroup, List<string> fieldList)
        {
            List<string> listOfFieldsToWorkWith = new List<string>(fieldList);

            if (!listOfFieldsToWorkWith.Any())
            {
                return expenseGroup;
            }
            else
            {
                var listOfExpenseFields = listOfFieldsToWorkWith.Where(f => f.Contains("expenses")).ToList();

                var returnPartialExpenses = listOfExpenseFields.Any() && !listOfExpenseFields.Contains("expenses");

                if (returnPartialExpenses)
                {
                    listOfFieldsToWorkWith.RemoveRange(listOfExpenseFields);
                        // removes expense-related fields from the original list of fields
                    listOfExpenseFields =
                        listOfExpenseFields.Select(f => f.Substring(f.IndexOf(".", StringComparison.Ordinal) + 1))
                            .ToList();
                }
                else
                {
                    listOfExpenseFields.Remove("expenses");
                    listOfFieldsToWorkWith.RemoveRange(listOfExpenseFields);
                }    

                ExpandoObject objectToReturn = new ExpandoObject();

                foreach (var field in listOfFieldsToWorkWith)
                {
                    var fieldValue =
                        expenseGroup.GetType()
                            .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                            .GetValue(expenseGroup, null);

                    ((IDictionary<string, object>) objectToReturn).Add(field, fieldValue);
                }

                if (returnPartialExpenses)
                {
                    List<object> expenses = new List<object>();
                    foreach (var expense in expenseGroup.Expenses)
                    {
                        expenses.Add(expenseFactory.CreateDataShapedObject(expense, listOfExpenseFields));
                    }

                    ((IDictionary<string, object>) objectToReturn).Add("expenses", expenses);
                }

                return objectToReturn;
            }
        }
    }
}
