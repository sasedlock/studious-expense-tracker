using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using ExpenseTracker.API.Loggers;
using ExpenseTracker.Repository.Dapper;
using ExpenseTracker.Repository.Entities;
using ExpenseTracker.Repository.Interfaces;
using ExpenseTracker.Repository.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpenseTracker.API.Tests.DependencyInjection
{
    [TestClass]
    public class AutoFacTests
    {
        [TestInitialize]
        public void Initialize()
        {
            
        }

        [TestMethod]
        public void AutoFacRegistersDapperRepoCorrectly()
        {
            // Build container
            var builder = WebApiConfig.CreateDependencyInjectionContainer();

            IExpenseTrackerDapperRepository dapperRepo;
            // Resolve IExpenseTrackerDapperRepo
            using (var scope = builder.BeginLifetimeScope())
            {
                dapperRepo = scope.Resolve<IExpenseTrackerDapperRepository>();

            }
            // Assert
            Assert.IsInstanceOfType(dapperRepo, typeof(ExpenseTrackerDapperRepository));
        }

        [TestMethod]
        [Ignore] // Not able to currently decorate w/ AutoFac at this time
        public void AutoFacRegistersGenericBaseRepoCorrectly()
        {
            var builder = WebApiConfig.CreateDependencyInjectionContainer();

            IExpenseTrackerGenericRepository<Expense> expenseRepo; 
            using (var scope = builder.BeginLifetimeScope())
            {
                expenseRepo = scope.ResolveNamed<IExpenseTrackerGenericRepository<Expense>>("repo");
            }

            Assert.IsInstanceOfType(expenseRepo, typeof(ExpenseTrackerRepository<Expense>));
        }

        [TestMethod]
        public void AutoFacRegistersExpenseRepoCorrectly()
        {
            var builder = WebApiConfig.CreateDependencyInjectionContainer();

            IExpenseRepository expenseRepo;
            using (var scope = builder.BeginLifetimeScope())
            {
                expenseRepo = scope.Resolve<IExpenseRepository>();
            }

            Assert.IsInstanceOfType(expenseRepo, typeof(ExpenseRepository));
        }

        [TestMethod]
        public void AutoFacRegistersRepoLoggerCorrectly()
        {
            var builder = WebApiConfig.CreateDependencyInjectionContainer();

            using (var scope = builder.BeginLifetimeScope())
            {
                var repoLogger = scope.ResolveOptionalNamed<IExpenseTrackerGenericRepository<Expense>>("repo");
            }
        }
    }
}
