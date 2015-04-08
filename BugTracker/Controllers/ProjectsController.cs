using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;

using PagedList;
namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize(Roles= "Project Manager, Developer")]
        public ActionResult Index(string query, int? page, string sortingBy, List<string> selectedFilters)
        {
            //TempData["lastSelectedFilters"]

            if (selectedFilters == null)
                selectedFilters = new List<string>();

            //We cannot receive it as a binding in the parametters because the id has a "." and variable names cannot have
            //an "."
            //var selectedFilters = Request["filters.selectedSearchFilters"];
            ViewBag.Controller = "Projects";
            ViewBag.ShowWhat = "projects";
            if (sortingBy == null)
                sortingBy = "name";
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

                ViewBag.FoundCount = db.Projects.Count();
                return View(OrganizeItemsBy(sortingBy, db.Projects, pageNumber, pageSize));
            }
            else
            {
                var searchFilters = PrepareFilters(selectedFilters);
                ViewBag.SearchFilters = searchFilters;
                return View(SearchFiltered(query, sortingBy, searchFilters, pageNumber, pageSize));
            }
            //return View(db.Projects.ToList());
        }
        private IPagedList<Project> SearchFiltered(string query, string sortingBy, Search filters, int pageNumber, int pageSize)
        {
            var resutls = db.Projects.Where(p => filters.selectedSearchFilters.Any(f => f == "Name") && p.Name.Contains(query))
                .Union(db.Projects.Where(p => filters.selectedSearchFilters.Any(f => f == "Users") && p.ProjectUsers.Any(u => u.User.DisplayName.Contains(query))))
                ;

            while (resutls.Count() == 0)
            {
                pageNumber -= 1;
                if (pageNumber == 0) { pageNumber = 1; break; }

                resutls = db.Projects.Where(p => filters.selectedSearchFilters.Any(f => f == "Title") && p.Name.Contains(query))
                .Union(db.Projects.Where(p => filters.selectedSearchFilters.Any(f => f == "Users") && p.ProjectUsers.Any(u => u.User.DisplayName.Contains(query))))
                ;
            }

            ViewBag.FoundCount = resutls.Count();
            return OrganizeItemsBy(sortingBy, resutls, pageNumber, pageSize);
        }
        private Search PrepareFilters(List<string> selectedFilters)
        {
            //typeof(Ticket).GetProperties().Where(m=>m.MetadataToken == "").Select(m => new SelectListItem{ Value = m.Name, Text = m.Name })
            var items = new List<SelectListItem>() {
                new SelectListItem { Value = "Name", Text = "Name" },
                new SelectListItem { Value = "Users", Text = "Users" },
                };

            var selected = new List<string>();

            if (selectedFilters.Count() == 0)
                foreach (var item in items) { selected.Add(item.Value); item.Selected = true; }

            else
                foreach (var item in items)
                    if (selectedFilters.Contains(item.Value)) { selected.Add(item.Value); item.Selected = true; }


            return new Search() { searchFilters = new MultiSelectList(items, "Value", "Text"), selectedSearchFilters = selected.ToArray() };     
        }
        private IPagedList<Project> OrganizeItemsBy(string _by, IQueryable<Project> list, int pageNumber, int pageSize)
        {
            IPagedList<Project> result = null;
            switch (_by)
            {
                case "name":
                    result = list.OrderBy(x => x.Name).ToPagedList(pageNumber, pageSize);
                    break;
                case "nameReverse":
                    result = list.OrderByDescending(x => x.Name).ToPagedList(pageNumber, pageSize);
                    break;
                case "tickets#":
                    result = list.OrderBy(x => x.Tickets.Count()).ToPagedList(pageNumber, pageSize);
                    break;
                case "tickets#Reverse":
                    result = list.OrderByDescending(x => x.Tickets.Count()).ToPagedList(pageNumber, pageSize);
                    break;
                case "users#":
                    result = list.OrderBy(x => x.ProjectUsers.Count()).ToPagedList(pageNumber, pageSize);
                    break;
                case "users#Reverse":
                    result = list.OrderByDescending(x => x.ProjectUsers.Count()).ToPagedList(pageNumber, pageSize);
                    break;     
            }
            return result;
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Project Manager, Developer")]
        public ActionResult Details(int? id)
        {
            ViewBag.Controller = "Projects";
            @ViewBag.Action = "Details";

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Project Manager")]
        public ActionResult Create()
        {
            ViewBag.Controller = "Projects";
            @ViewBag.Action = "Create";
            var project = new Project();
            return View(project);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project)
        {
            //Guests cannot save any changes
            if (User.IsInRole("Guest"))
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Edit", new {project.Id});
                //return RedirectToAction("Index");
            }

            ViewBag.Controller = "Projects";
            @ViewBag.Action = "Create";
            return View(project);
        }

        //// GET: Projects/Edit/5
        //[Authorize(Roles = "Project Manager")]
        //public ActionResult Edit(int? id)
        //{
        //    ViewBag.Controller = "Projects";
        //    @ViewBag.Action = "Edit";

        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Project project = db.Projects.Find(id);
        //    if (project == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(project);
        //}

        //// POST: Projects/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Project Manager")]
        //public ActionResult Edit([Bind(Include = "Id,Name")] Project project)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(project).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Add_RemoveUsers");
        //        //return RedirectToAction("Index");
        //    }
            
        //    ViewBag.Controller = "Projects";
        //    @ViewBag.Action = "Eidt";
        //    return View(project);
        //}

        // GET:
        [Authorize(Roles = "Project Manager")]
        public ActionResult Edit(int id)
        {
            ViewBag.Controller = "Projects";
            @ViewBag.Action = "Edit";

            //ViewBag.RoleId = id;
            var helper = new UserProjectsHelper();
            var project = db.Projects.First(p => p.Id == id);
            var subUsers = helper.UsersOnProject(id);
            var noneSubUsers = helper.UsersNotOnProject(id);
            project.subscribedUsers = new MultiSelectList(subUsers.OrderBy(u => u.DisplayName), "Id", "UserName");
            project.noneSubscribedUsers = new MultiSelectList(noneSubUsers.OrderBy(u => u.DisplayName), "Id", "UserName");
            
            //userRole.subscribedUsers = new MultiSelectList(helper.UsersInRole(userRole.Role.Name), "Id", "UserName");
            //userRole.noneSubscribedUsers = new MultiSelectList(helper.UsersNotInRole(userRole.Role.Name), "Id", "UserName");
            return View(project);
        }
        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public ActionResult Edit(int id, [Bind(Include = "Id, Name,subscribedUsers,noneSubscribedUsers,selectedSubscribedUsersId, selecteNoneSubscribedUsers")] Project model,
            string removeButton, string addButton, string saveButton)
        {

            //Guests cannot save any changes
            if (User.IsInRole("Guest"))
                return RedirectToAction("Edit", new { id = id });
                

            if (ModelState.IsValid)
            {
                var helper = new UserProjectsHelper();

                if (removeButton != null)
                {
                    if (model.selectedSubscribedUsersId != null)
                    {
                        foreach (string userId in model.selectedSubscribedUsersId)
                        {
                            helper.RemoveProjectUserFromProject(userId, id);
                            UserNotificationsHelper.Notify_UnasignedFromProject(userId, id);
                        }                            
                    }
                }
                else if (addButton != null)
                {
                    if (model.selecteNoneSubscribedUsers != null)
                    {
                        foreach (string userId in model.selecteNoneSubscribedUsers)
                        {
                            helper.AddUserToProject(userId, id);
                            UserNotificationsHelper.Notify_AsignedToProject(userId, id);
                        }                            
                    }
                }


                var proj = db.Projects.FirstOrDefault(p => p.Id == model.Id);
                if(proj!= null)
                {
                    proj.Name = model.Name;

                    db.Entry(proj).State = EntityState.Modified;
                    db.SaveChanges();
                }


                //return RedirectToAction("RolesEdit", new { id = id });
            }

            ViewBag.Controller = "Projects";
            @ViewBag.Action = "Edit";

            return RedirectToAction("Edit", new { id = id });
        }      

        // GET: Projects/Delete/5
        [Authorize(Roles = "Project Manager")]
        public ActionResult Delete(int? id)
        {
            ViewBag.Controller = "Projects";
            @ViewBag.Action = "Delete";

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Project Manager")]
        public ActionResult DeleteOne(int id, string query, int? page)
        {
            //Guests cannot save any changes
            if (User.IsInRole("Guest"))
                return RedirectToAction("Index", new { query = query, page = page });

            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();



            ViewBag.Controller = "Projects";
            @ViewBag.Action = "Delete";
            return RedirectToAction("Index", new { query = query, page = page });
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            //Guests cannot save any changes
            if (User.IsInRole("Guest"))
                return RedirectToAction("Index");

            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();



            ViewBag.Controller = "Projects";
            @ViewBag.Action = "Delete";
            return RedirectToAction("Index");
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
