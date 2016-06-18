using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using ExpenseTracker.DTO;
using ExpenseTracker.WebClient.Helpers;
using ExpenseTracker.WebClient.Models;
using Newtonsoft.Json;

namespace ExpenseTracker.WebClient.Controllers
{
    public class ExpenseGroupsController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var client = ExpenseTrackerHttpClient.GetClient();

            var model = new ExpenseGroupsViewModel();

            var egsResponse = await client.GetAsync("api/expensegroupstatusses");

            if (egsResponse.IsSuccessStatusCode)
            {
                var egsContent = await egsResponse.Content.ReadAsStringAsync();
                var listExpenseGroupStatusses =
                    JsonConvert.DeserializeObject<IEnumerable<ExpenseGroupStatus>>(egsContent);

                model.ExpenseGroupStatuses = listExpenseGroupStatusses;
            }
            else
            {
                return Content("An error occurred");
            }

            var response = await client.GetAsync("api/expensegroups");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var expenseGroupList = JsonConvert.DeserializeObject<IEnumerable<ExpenseGroup>>(content);

                model.ExpenseGroups = expenseGroupList;

            }
            else
            {
                return Content("An error occurred");
            }

            return View(model);
        }
    }
}
