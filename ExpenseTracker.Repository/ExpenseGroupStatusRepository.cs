﻿using System.Linq;
using ExpenseTracker.Repository.Entities;
using ExpenseTracker.Repository.Interfaces;

namespace ExpenseTracker.Repository
{
    public class ExpenseGroupStatusRepository : IExpenseGroupStatusRepository
    {
        private readonly IExpenseTrackerDbContext _context;

        public ExpenseGroupStatusRepository(IExpenseTrackerDbContext context)
        {
            _context = context;
        }

        public ExpenseGroupStatus GetById(int id)
        {
            return _context.ExpenseGroupStatusses.FirstOrDefault(egs => egs.Id == id);
        }

        public IQueryable<ExpenseGroupStatus> GetAllAsQueryable()
        {
            return _context.ExpenseGroupStatusses;
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