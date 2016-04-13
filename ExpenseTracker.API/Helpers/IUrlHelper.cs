using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace ExpenseTracker.API.Helpers
{
    public interface IUrlHelper
    {
        string Link(string routeName, object routeValues, HttpRequestMessage request);
    }

    public class ExpenseTrackerUrlHelper : IUrlHelper
    {
        public ExpenseTrackerUrlHelper() { }

        public string Link(string rounteName, object routeValues, HttpRequestMessage request)
        {
            var urlHelper = new UrlHelper(request);
            return urlHelper.Link(rounteName, routeValues);
        }
    }
}
