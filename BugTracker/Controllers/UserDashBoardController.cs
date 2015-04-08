using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    public class UserDashBoardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserDashBoard
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.TicketsModel = db.Tickets.Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
