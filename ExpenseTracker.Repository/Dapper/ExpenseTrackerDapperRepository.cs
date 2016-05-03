using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Repository.Dapper
{
    public class ExpenseTrackerDapperRepository : IExpenseTrackerDapperRepository
    {
        internal IDbConnection Connection => new SqlConnection(ConfigurationManager.ConnectionStrings["ExpenseTrackerContext"].ConnectionString);

        public IEnumerable<ExpenseGroup> GetAllExpenseGroups()
        {
            IEnumerable<ExpenseGroup> result;

            using (IDbConnection connection = Connection)
            {
                connection.Open();
                result = connection.Query<ExpenseGroup>("SELECT Id, UserId, Title, Description FROM dbo.ExpenseGroup");
            }

            return result;
        }

        public IEnumerable<ExpenseGroup> GetAllExpenseGroupsWithExpenses()
        {
            var result = new Dictionary<int, ExpenseGroup>();

            using (IDbConnection connection = Connection)
            {
                connection.Open();
                connection.Query<ExpenseGroup, Expense, ExpenseGroup>(@"  SELECT 
	                                                            eg.Id, 
	                                                            UserId, 
	                                                            Title, 
	                                                            eg.[Description],
	                                                            e.Id,
	                                                            e.Amount,
	                                                            e.[Date],
	                                                            e.[Description]
                                                                e.ExpenseGroupId
                                                            FROM dbo.ExpenseGroup eg
                                                            INNER JOIN dbo.Expense e ON eg.Id = e.ExpenseGroupId",
                    (eg, ex) =>
                    {
                        ExpenseGroup expenseGroup;
                        if (!result.TryGetValue(eg.Id, out expenseGroup))
                        {
                            result.Add(eg.Id, expenseGroup = eg);
                        }
                        if (expenseGroup.Expenses == null)
                        {
                            expenseGroup.Expenses = new List<Expense>();
                        }
                        expenseGroup.Expenses.Add(ex);
                        return expenseGroup;
                    });
            }

            return result.Values;
        }
    }
}
