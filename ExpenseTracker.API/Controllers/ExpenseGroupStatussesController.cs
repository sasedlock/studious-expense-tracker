using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using ExpenseTracker.Repository.Interfaces;

namespace ExpenseTracker.API.Controllers
{
  //  [RoutePrefix("api/expensegroupstatusses")]
    public class ExpenseGroupStatussesController : ApiController
    {
        IExpenseGroupStatusRepository _repository;
        ExpenseMasterDataFactory _expenseMasterDataFactory = new ExpenseMasterDataFactory();

        public ExpenseGroupStatussesController(IExpenseGroupStatusRepository repository)
        {
            _repository = repository;
        }

        public IHttpActionResult Get()
        {

            try
            {
                // get expensegroupstatusses & map to DTO's
                var expenseGroupStatusses = _repository.GetAllAsQueryable().ToList()
                    .Select(egs => _expenseMasterDataFactory.CreateExpenseGroupStatus(egs));

                return Ok(expenseGroupStatusses);

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}