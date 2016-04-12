using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Factories;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.UI.WebControls;
using ExpenseTracker.API.Helpers;
using Marvin.JsonPatch;

namespace ExpenseTracker.API.Controllers
{
    public class ExpenseGroupsController : ApiController
    {
        IExpenseTrackerRepository _repository;
        private IExpenseGroupFactory _expenseGroupFactory;
        public IUrlHelper _urlHelper;
        private const int MaxPageSize = 10;

        public ExpenseGroupsController() : this(new ExpenseTrackerEFRepository(new Repository.Entities.ExpenseTrackerContext()), new ExpenseGroupFactory())
        {
        }

        public ExpenseGroupsController(IExpenseTrackerRepository repository, IExpenseGroupFactory factory)
        {
            _repository = repository;
            _expenseGroupFactory = factory;
        }    

        [Route("api/expensegroups", Name = "ExpenseGroupsList")]
        public IHttpActionResult Get(string sort = "id", string status = null, string userId = null, int pageSize = 5, int pageIndex = 1)
        {
            try
            {
                pageSize = Math.Min(pageSize, MaxPageSize);

                var expenseGroups = _repository.GetExpenseGroups().ApplySort(sort);

                if (status != null)
                {
                    var expenseGroupStatus = _repository.GetExpenseGroupStatusses().FirstOrDefault(egs => string.Equals(egs.Description, status, StringComparison.CurrentCultureIgnoreCase));
                    if (expenseGroupStatus != null)
                    {
                        var statusId =
                            expenseGroupStatus.Id;

                        expenseGroups = expenseGroups.Where(eg => eg.ExpenseGroupStatusId == statusId);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }

                if (userId != null)
                {
                    expenseGroups = expenseGroups.Where(eg => eg.UserId == userId);
                }

                if (expenseGroups == null || !expenseGroups.Any())
                {
                    return NotFound();
                }
                else
                {
                    var numberOfResults = expenseGroups.Count();
                    var numberOfPages = CalculatePageNumbers(numberOfResults, pageSize);

                    expenseGroups = expenseGroups.Skip((pageIndex - 1) * pageSize).Take(pageSize);

                    //var urlHelper = new UrlHelper(Request);
                    _urlHelper = new ExpenseTrackerUrlHelper(Request);

                    var prevLink = 
                        pageIndex > 1 ?
                        _urlHelper.Link("ExpenseGroupsList",
                        new
                        {
                            pageIndex = pageIndex - 1,
                            pageSize = pageSize,
                            sort = sort,
                            status = status,
                            userId = userId
                        }) : "";

                    var nextLink =
                        pageIndex < numberOfPages ?
                        _urlHelper.Link("ExpenseGroupsList",
                        new
                        {
                            pageIndex = pageIndex + 1,
                            pageSize = pageSize,
                            sort = sort,
                            status = status,
                            userId = userId
                        }) : "";

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

                    return Ok(_expenseGroupFactory.CreateExpenseGroups(expenseGroups.ToList()));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        public IHttpActionResult Get(int id)
        {
            try
            {
                var expenseGroup = _repository.GetExpenseGroup(id);

                if (expenseGroup == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(_expenseGroupFactory.CreateExpenseGroup(expenseGroup));
                }
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] DTO.ExpenseGroup expenseGroup)
        {
            try
            {
                if (expenseGroup == null)
                {
                    return BadRequest();
                }
                else
                {
                    var result = _repository.InsertExpenseGroup(_expenseGroupFactory.CreateExpenseGroup(expenseGroup));

                    if (result.Status == RepositoryActionStatus.Created)
                    {
                        return Created(Request.RequestUri + "/" + result.Entity.Id.ToString(), result.Entity);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody] DTO.ExpenseGroup expenseGroup)
        {
            try
            {
                if (expenseGroup == null || expenseGroup.Id == 0 || id == 0)
                {
                    return BadRequest();
                }

                var result = _repository.UpdateExpenseGroup(_expenseGroupFactory.CreateExpenseGroup(expenseGroup));
                if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }
                if (result.Status == RepositoryActionStatus.Updated)
                {
                    return Ok(_expenseGroupFactory.CreateExpenseGroup(result.Entity));
                }
                
                return BadRequest();
                
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody] JsonPatchDocument<DTO.ExpenseGroup> expenseGroupPatchDocument)
        {
            try
            {
                if (expenseGroupPatchDocument == null || id == 0)
                {
                    return BadRequest();
                }

                var expenseGroupToUpdate = _repository.GetExpenseGroup(id);

                if (expenseGroupToUpdate == null)
                {
                    return NotFound();
                }

                var eg = _expenseGroupFactory.CreateExpenseGroup(expenseGroupToUpdate);

                expenseGroupPatchDocument.ApplyTo(eg);

                var result =
                    _repository.UpdateExpenseGroup(_expenseGroupFactory.CreateExpenseGroup(eg));

                if (result.Status == RepositoryActionStatus.Updated)
                {
                    return Ok(_expenseGroupFactory.CreateExpenseGroup(result.Entity));
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var result = _repository.DeleteExpenseGroup(id);

                if (result.Status == RepositoryActionStatus.Deleted)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
                if (result.Status == RepositoryActionStatus.NotFound)
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

        #region Helper Methods

        public int CalculatePageNumbers(int numberOfRecords, int pageSize)
        {
            return numberOfRecords % pageSize == 0 ? (int)numberOfRecords / pageSize : (int)Math.Ceiling((decimal)numberOfRecords / pageSize);
        }

        #endregion
    }
}
