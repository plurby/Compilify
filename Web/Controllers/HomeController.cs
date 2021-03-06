﻿using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Compilify.Models;
using Compilify.Services;
using Compilify.Web.Models;
using Compilify.Web.Services;

namespace Compilify.Web.Controllers
{
    public class HomeController : AsyncController
    {
        public HomeController(IPostRepository contentRepository)
        {
            db = contentRepository;
            compiler = new CSharpCompiler();
        }

        private readonly IPostRepository db;
        private readonly CSharpCompiler compiler;

        public ActionResult Index()
        {
            var viewModel = new PostViewModel();

            var builder = new StringBuilder();
            builder.AppendLine("// The value returned is displayed in the results panel below");
            builder.AppendLine("return \"Hello, world!\";");
            var code = builder.ToString();
            
            viewModel.Post.Content = code;

            viewModel.Errors = compiler.GetCompilationErrors(code);
            return View("Show", viewModel);
        }

        // GET /:slug           -> Equivilant to /:slug/1
        // GET /:slug/:version  -> Get a specific version of the content
        // GET /:slug/latest    -> Get the latest saved version of the content
        // GET /:slug/live      -> Watch or collaborate on the content in real time
        //
        [HttpGet]
        public ActionResult Show(string slug, int? version)
        {
            
            if (version <= 1)
            {
                // Redirect the user to /:slug instead of /:slug/1
                return RedirectToActionPermanent("Show", "Home", new { slug = slug, version = (int?)null });
            }

            var content = db.GetVersion(slug, version ?? 1);

            if (content == null)
            {
                return HttpNotFound();
            }

            var viewModel = new PostViewModel
            {
                Post = content, 
                Errors = compiler.GetCompilationErrors(content.Content)
            };

            if (Request.IsAjaxRequest())
            {
                return Json(new { status = "ok", data = viewModel }, JsonRequestBehavior.AllowGet);
            }
            
            return View("Show", viewModel);
        }
        
        [HttpGet]
        public ActionResult Latest(string slug)
        {
            var latest = db.GetLatestVersion(slug);

            if (latest < 1)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Show", "Home", new { slug = slug, version = latest });
        }

        [HttpPost]
        public ActionResult Save(string slug, Post post)
        {
            var result = db.Save(slug, post);

            var routeValues = new RouteValueDictionary { { "slug", result.Slug } };

            if (result.Version > 1)
            {
                routeValues.Add("version", result.Version);
            }

            var url = Url.Action("Show", routeValues);
            return Json(new { status = "ok", data = new { slug = result.Slug, version = result.Version, url = url } });
        }

        [HttpPost]
        public ActionResult Validate(string code)
        {
            var errors = compiler.GetCompilationErrors(code)
                                 .ToArray();

            return Json(new { status = "ok", data = errors });
        }
    }
}
