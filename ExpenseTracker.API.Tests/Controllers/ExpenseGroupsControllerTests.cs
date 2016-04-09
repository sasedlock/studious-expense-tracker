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
    public class ExpenseGroupsControllerTests
    {
        private Mock<IExpenseTrackerRepository> _mockRespository;
        private Mock<IExpenseGroupFactory> _mockFactory;
        private List<DTO.ExpenseGroup> _expenseGroupDtos;
        private List<Repository.Entities.ExpenseGroup> _expenseGroupEntities;
        private List<Repository.Entities.ExpenseGroupStatus> _expenseGroupStatuses;
        private ExpenseGroupsController _controllerToTest;

        [TestInitialize]
        public void InitializeTests()
        {
            _expenseGroupStatuses = new List<ExpenseGroupStatus>
            {
                new ExpenseGroupStatus
                {
                    Description = "open",
                    Id = 1
                },
                new ExpenseGroupStatus
                {
                    Description = "confirmed",
                    Id = 2
                }
            };

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
                    ExpenseGroupStatusId = 2,
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
                    ExpenseGroupStatusId = 2,
                    Id = 3,
                    Title = "TestTitle3",
                    UserId = "TestUser3"
                }
            };

            _mockRespository = new Mock<IExpenseTrackerRepository>();
            _mockRespository.Setup(r => r.GetExpenseGroups()).Returns(_expenseGroupEntities.AsQueryable());
            _mockRespository.Setup(r => r.GetExpenseGroupStatusses()).Returns(_expenseGroupStatuses.AsQueryable());

            _mockFactory = new Mock<IExpenseGroupFactory>();
            _mockFactory.Setup(f => f.CreateExpenseGroups(_expenseGroupEntities)).Returns(_expenseGroupDtos);
        }

        [TestMethod]
        public void GetExpenseGroupsShouldReturnAllAvailableExpenseGroups()
        {
            _controllerToTest = new ExpenseGroupsController(_mockRespository.Object, _mockFactory.Object);
            _controllerToTest.Request = new HttpRequestMessage();
            _controllerToTest.Request.RequestUri = new Uri("https://www.google.com/?search?q=hello");
            _controllerToTest.Configuration = new HttpConfiguration();

            var result = _controllerToTest.Get() as OkNegotiatedContentResult<IEnumerable<DTO.ExpenseGroup>>;

            Assert.IsNotNull(result);
            Assert.AreEqual(_expenseGroupDtos, result.Content);
        }

        [TestMethod]
        public void GetExpenseGroupsFilterOnStatusReturnsExpectedResult()
        {
            var expectedExpenseGroupEntityList = new List<ExpenseGroup>
            {
                new ExpenseGroup
                {
                    Description = "TestDescription1",
                    ExpenseGroupStatusId = 1,
                    ExpenseGroupStatus = null,
                    Id = 1,
                    Title = "TestTitle1",
                    UserId = "TestUser1",
                    Expenses = new List<Expense>()
                }
            };

            var expectedExpenseGroupDtoList = new List<DTO.ExpenseGroup>
            {
                new DTO.ExpenseGroup
                {
                    Description = "TestDescription1",
                    ExpenseGroupStatusId = 1,
                    Id = 1,
                    Title = "TestTitle1",
                    UserId = "TestUser1"
                }
            };

            _mockFactory.Setup(
                f =>
                    f.CreateExpenseGroups(
                        It.Is<List<ExpenseGroup>>(
                            egl => egl[0].Description == expectedExpenseGroupEntityList[0].Description)))
                .Returns(expectedExpenseGroupDtoList);

            _controllerToTest = new ExpenseGroupsController(_mockRespository.Object, _mockFactory.Object);
            _controllerToTest.Request = new HttpRequestMessage();
            _controllerToTest.Request.RequestUri = new Uri("https://www.google.com/?search?q=hello");
            _controllerToTest.Configuration = new HttpConfiguration();

            var status = "open";
            var result =
                _controllerToTest.Get(status: status) as OkNegotiatedContentResult<IEnumerable<DTO.ExpenseGroup>>;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Content.ToList().Count);
        }

        [TestMethod]
        public void GetExpenseGroupsFilteredByUserIdReturnsExpectedResults()
        {
            var expectedExpenseGroupEntityList = new List<ExpenseGroup>
            {
                new ExpenseGroup
                {
                    Description = "TestDescription1",
                    ExpenseGroupStatusId = 1,
                    ExpenseGroupStatus = null,
                    Id = 1,
                    Title = "TestTitle1",
                    UserId = "TestUser1",
                    Expenses = new List<Expense>()
                }
            };

            var expectedExpenseGroupDtoList = new List<DTO.ExpenseGroup>
            {
                new DTO.ExpenseGroup
                {
                    Description = "TestDescription1",
                    ExpenseGroupStatusId = 1,
                    Id = 1,
                    Title = "TestTitle1",
                    UserId = "TestUser1"
                }
            };

            _mockFactory.Setup(
                f =>
                    f.CreateExpenseGroups(
                        It.Is<List<ExpenseGroup>>(
                            egl => egl[0].Description == expectedExpenseGroupEntityList[0].Description)))
                .Returns(expectedExpenseGroupDtoList);

            _controllerToTest = new ExpenseGroupsController(_mockRespository.Object, _mockFactory.Object);
            _controllerToTest.Request = new HttpRequestMessage();
            _controllerToTest.Request.RequestUri = new Uri("https://www.google.com/?search?q=hello");
            _controllerToTest.Configuration = new HttpConfiguration();

            var userId = "TestUser1";
            var result =
                _controllerToTest.Get(userId: userId) as OkNegotiatedContentResult<IEnumerable<DTO.ExpenseGroup>>;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Content.ToList().Count);
        }

        [TestMethod]
        public void GetExpenseGroupsNullResultReturnsNotFoundResponse()
        {
            _mockRespository = new Mock<IExpenseTrackerRepository>();
            _mockFactory = new Mock<IExpenseGroupFactory>();

            _controllerToTest = new ExpenseGroupsController(_mockRespository.Object, _mockFactory.Object);

            var result = _controllerToTest.Get() as NotFoundResult;

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetExpenseGroupsFilteredOutReturnsNotFoundResponse()
        {
            var emptyExpenseGroupList = new List<ExpenseGroup>();

            _mockRespository.Setup(r => r.GetExpenseGroups()).Returns(emptyExpenseGroupList.AsQueryable());
            _mockFactory = new Mock<IExpenseGroupFactory>();

            _controllerToTest = new ExpenseGroupsController(_mockRespository.Object, _mockFactory.Object);

            var result = _controllerToTest.Get() as NotFoundResult;

            Assert.IsInstanceOfType(result, typeof (NotFoundResult));
        }

        [TestMethod]
        public void GetExpenseGroupsWithDefaultPagingReturnsExpectedResults()
        {
            _expenseGroupEntities = new List<ExpenseGroup>();

            for (var i = 0; i < 50; i++)
            {
                _expenseGroupEntities.Add(new ExpenseGroup
                {
                    Description = "ExpenseGroupDescription" + (i + 1),
                    ExpenseGroupStatusId = i < 25 ? 1 : 2,
                    Id = i + 1,
                    Title = "ExpenseGroupTitle" + (i + 1),
                    UserId = i < 10 ? "User1" : "User2"
                });
            }

            _expenseGroupDtos = new List<DTO.ExpenseGroup>();

            for (var i = 0; i < 50; i++)
            {
                _expenseGroupDtos.Add(new DTO.ExpenseGroup
                {
                    Description = "ExpenseGroupDescription" + (i + 1),
                    ExpenseGroupStatusId = i < 25 ? 1 : 2,
                    Id = i + 1,
                    Title = "ExpenseGroupTitle" + (i + 1),
                    UserId = i < 10 ? "User1" : "User2"
                });
            }

            _mockRespository.Setup(r => r.GetExpenseGroups()).Returns(_expenseGroupEntities.AsQueryable());
            _mockFactory.Setup(f => f.CreateExpenseGroups(It.Is<List<ExpenseGroup>>(egl => egl.Count == 5)))
                .Returns(_expenseGroupDtos.Take(5));

            _controllerToTest = new ExpenseGroupsController(_mockRespository.Object, _mockFactory.Object);
            _controllerToTest.Request = new HttpRequestMessage();
            _controllerToTest.Request.RequestUri = new Uri("https://www.google.com/?search?q=hello");
            _controllerToTest.Configuration = new HttpConfiguration();

            var result = _controllerToTest.Get() as OkNegotiatedContentResult<IEnumerable<DTO.ExpenseGroup>>;

            Assert.AreEqual(5, result.Content.Count());
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(_expenseGroupDtos[i].Id,result.Content.ToList()[i].Id);
            }
        }

        [TestMethod]
        public void GetExpenseGroupsPassingValidPageSizeReturnsExpectedResults()
        {
            _expenseGroupEntities = new List<ExpenseGroup>();

            for (var i = 0; i < 50; i++)
            {
                _expenseGroupEntities.Add(new ExpenseGroup
                {
                    Description = "ExpenseGroupDescription" + (i + 1),
                    ExpenseGroupStatusId = i < 25 ? 1 : 2,
                    Id = i + 1,
                    Title = "ExpenseGroupTitle" + (i + 1),
                    UserId = i < 10 ? "User1" : "User2"
                });
            }

            _expenseGroupDtos = new List<DTO.ExpenseGroup>();

            for (var i = 0; i < 50; i++)
            {
                _expenseGroupDtos.Add(new DTO.ExpenseGroup
                {
                    Description = "ExpenseGroupDescription" + (i + 1),
                    ExpenseGroupStatusId = i < 25 ? 1 : 2,
                    Id = i + 1,
                    Title = "ExpenseGroupTitle" + (i + 1),
                    UserId = i < 10 ? "User1" : "User2"
                });
            }

            var pageSize = 7;

            _mockRespository.Setup(r => r.GetExpenseGroups()).Returns(_expenseGroupEntities.AsQueryable());
            _mockFactory.Setup(f => f.CreateExpenseGroups(It.Is<List<ExpenseGroup>>(egl => egl.Count == pageSize)))
                .Returns(_expenseGroupDtos.Take(pageSize));

            _controllerToTest = new ExpenseGroupsController(_mockRespository.Object, _mockFactory.Object);
            _controllerToTest.Request = new HttpRequestMessage();
            _controllerToTest.Request.RequestUri = new Uri("https://www.google.com/?search?q=hello");
            _controllerToTest.Configuration = new HttpConfiguration();

            var result = _controllerToTest.Get(pageSize: pageSize) as OkNegotiatedContentResult<IEnumerable<DTO.ExpenseGroup>>;

            Assert.AreEqual(pageSize, result.Content.Count());
            for (var i = 0; i < pageSize; i++)
            {
                Assert.AreEqual(_expenseGroupDtos[i].Id, result.Content.ToList()[i].Id);
            }
        }

        [TestMethod]
        public void GetExpenseGroupsPassingAbovePageSizeReturnsExpectedResults()
        {
            _expenseGroupEntities = new List<ExpenseGroup>();

            for (var i = 0; i < 50; i++)
            {
                _expenseGroupEntities.Add(new ExpenseGroup
                {
                    Description = "ExpenseGroupDescription" + (i + 1),
                    ExpenseGroupStatusId = i < 25 ? 1 : 2,
                    Id = i + 1,
                    Title = "ExpenseGroupTitle" + (i + 1),
                    UserId = i < 10 ? "User1" : "User2"
                });
            }

            _expenseGroupDtos = new List<DTO.ExpenseGroup>();

            for (var i = 0; i < 50; i++)
            {
                _expenseGroupDtos.Add(new DTO.ExpenseGroup
                {
                    Description = "ExpenseGroupDescription" + (i + 1),
                    ExpenseGroupStatusId = i < 25 ? 1 : 2,
                    Id = i + 1,
                    Title = "ExpenseGroupTitle" + (i + 1),
                    UserId = i < 10 ? "User1" : "User2"
                });
            }

            var pageSize = 12;
            var maxPageSize = 10;

            _mockRespository.Setup(r => r.GetExpenseGroups()).Returns(_expenseGroupEntities.AsQueryable());
            _mockFactory.Setup(f => f.CreateExpenseGroups(It.Is<List<ExpenseGroup>>(egl => egl.Count == maxPageSize)))
                .Returns(_expenseGroupDtos.Take(maxPageSize));

            _controllerToTest = new ExpenseGroupsController(_mockRespository.Object, _mockFactory.Object);
            _controllerToTest.Request = new HttpRequestMessage();
            _controllerToTest.Request.RequestUri = new Uri("https://www.google.com/?search?q=hello");
            _controllerToTest.Configuration = new HttpConfiguration();

            var result = _controllerToTest.Get(pageSize: pageSize) as OkNegotiatedContentResult<IEnumerable<DTO.ExpenseGroup>>;

            Assert.AreEqual(maxPageSize, result.Content.Count());
            for (var i = 0; i < maxPageSize; i++)
            {
                Assert.AreEqual(_expenseGroupDtos[i].Id, result.Content.ToList()[i].Id);
            }
        }

        [TestMethod]
        public void GetExpenseGroupsPassingPageIndexReturnsExpectedResults()
        {
            _expenseGroupEntities = new List<ExpenseGroup>();

            for (var i = 0; i < 50; i++)
            {
                _expenseGroupEntities.Add(new ExpenseGroup
                {
                    Description = "ExpenseGroupDescription" + (i + 1),
                    ExpenseGroupStatusId = i < 25 ? 1 : 2,
                    Id = i + 1,
                    Title = "ExpenseGroupTitle" + (i + 1),
                    UserId = i < 10 ? "User1" : "User2"
                });
            }

            _expenseGroupDtos = new List<DTO.ExpenseGroup>();

            for (var i = 0; i < 50; i++)
            {
                _expenseGroupDtos.Add(new DTO.ExpenseGroup
                {
                    Description = "ExpenseGroupDescription" + (i + 1),
                    ExpenseGroupStatusId = i < 25 ? 1 : 2,
                    Id = i + 1,
                    Title = "ExpenseGroupTitle" + (i + 1),
                    UserId = i < 10 ? "User1" : "User2"
                });
            }

            var pageSize = 5;
            var maxPageSize = 10;
            var pageIndex = 2;

            _mockRespository.Setup(r => r.GetExpenseGroups()).Returns(_expenseGroupEntities.AsQueryable());
            _mockFactory.Setup(f => f.CreateExpenseGroups(It.Is<List<ExpenseGroup>>(egl => egl.Count == pageSize && egl.TrueForAll(eg => eg.Id > pageSize))))
                .Returns(_expenseGroupDtos.Skip((pageIndex - 1) * pageSize).Take(pageSize));

            _controllerToTest = new ExpenseGroupsController(_mockRespository.Object, _mockFactory.Object);
            _controllerToTest.Request = new HttpRequestMessage();
            _controllerToTest.Request.RequestUri = new Uri("https://www.google.com/?search?q=hello");
            _controllerToTest.Configuration = new HttpConfiguration();

            var result = _controllerToTest.Get(pageIndex: pageIndex) as OkNegotiatedContentResult<IEnumerable<DTO.ExpenseGroup>>;

            Assert.AreEqual(pageSize, result.Content.Count());
            for (var i = (pageIndex - 1) * pageSize; i < pageSize; i++)
            {
                Assert.AreEqual(_expenseGroupDtos[i].Id, result.Content.ToList()[i].Id);
            }
        }
        
        [TestMethod]
        public void CalculatePageNumbersReturnsExpectedResultEvenlyDistributed()
        {
            var expected = 5;

            _controllerToTest = new ExpenseGroupsController(_mockRespository.Object, _mockFactory.Object);

            var actual = _controllerToTest.CalculatePageNumbers(15, 3);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CalculatePageNumbersReturnsExpectedResultUnevenlyDistributed()
        {
            var expected = 5;

            _controllerToTest = new ExpenseGroupsController(_mockRespository.Object, _mockFactory.Object);

            var actual = _controllerToTest.CalculatePageNumbers(14, 3);

            Assert.AreEqual(expected, actual);
        }
    }
}
