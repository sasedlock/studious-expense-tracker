using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.SessionState;
using ExpenseTracker.API.Controllers;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Respawn;

namespace ExpenseTracker.API.IntegrationTests
{
    [TestClass]
    public class ExpenseGroupsIntegrationTests
    {
        private HttpClient _client;
        
        [TestInitialize]
        public void Initialize()
        {
            var config = new HttpConfiguration();
            config.Routes.AddHttpRoutes();
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            var server = new HttpServer(config);
            _client = new HttpClient(server);
        }

        [TestMethod]
        public void GettingExpenseGroupsWithOneRecordReturnsExpectedRecord()
        {
            HttpContext.Current = this.FakeHttpContext(
                new Dictionary<string, object>(), "http://localhost:43321/api/");

            var uri = "http://localhost:43321/api/expensegroups/";
            var result = _client.GetAsync(uri).Result;

            Assert.IsTrue(result.IsSuccessStatusCode);
            Assert.IsNotNull(result.Content);
        }

        #region Helpers

        public HttpContext FakeHttpContext(Dictionary<string, object> sessionVariables, string path)
        {
            var httpRequest = new HttpRequest(string.Empty, path, string.Empty);
            var stringWriter = new StringWriter();
            var httpResponce = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponce);
            httpContext.User = new GenericPrincipal(new GenericIdentity("username"), new string[0]);
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("username"), new string[0]);
            var sessionContainer = new HttpSessionStateContainer(
              "id",
              new SessionStateItemCollection(),
              new HttpStaticObjectsCollection(),
              10,
              true,
              HttpCookieMode.AutoDetect,
              SessionStateMode.InProc,
              false);

            foreach (var var in sessionVariables)
            {
                sessionContainer.Add(var.Key, var.Value);
            }

            SessionStateUtility.AddHttpSessionStateToContext(httpContext, sessionContainer);
            return httpContext;
        }

#endregion
    }
}
