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
using Newtonsoft.Json;

namespace ExpenseTracker.WebClient.Controllers
{
    public class ExpenseGroupsController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var client = ExpenseTrackerHttpClient.GetClient();
            HttpResponseMessage response = await client.GetAsync("api/expensegroups");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<IEnumerable<ExpenseGroup>>(content);

                return View(model);
            }
            else
            {
                return Content("An error occurred");
            }
        }
    }
}
