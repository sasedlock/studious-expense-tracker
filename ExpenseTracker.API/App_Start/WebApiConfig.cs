using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using Autofac.Core;
using Autofac.Integration.WebApi;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.Loggers;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Dapper;
using ExpenseTracker.Repository.Entities;
using ExpenseTracker.Repository.Factories;
using ExpenseTracker.Repository.Interfaces;
using ExpenseTracker.Repository.Repositories;
using ExpenseGroupStatusRepository = ExpenseTracker.Repository.Repositories.ExpenseGroupStatusRepository;

namespace ExpenseTracker.API
{
    public static class WebApiConfig
    {
        public static HttpConfiguration Register()
        {
            //SetUpIoC();

            var config = new HttpConfiguration();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.AddHttpRoutes();

            var builder = new ContainerBuilder();

            builder.RegisterType<ExpenseTrackerContext>().As<IExpenseTrackerDbContext>();
            builder.RegisterType<ExpenseTrackerDapperRepository>().As<IExpenseTrackerDapperRepository>();
            builder.RegisterType<ExpenseGroupFactory>().As<IExpenseGroupFactory>();
            builder.RegisterType<ExpenseTrackerUrlHelper>().As<IUrlHelper>();

            builder.RegisterType<ExpenseRepository>().As<IExpenseRepository>();
            builder.RegisterType<ExpenseGroupRepository>().As<IExpenseGroupRepository>();
            builder.RegisterType<ExpenseGroupStatusRepository>().As<IExpenseGroupStatusRepository>();

            builder.RegisterType<ExpenseRepository>()
                .Named<IExpenseTrackerGenericRepository<Expense>>("repoImplementation");

            builder.RegisterGenericDecorator(
                typeof (RepositoryLogger<>),
                typeof (IExpenseTrackerGenericRepository<>),
                "repoImplementation");

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            var container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(
                new MediaTypeHeaderValue("application/json-patch+json"));

            config.Formatters.JsonFormatter.SerializerSettings.Formatting
                = Newtonsoft.Json.Formatting.Indented;

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver
                = new CamelCasePropertyNamesContractResolver();

            var cacheCowMessageHandler = new CacheCow.Server.CachingHandler(config);
            config.MessageHandlers.Add(cacheCowMessageHandler);

            return config; // test
        }

        public static void AddHttpRoutes(this HttpRouteCollection routeCollection)
        {
            var routes = GetRoutes();
            routes.ForEach(route => routeCollection.MapHttpRoute(route.Name, route.Template, route.Defaults));
        }

        public static void AddHttpRoutes(this RouteCollection routeCollection)
        {
            var routes = GetRoutes();
            routes.ForEach(route => routeCollection.MapHttpRoute(route.Name, route.Template, route.Defaults));
        }

        private static List<Route> GetRoutes()
        {
            return new List<Route>
               {
                   new Route(
                       "DefaultRouting",
                       "api/{controller}/{id}",
                       new { id = RouteParameter.Optional })
               };
        }

        private class Route
        {
            public string Name { get; set; }
            public string Template { get; set; }
            public object Defaults { get; set; }

            public Route(string name, string template, object defaults)
            {
                this.Name = name;
                this.Template = template;
                this.Defaults = defaults;
            }
        }
    }
}
