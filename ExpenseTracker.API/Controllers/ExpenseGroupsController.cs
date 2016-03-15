using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Factories;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls;
using ExpenseTracker.API.Helpers;
using Marvin.JsonPatch;

namespace ExpenseTracker.API.Controllers
{
    public class ExpenseGroupsController : ApiController
    {
        IExpenseTrackerRepository _repository;
        private IExpenseGroupFactory _expenseGroupFactory;
        private const int MaxPageSize = 10;

        public ExpenseGroupsController() : this(new ExpenseTrackerEFRepository(new Repository.Entities.ExpenseTrackerContext()), new ExpenseGroupFactory() )
        {
        }

        public ExpenseGroupsController(IExpenseTrackerRepository repository, IExpenseGroupFactory factory)
        {
            _repository = repository;
            _expenseGroupFactory = factory;
        }    


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

                // To implement paging, we must:
                
                
                // 3. Be able to tell the client how to get either the next or previous page (TBD)
                
                else
                {
                    var numberOfPages = CalculatePageNumbers(expenseGroups.Count(), pageSize);

                    expenseGroups = expenseGroups.Skip((pageIndex - 1) * pageSize).Take(pageSize);

                    var prevPath = pageIndex > 1
                        ? Request.RequestUri.AbsolutePath 
                            + "?sort=" + sort 
                            + (!string.IsNullOrEmpty(status) ? "&status=" + status : string.Empty)
                            + (!string.IsNullOrEmpty(userId) ? "&userId=" + userId : string.Empty)
                            + "&pageSize=" + pageSize
                            + "&pageIndex=" + (pageIndex - 1)
                        : Request.RequestUri.AbsolutePath;

                    var nextPath = pageIndex < numberOfPages
                        ? Request.RequestUri.AbsolutePath
                            + "?sort=" + sort
                            + (!string.IsNullOrEmpty(status) ? "&status=" + status : string.Empty)
                            + (!string.IsNullOrEmpty(userId) ? "&userId=" + userId : string.Empty)
                            + "&pageSize=" + pageSize
                            + "&pageIndex=" + (pageIndex + 1)
                        : Request.RequestUri.AbsolutePath;

                    return Ok(_expenseGroupFactory.CreateExpenseGroups(expenseGroups.ToList()));
                }
            }
            catch (Exception)
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
