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
        string Link(string routeName, object routeValues);
    }

    public class ExpenseTrackerUrlHelper : IUrlHelper
    {
        public ExpenseTrackerUrlHelper(HttpRequestMessage request)
        {
            urlHelper = new UrlHelper(request);
        }

        public ExpenseTrackerUrlHelper() { }

        public string Link(string rounteName, object routeValues)
        {
            return urlHelper.Link(rounteName, routeValues);
        }

        public UrlHelper urlHelper { get; set; }
    }
}
