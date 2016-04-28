using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository.Entities
{
    public interface IExpenseTrackerDbContext
    {
        DbSet<Expense> Expenses { get; set; }
        DbSet<ExpenseGroup> ExpenseGroups { get; set; }
        DbSet<ExpenseGroupStatus> ExpenseGroupStatusses { get; set; }
        int SaveChanges();
        DbEntityEntry Entry(Object entity);
        void SetModified(Object entity);
        void SetDetached(Object entity);
    }
}
