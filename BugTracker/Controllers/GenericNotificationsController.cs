using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;

using Microsoft.AspNet.Identity;

using PagedList;

namespace BugTracker.Controllers
{
    public class GenericNotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: GenericNotifications
        [Authorize(Roles = "Submitter")]
        public ActionResult Index(string query, int? page, string sortingBy, List<string> selectedFilters)
        {
            //TempData["lastSelectedFilters"]

            if (selectedFilters == null)
                selectedFilters = new List<string>();

            //We cannot receive it as a binding in the parametters because the id has a "." and variable names cannot have
            //an "."
            //var selectedFilters = Request["filters.selectedSearchFilters"];
            ViewBag.Controller = "Notifications";
            ViewBag.ShowWhat = "notifications";
            if (sortingBy == null)
                sortingBy = "received";
            ViewBag.SortingBy = sortingBy;
            //ViewBag.SearchFilterFilters = searchFilters;
            @ViewBag.Page = page;
            ViewBag.currentQuery = query;

            int pageSize = 8;
            int pageNumber = page ?? 1;

            if (query == null || query == "")
            {
                var searchFilters = PrepareFilters(selectedFilters);
                ViewBag.SearchFilters = searchFilters;

                string userId = User.Identity.GetUserId();
                var userNotifications = db.Notifications.Where(g => g.ToUser.Id == userId);
                ViewBag.FoundCount = userNotifications.Count();
                return View(OrganizeItemsBy(sortingBy, userNotifications, pageNumber, pageSize));                  
            }
            else
            {
                var searchFilters = PrepareFilters(selectedFilters);
                ViewBag.SearchFilters = searchFilters;

                string userId = User.Identity.GetUserId();
                var userNotifications = db.Notifications.Where(g => g.ToUser.Id == userId);
                return View(SearchFiltered(query, userNotifications, sortingBy, searchFilters, pageNumber, pageSize));
            }
        }
        private IPagedList<GenericNotification> SearchFiltered(string query, IQueryable<GenericNotification> list, string sortingBy, Search filters, int pageNumber, int pageSize)
        {
            var resutls = list.Where(p => filters.selectedSearchFilters.Any(f => f == "Title") && p.Title.Contains(query))
                .Union(list.Where(p => filters.selectedSearchFilters.Any(f => f == "FromUser") && p.FromUser.DisplayName.Contains(query)))
                .Union(list.Where(p => filters.selectedSearchFilters.Any(f => f == "Message") && p.Message.Contains(query)))
                ;

            while (resutls.Count() == 0)
            {
                pageNumber -= 1;
                if (pageNumber == 0) { pageNumber = 1; break; }

                resutls = list.Where(p => filters.selectedSearchFilters.Any(f => f == "Title") && p.Title.Contains(query))
                .Union(list.Where(p => filters.selectedSearchFilters.Any(f => f == "FromUser") && p.FromUser.DisplayName.Contains(query)))
                .Union(list.Where(p => filters.selectedSearchFilters.Any(f => f == "Message") && p.Message.Contains(query)))
                ;
            }

            ViewBag.FoundCount = resutls.Count();
            return OrganizeItemsBy(sortingBy, resutls, pageNumber, pageSize);
        }
        private Search PrepareFilters(List<string> selectedFilters)
        {
            //typeof(Ticket).GetProperties().Where(m=>m.MetadataToken == "").Select(m => new SelectListItem{ Value = m.Name, Text = m.Name })
            var items = new List<SelectListItem>() {
                new SelectListItem { Value = "Title", Text = "Title" },
                new SelectListItem { Value = "FromUser", Text = "FromUser" },
                new SelectListItem { Value = "Message", Text = "Message" },
                };

            var selected = new List<string>();

            if (selectedFilters.Count() == 0)
                foreach (var item in items)  { selected.Add(item.Value); item.Selected = true; }
            else
                foreach (var item in items)
                    if (selectedFilters.Contains(item.Value)) { selected.Add(item.Value); item.Selected = true; }

            return new Search() { searchFilters = new MultiSelectList(items, "Value", "Text"), selectedSearchFilters = selected.ToArray() };
        }
        private IPagedList<GenericNotification> OrganizeItemsBy(string _by, IQueryable<GenericNotification> list, int pageNumber, int pageSize)
        {
            IPagedList<GenericNotification> result = null;
            switch (_by)
            {
                case "title":
                    result = list.OrderBy(x => x.Title).ToPagedList(pageNumber, pageSize);
                    break;
                case "titleReverse":
                    result = list.OrderByDescending(x => x.Title).ToPagedList(pageNumber, pageSize);
                    break;
                case "message":
                    result = list.OrderBy(x => x.Message).ToPagedList(pageNumber, pageSize);
                    break;
                case "messageReverse":
                    result = list.OrderByDescending(x => x.Message).ToPagedList(pageNumber, pageSize);
                    break;
                case "from":
                    result = list.OrderBy(x => x.FromUser.DisplayName).ToPagedList(pageNumber, pageSize);
                    break;
                case "fromReverse":
                    result = list.OrderByDescending(x => x.FromUser.DisplayName).ToPagedList(pageNumber, pageSize);
                    break;
                case "received":
                    result = list.OrderBy(x => x.Created).ToPagedList(pageNumber, pageSize);
                    break;
                case "receivedReverse":
                    result = list.OrderByDescending(x => x.Created).ToPagedList(pageNumber, pageSize);
                    break;
            }
            return result;
        }


     
        // GET: GenericNotifications/Details/5
        [Authorize(Roles = "Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GenericNotification genericNotification = db.Notifications.Find(id);
            if (genericNotification == null)
            {
                return HttpNotFound();
            }

            genericNotification.Read = true;
            db.Entry(genericNotification).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.Controller = "GenericNotifications";
            @ViewBag.Action = "Details";

            return View(genericNotification);
        }

        // GET: GenericNotifications/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create(string toUserId)
        {
            if (toUserId != null)
            {
                ViewBag.ToUserId = new SelectList(db.Users.OrderBy(u => u.DisplayName), "Id", "FirstName", new { id = toUserId });
            }
            else
            {
                ViewBag.ToUserId = new SelectList(db.Users.OrderBy(u => u.DisplayName), "Id", "FirstName");
            }

            

            ViewBag.Controller = "GenericNotifications";
            @ViewBag.Action = "Create";
            return View();
        }
        // POST: GenericNotifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
        public ActionResult Create([Bind(Include = "Id,Title,ObjectId,ToUserId,Read,Message")] GenericNotification genericNotification)
        {
            if (ModelState.IsValid)
            {
                var currUserId = User.Identity.GetUserId();
                genericNotification.FromUser = db.Users.First(u => u.Id == currUserId);
                genericNotification.FromUserId = User.Identity.GetUserId();

                genericNotification.ToUser = db.Users.First(u=>u.Id == genericNotification.ToUserId);

                db.Notifications.Add(genericNotification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Controller = "GenericNotifications";
            @ViewBag.Action = "Create";
            ViewBag.ToUserId = new SelectList(db.Users, "Id", "FirstName", genericNotification.ToUserId);
            return View(genericNotification);
        }

        // GET: GenericNotifications/Edit/5
        [Authorize(Roles = "Submitter")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GenericNotification genericNotification = db.Notifications.Find(id);
            if (genericNotification == null)
            {
                return HttpNotFound();
            }

            ViewBag.Controller = "GenericNotifications";
            @ViewBag.Action = "Edit";
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", genericNotification.ToUserId);
            return View(genericNotification);
        }

        // POST: GenericNotifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
        public ActionResult Edit([Bind(Include = "Id,ObjectId,UserId,Read,Message")] GenericNotification genericNotification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(genericNotification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Controller = "GenericNotifications";
            @ViewBag.Action = "Edit";

            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", genericNotification.ToUserId);
            return View(genericNotification);
        }


        // GET: GenericNotifications/Delete/5
        [Authorize(Roles = "Submitter")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GenericNotification genericNotification = db.Notifications.Find(id);
            if (genericNotification == null)
            {
                return HttpNotFound();
            }

            ViewBag.Controller = "GenericNotifications";
            @ViewBag.Action = "Delete";
            return View(genericNotification);
        }

        // POST: GenericNotifications/Delete/5
        [Authorize(Roles = "Submitter")]
        public ActionResult DeleteOne(int id, string query, int? page)
        {
            GenericNotification genericNotification = db.Notifications.Find(id);
            db.Notifications.Remove(genericNotification);
            db.SaveChanges();

            ViewBag.Controller = "GenericNotifications";
            @ViewBag.Action = "Delete";

            return RedirectToAction("Index", new {query = query, page = page });
        }

        // POST: GenericNotifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
        public ActionResult DeleteConfirmed(int id)
        {
            GenericNotification genericNotification = db.Notifications.Find(id);
            db.Notifications.Remove(genericNotification);
            db.SaveChanges();

            ViewBag.Controller = "GenericNotifications";
            @ViewBag.Action = "Delete";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
