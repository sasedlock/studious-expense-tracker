using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using ExpenseTracker.API.Controllers;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Entities;
using ExpenseTracker.Repository.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ExpenseTracker.API.Tests.Controllers
{
    [TestClass]
    public class ExpensesControllerTests
    {
        private Mock<IExpenseFactory> _mockFactory;
        private List<DTO.Expense> _expenseDtos;
        private List<Expense> _expenseEntities;
        private ExpensesController _controllerToTest;

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
