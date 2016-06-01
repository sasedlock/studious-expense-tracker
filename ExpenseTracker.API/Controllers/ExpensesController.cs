using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Factories;
using Marvin.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.Loggers;
using ExpenseTracker.Repository.Interfaces;

namespace ExpenseTracker.API.Controllers
{
    [RoutePrefix("api")]
    public class ExpensesController : ApiController
    {
        private IExpenseRepository _repository;
        private ExpenseFactory _expenseFactory = new ExpenseFactory();
        private IUrlHelper _urlHelper;
        private const int MaxPageSize = 10;
        public ExpensesController(IExpenseRepository repository, IUrlHelper urlHelper)
        {
            _repository = repository;
            _urlHelper = urlHelper;
        }

        [Route("expensegroups/{expenseGroupId}/expenses", Name = "ExpensesList")]
        public IHttpActionResult Get(int expenseGroupId, string fields = null, string sort = "id", int pageSize = 5, int pageIndex = 1)
        {
            try
            {
                pageSize = Math.Min(pageSize, MaxPageSize);
                
                if (expenseGroupId == 0)
                {
                    return BadRequest();
                }

                List<string> listOfFields = new List<string>();

                if (fields != null)
                {
                    listOfFields = fields.ToLower().Split(',').ToList();
                }

                var expenses = _repository.GetExpensesByExpenseGroupId(expenseGroupId).ApplySort(sort);

                if (expenses == null)
                {
                    return NotFound();
                }

                var numberOfResults = expenses.Count();
                var numberOfPages = CalculatePageNumbers(numberOfResults, pageSize);

                expenses = expenses.Skip((pageIndex - 1)*pageSize).Take(pageSize);

                var prevLink =
                        pageIndex > 1 ?
                        _urlHelper.Link("ExpensesList",
                        new
                        {
                            pageIndex = pageIndex - 1,
                            fields = fields,
                            pageSize = pageSize,
                            sort = sort,
                        }, Request) : "";

                var nextLink =
                    pageIndex < numberOfPages ?
                    _urlHelper.Link("ExpensesList",
                    new
                    {
                        pageIndex = pageIndex + 1,
                        fields = fields,
                        pageSize = pageSize,
                        sort = sort,
                    }, Request) : "";

                var paginationHeader = new
                {
                    currentPage = pageIndex,
                    pageSize = pageSize,
                    totalCount = numberOfResults,
                    totalPages = numberOfPages,
                    previousPageLink = prevLink,
                    nextPageLink = nextLink
                };

                HttpContext.Current.Response.AddHeader("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

                var expensesToReturn =
                    expenses.ToList().Select(exp => _expenseFactory.CreateDataShapedObject(exp, listOfFields));

                if (!expensesToReturn.Any())
                {
                    return NotFound();
                }

                return Ok(expensesToReturn);

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [VersionedRoute("expensegroups/{expenseGroupId}/expenses/{id}",1)]
        [VersionedRoute("expenses/{id}", 1)]
        public IHttpActionResult Get(int id, int? expenseGroupId = null)
        {
            try
            {
                Repository.Entities.Expense expense = null;

                if (expenseGroupId == null)
                {
                    expense = _repository.GetById(id);
                }
                else
                {
                    var expensesForGroup = _repository.GetExpensesByExpenseGroupId((int)expenseGroupId);

                    if (expensesForGroup != null)
                    {
                        expense = expensesForGroup.FirstOrDefault(ex => ex.Id == id);
                    }
                }

                if (expense != null)
                {
                    return Ok(_expenseFactory.CreateExpense(expense));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [VersionedRoute("expensegroups/{expenseGroupId}/expenses/{id}", 2)]
        [VersionedRoute("expenses/{id}", 2)]
        public IHttpActionResult GetV2(int id, int? expenseGroupId = null)
        {
            try
            {
                Repository.Entities.Expense expense = null;

                if (expenseGroupId == null)
                {
                    expense = _repository.GetById(id);
                }
                else
                {
                    var expensesForGroup = _repository.GetExpensesByExpenseGroupId((int)expenseGroupId);

                    if (expensesForGroup != null)
                    {
                        expense = expensesForGroup.FirstOrDefault(ex => ex.Id == id);
                    }
                }

                if (expense != null)
                {
                    return Ok(_expenseFactory.CreateExpense(expense));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


        [Route("expenses/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {

                var result = _repository.Delete(id);

                if (result.Status == RepositoryActionStatus.Deleted)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("expenses")]
        public IHttpActionResult Post([FromBody]DTO.Expense expense)
        {
            try
            {
                if (expense == null)
                {
                    return BadRequest();
                }

                // map
                var exp = _expenseFactory.CreateExpense(expense);

                var result = _repository.Insert(exp);
                if (result.Status == RepositoryActionStatus.Created)
                {
                    // map to dto
                    var newExp = _expenseFactory.CreateExpense(result.Entity);
                    return Created<DTO.Expense>(Request.RequestUri + "/" + newExp.Id.ToString(), newExp);
                }

                return BadRequest();

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


        [Route("expenses/{id}")]
        public IHttpActionResult Put(int id, [FromBody]DTO.Expense expense)
        {
            try
            {
                if (expense == null)
                {
                    return BadRequest();
                }

                // map
                var exp = _expenseFactory.CreateExpense(expense);

                var result = _repository.Update(exp);
                if (result.Status == RepositoryActionStatus.Updated)
                {
                    // map to dto
                    var updatedExpense = _expenseFactory.CreateExpense(result.Entity);
                    return Ok(updatedExpense);
                }
                else if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


        [Route("expenses/{id}")]
        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody]JsonPatchDocument<DTO.Expense> expensePatchDocument)
        {
            try
            {
                // find 
                if (expensePatchDocument == null)
                {
                    return BadRequest();
                }

                var expense = _repository.GetById(id);
                if (expense == null)
                {
                    return NotFound();
                }

                //// map
                var exp = _expenseFactory.CreateExpense(expense);

                // apply changes to the DTO
                expensePatchDocument.ApplyTo(exp);

                // map the DTO with applied changes to the entity, & update
                var result = _repository.Update(_expenseFactory.CreateExpense(exp));

                if (result.Status == RepositoryActionStatus.Updated)
                {
                    // map to dto
                    var updatedExpense = _expenseFactory.CreateExpense(result.Entity);
                    return Ok(updatedExpense);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        #region Helper Methods

        public int CalculatePageNumbers(int numberOfRecords, int pageSize) // TODO - Refactor to consolidate for other controllers
        {
            return numberOfRecords % pageSize == 0 ? (int)numberOfRecords / pageSize : (int)Math.Ceiling((decimal)numberOfRecords / pageSize);
        }

        #endregion



    }
}