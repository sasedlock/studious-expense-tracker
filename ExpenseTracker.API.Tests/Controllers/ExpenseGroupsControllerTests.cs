using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class ExpenseGroupsControllerTests
    {
        private Mock<IExpenseTrackerRepository> _mockRespository;
        private Mock<IExpenseGroupFactory> _mockFactory;
        private List<DTO.ExpenseGroup> _expenseGroupDtos;
        private List<Repository.Entities.ExpenseGroup> _expenseGroupEntities;
        private ExpenseGroupsController _controllerToTest;

        [TestMethod]
        public void GetExpenseGroupsShouldReturnAllAvailableExpenseGroups()
        {
            _expenseGroupEntities = new List<ExpenseGroup>
            {
                new ExpenseGroup
                {
                    Description = "TestDescription1",
                    ExpenseGroupStatusId = 1,
                    Id = 1,
                    Title = "TestTitle1",
                    UserId = "TestUser1"
                },
                new ExpenseGroup
                {
                    Description = "TestDescription2",
                    ExpenseGroupStatusId = 2,
                    Id = 2,
                    Title = "TestTitle2",
                    UserId = "TestUser2"
                },
                new ExpenseGroup
                {
                    Description = "TestDescription3",
                    ExpenseGroupStatusId = 3,
                    Id = 3,
                    Title = "TestTitle3",
                    UserId = "TestUser3"
                }
            };

            _expenseGroupDtos = new List<DTO.ExpenseGroup>
            {
                new DTO.ExpenseGroup
                {
                    Description = "TestDescription1",
                    ExpenseGroupStatusId = 1,
                    Id = 1,
                    Title = "TestTitle1",
                    UserId = "TestUser1"
                },
                new DTO.ExpenseGroup
                {
                    Description = "TestDescription2",
                    ExpenseGroupStatusId = 2,
                    Id = 2,
                    Title = "TestTitle2",
                    UserId = "TestUser2"
                },
                new DTO.ExpenseGroup
                {
                    Description = "TestDescription3",
                    ExpenseGroupStatusId = 3,
                    Id = 3,
                    Title = "TestTitle3",
                    UserId = "TestUser3"
                }
            };

            _mockRespository = new Mock<IExpenseTrackerRepository>();
            _mockRespository.Setup(f => f.GetExpenseGroups()).Returns(_expenseGroupEntities.AsQueryable());

            _mockFactory = new Mock<IExpenseGroupFactory>();
            _mockFactory.Setup(f => f.CreateExpenseGroups(_expenseGroupEntities)).Returns(_expenseGroupDtos);

            _controllerToTest = new ExpenseGroupsController(_mockRespository.Object, _mockFactory.Object);

            var result = _controllerToTest.Get() as OkNegotiatedContentResult<IEnumerable<DTO.ExpenseGroup>>;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Content, _expenseGroupDtos);
        }
    }
}
