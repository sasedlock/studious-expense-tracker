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

namespace ExpenseTracker.API.Controllers
{
    [RoutePrefix("api")]
    public class ExpensesController : ApiController
    {

        IExpenseTrackerRepository _repository;
        ExpenseFactory _expenseFactory = new ExpenseFactory();

        public ExpensesController()
        {
            _repository = new ExpenseTrackerEFRepository(new Repository.Entities.ExpenseTrackerContext());
        }

        public ExpensesController(IExpenseTrackerRepository repository)
        {
            _repository = repository;
        }

        //[Route("expenses/{id}")]
        //public IHttpActionResult Get(int id)
        //{
        //    try
        //    {
        //        var expense = _repository.GetExpense(id);

        //        if (expense != null)
        //        {
        //            return Ok(_expenseFactory.CreateExpense(expense));
        //        }
        //        else
        //        {
        //            return NotFound();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return InternalServerError();
        //    }
        //}

        [Route("expensegroups/{expenseGroupId}/expenses")]
        public IHttpActionResult Get(int expenseGroupId)
        {
            try
            {
                if (expenseGroupId == 0)
                {
                    return BadRequest();
                }

                var expenses = _repository.GetExpenses(expenseGroupId);

                if (expenses == null)
                {
                    return NotFound();
                }

                var expensesToReturn = _expenseFactory.CreateExpenses(expenses).ToList();

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

        [Route("expensegroups/{expenseGroupId}/expenses/{id}")]
        [Route("expenses/{id}")]
        public IHttpActionResult Get(int id, int? expenseGroupId = null)
        {
            try
            {
                Repository.Entities.Expense expense = null;

                if (expenseGroupId == null)
                {
                    expense = _repository.GetExpense(id);
                }
                else
                {
                    var expensesForGroup = _repository.GetExpenses((int)expenseGroupId);

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

                var result = _repository.DeleteExpense(id);

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

                var result = _repository.InsertExpense(exp);
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

                var result = _repository.UpdateExpense(exp);
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

                var expense = _repository.GetExpense(id);
                if (expense == null)
                {
                    return NotFound();
                }

                //// map
                var exp = _expenseFactory.CreateExpense(expense);

                // apply changes to the DTO
                expensePatchDocument.ApplyTo(exp);

                // map the DTO with applied changes to the entity, & update
                var result = _repository.UpdateExpense(_expenseFactory.CreateExpense(exp));

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



    }
}