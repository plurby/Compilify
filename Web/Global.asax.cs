﻿using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Web.Optimization;

namespace Compilify.Web
{
    public class Application : HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            MvcHandler.DisableMvcResponseHeader = true;

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterBundles(BundleTable.Bundles);
            RegisterRoutes(RouteTable.Routes);
        }

        private static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Root",
                url: "",
                defaults: new { controller = "Home", action = "Index" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") }
            );

            routes.MapRoute(
                name: "validate",
                url: "validate",
                defaults: new { controller = "Home", action = "Validate" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            routes.MapRoute(
                name: "Save",
                url: "{slug}",
                defaults: new { controller = "Home", action = "Save", slug = UrlParameter.Optional },
                constraints: new
                             {
                                 httpMethod = new HttpMethodConstraint("POST"),
                                 slug = @"[a-z0-9]*"
                             }
            );

            routes.MapRoute(
                name: "Show",
                url: "{slug}/{version}",
                defaults: new { controller = "Home", action = "Show", version = UrlParameter.Optional },
                constraints: new
                             {
                                 httpMethod = new HttpMethodConstraint("GET"),
                                 slug = @"[a-z0-9]+"
                             }
            );
        }

        private static void RegisterBundles(BundleCollection bundles)
        {
            var css = new Bundle("~/vendor/css", typeof(CssMinify));
            css.AddFile("~/assets/css/vendor/bootstrap-2.0.2.css");
            css.AddFile("~/assets/css/vendor/codemirror-2.23.css");
            css.AddFile("~/assets/css/vendor/codemirror-neat-2.23.css");
            bundles.Add(css);

            var js = new Bundle("~/vendor/js", typeof(JsMinify));
            js.AddFile("~/assets/js/vendor/json2.js");
            js.AddFile("~/assets/js/vendor/underscore-1.3.1.js");
            js.AddFile("~/assets/js/vendor/backbone-0.9.2.js");
            js.AddFile("~/assets/js/vendor/bootstrap-2.0.2.js");
            js.AddFile("~/assets/js/vendor/codemirror-2.23.js");
            js.AddFile("~/assets/js/vendor/codemirror-clike-2.23.js");
            js.AddFile("~/assets/js/vendor/jquery.signalr.js");
            bundles.Add(js);
        }
    }
}
