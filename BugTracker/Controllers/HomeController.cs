using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.Mvc;

using PagedList;

using BugTracker.Models;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Tickets");
            //return View(result);
        }






        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}