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
using System.IO;


using PagedList;
using BugTracker.Common;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        [Authorize(Roles = "Submitter, Guest")]
        public ActionResult Index(string query, int? page, string sortingBy, List<string> selectedFilters)
        {
            //TempData["lastSelectedFilters"]

            if (selectedFilters == null)
                selectedFilters = new List<string>();

            //We cannot receive it as a binding in the parametters because the id has a "." and variable names cannot have
            //an "."
            //var selectedFilters = Request["filters.selectedSearchFilters"];
            ViewBag.Controller = "Tickets";
            ViewBag.ShowWhat = "tickets";            
            if (sortingBy==null)
                sortingBy = "created";
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

                ViewBag.FoundCount = db.Tickets.Count();
                return View(OrganizeItemsBy(sortingBy, db.Tickets, pageNumber, pageSize));                  
            }
            else
            {
                var searchFilters = PrepareFilters(selectedFilters);
                ViewBag.SearchFilters = searchFilters;
                return View(SearchFiltered(query, sortingBy, searchFilters, pageNumber, pageSize));
            }
        }  
        private IPagedList<Ticket> SearchFiltered(string query, string sortingBy, Search filters, int pageNumber, int pageSize)
        {

            //User,Asignment,Type,Priority,Status,Project

            var resutls = db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Title") && p.Title.Contains(query))
                .Union(db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Project") && p.Project.Name.Contains(query)))
                .Union(db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Type") && p.TicketType.Name.Contains(query)))
                .Union(db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Status") && p.TicketStatus.Name.Contains(query)))
                .Union(db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Priority") && p.TicketPriority.Name.Contains(query)))
                .Union(db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Owner") && p.OwnerUser.DisplayName.Contains(query)))
                .Union(db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Assignee") && p.AssignedToUser.DisplayName.Contains(query)))

                //.Union(db.Tickets.Where(p => p.TicketComments.Any(c => c.User.FirstName.Contains(query))))
                //.Union(db.Tickets.Where(p => p.TicketComments.Any(c => c.User.LastName.Contains(query))))
                //.Union(db.Tickets.Where(p => p.TicketComments.Any(c => c.User.Email.Contains(query))))
                //.Union(db.Tickets.Where(p => p.TicketComments.Any(c => c.User.DisplayName.Contains(query))))
                //.Union(db.Tickets.Where(p => p.TicketComments.Any(c => c.Description.Contains(query))))
                ;

            while (resutls.Count() == 0)
            {
                pageNumber -= 1;
                if (pageNumber == 0) { pageNumber = 1; break; }

                resutls = db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Title") && p.Title.Contains(query))
                .Union(db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Project") && p.Project.Name.Contains(query)))
                .Union(db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Type") && p.TicketType.Name.Contains(query)))
                .Union(db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Status") && p.TicketStatus.Name.Contains(query)))
                .Union(db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Priority") && p.TicketPriority.Name.Contains(query)))
                .Union(db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Owner") && p.OwnerUser.DisplayName.Contains(query)))
                .Union(db.Tickets.Where(p => filters.selectedSearchFilters.Any(f => f == "Assignee") && p.AssignedToUser.DisplayName.Contains(query)))

                //.Union(db.Tickets.Where(p => p.TicketComments.Any(c => c.User.FirstName.Contains(query))))
                    //.Union(db.Tickets.Where(p => p.TicketComments.Any(c => c.User.LastName.Contains(query))))
                    //.Union(db.Tickets.Where(p => p.TicketComments.Any(c => c.User.Email.Contains(query))))
                    //.Union(db.Tickets.Where(p => p.TicketComments.Any(c => c.User.DisplayName.Contains(query))))
                    //.Union(db.Tickets.Where(p => p.TicketComments.Any(c => c.Description.Contains(query))))
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
            new SelectListItem { Value = "Owner", Text = "Owner" },
            new SelectListItem { Value = "Assignee", Text = "Assignee" },
            new SelectListItem { Value = "Type", Text = "Type" },
            new SelectListItem { Value = "Priority", Text = "Priority" },
            new SelectListItem { Value = "Status", Text = "Status" },
            new SelectListItem { Value = "Project", Text = "Project" }
            };

            var selected = new List<string>();

            if (selectedFilters.Count()==0)
                foreach (var item in items) { selected.Add(item.Value); item.Selected = true; }
                        
            else
                foreach (var item in items)
                    if (selectedFilters.Contains(item.Value)) { selected.Add(item.Value); item.Selected = true; }


            return new Search() { searchFilters = new MultiSelectList(items, "Value", "Text"), selectedSearchFilters = selected.ToArray() };     
            
        }
        private IPagedList<Ticket> OrganizeItemsBy(string _by, IQueryable<Ticket> list, int pageNumber, int pageSize)
        {
            IPagedList<Ticket> result = null;
            switch (_by)
            {
                case "title":
                    result = list.OrderBy(x => x.Title).ToPagedList(pageNumber, pageSize);
                    break;
                case "titleReverse":
                    result = list.OrderByDescending(x => x.Title).ToPagedList(pageNumber, pageSize);
                    break;
                case "project":
                    result = list.OrderBy(x => x.Project.Name).ToPagedList(pageNumber, pageSize);
                    break;
                case "projectReverse":
                    result = list.OrderByDescending(x => x.Project.Name).ToPagedList(pageNumber, pageSize);
                    break;
                case "type":
                    result = list.OrderBy(x => x.TicketType.Name).ToPagedList(pageNumber, pageSize);
                    break;
                case "typeReverse":
                    result = list.OrderByDescending(x => x.TicketType.Name).ToPagedList(pageNumber, pageSize);
                    break;
                case "status":
                    result = list.OrderBy(x => x.TicketStatus.Name).ToPagedList(pageNumber, pageSize);
                    break;
                case "statusReverse":
                    result = list.OrderByDescending(x => x.TicketStatus.Name).ToPagedList(pageNumber, pageSize);
                    break;
                case "priority":
                    result = list.OrderBy(x => x.TicketPriority.Name).ToPagedList(pageNumber, pageSize);
                    break;
                case "priorityReverse":
                    result = list.OrderByDescending(x => x.TicketPriority.Name).ToPagedList(pageNumber, pageSize);
                    break;
                case "owner":
                    result = list.OrderBy(x => x.OwnerUser.DisplayName).ToPagedList(pageNumber, pageSize);
                    break;
                case "ownerReverse":
                    result = list.OrderByDescending(x => x.OwnerUser.DisplayName).ToPagedList(pageNumber, pageSize);
                    break;

                //Only a project manager can asign an user to a ticket so this may be null
                case "assignee":
                    result = list.OrderBy(x => x.AssignedToUser.DisplayName).ToPagedList(pageNumber, pageSize);
                    break;
                case "assigneeReverse":
                    result = list.OrderByDescending(x => x.AssignedToUser.DisplayName).ToPagedList(pageNumber, pageSize);
                    break;


                case "created":
                    result = list.OrderBy(x => x.Created).ToPagedList(pageNumber, pageSize);
                    break;
                case "createdReverse":
                    result = list.OrderByDescending(x => x.Created).ToPagedList(pageNumber, pageSize);
                    break;

                //This may be null untill an update occurs
                case "updated":
                    result = list.OrderBy(x => x.Updated).ToPagedList(pageNumber, pageSize);
                    break;
                case "updatedReverse":
                    result = list.OrderByDescending(x => x.Updated).ToPagedList(pageNumber, pageSize);
                    break;
            }
            return result;
        }        
        private IPagedList<TicketComment> SearchTicketComments(int? ticketId, int? page)
        {
            int pageSize = 7;
            int pageNumber = page ?? 1;

            var ticket = db.Tickets.First(t => t.Id == ticketId);
            return ticket.TicketComments.ToPagedList(pageNumber, pageSize);
        }







        // GET: Tickets/Details/5
        [Authorize(Roles = "Submitter")]
        public ActionResult History(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            

            ViewBag.Id = id.Value;
            var history = HistoryHelper.TicketGetFullHistory(id);
            @ViewBag.FoundCount = history.Count();
            @ViewBag.History = history;
            ViewBag.Controller = "Tickets";
            @ViewBag.Action = "History";

            return View(ticket);
        }

        // GET: Tickets/Details/5
        [Authorize(Roles = "Submitter")]
        public ActionResult Details(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            ViewBag.Id = id.Value;
            ViewBag.LatestTicketChanges = HistoryHelper.GetTicketLastHistoryForEveryProperty(ticket);
            ViewBag.Ticket = ticket;


            ViewBag.Controller = "Tickets";
            @ViewBag.Action = "Details";

            return View(SearchTicketComments(id, page));
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            
            ViewBag.Controller = "Tickets";
            @ViewBag.Action = "Create";


            //try
            //{
                var priority = db.TicketPriorities.FirstOrDefault(p => p.Name == "Normal");
                var openstatus = db.TicketStatus.FirstOrDefault(s => s.Name == "Open");
                var type = db.TicketTypes.FirstOrDefault(p => p.Name == "Develope");



                ViewBag.ProjectId = new SelectList(db.Projects.OrderBy(u => u.Name), "Id", "Name");
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", new { id = priority.Id });
                ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "id", "Name", new { id = openstatus.id });
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", new { id = type.Id });

                //The asignet to user is only shown after the project has been selected
                //Because we need to pass the list of users in that project
                var defaultProject = db.Projects.OrderBy(u => u.Name).FirstOrDefault();
                UserProjectsHelper projectHelper = new UserProjectsHelper();
                var projectUsers = new List<ApplicationUser>();
                foreach (var u in db.Users)
                    if (projectHelper.IsOnProject(u.Id, defaultProject.Id))
                        projectUsers.Add(u);

                ViewBag.AssignedToUserId = new SelectList(projectUsers.OrderBy(u => u.DisplayName), "Id", "DisplayName");

                //var projectUsers = db.Users.Where(u => defaultProject.ProjectUsers.Any(a => a.User.UserName == u.UserName));
                //ViewBag.AssignedToUserId = new SelectList(projectUsers.OrderBy(u => u.DisplayName), "Id", "DisplayName");
                //ViewBag.AssignedToUserId = new SelectList(db.Users.Where(u => u.Roles.Any(r => r.RoleId == "Developer")).OrderBy(u => u.DisplayName), "Id", "DisplayName");

            //}
            //catch (Exception e)
            //{
            //    ViewBag.errorMessage = e.Message;
            //    return View("Error");
            //}

            var ticket = new Ticket();


            return View(ticket);
        }
        //Action result for ajax call
        [HttpPost]
        public ActionResult GetProjectUsers(int projectid)
        {
            var project = db.Projects.FirstOrDefault(p => p.Id == projectid);
            if (project != null)
            {
                UserProjectsHelper projectHelper = new UserProjectsHelper();
                var projectUsers = new List<ApplicationUser>();
                foreach (var u in db.Users)
                    if (projectHelper.IsOnProject(u.Id, project.Id))
                        projectUsers.Add(u);

                SelectList proUsersList = new SelectList(projectUsers.OrderBy(u => u.DisplayName), "Id", "DisplayName", 0);
                return Json(proUsersList);
            }
            else
                return Json(null);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket,
            HttpPostedFileBase[] files, string attachementsdescription)
        {
            //Guests cannot save any changes
            if (User.IsInRole("Guest"))
                return RedirectToAction("Index");

            ViewBag.Controller = "Tickets";
            @ViewBag.Action = "Create";

            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName");

            
            if (ModelState.IsValid)
            {
                ticket.OwnerUserId = User.Identity.GetUserId();

                ticket.Created = DateTimeOffset.Now;
                if (files != null && files[0] != null && String.IsNullOrWhiteSpace(attachementsdescription))
                {
                    ModelState.AddModelError("AttachmentDescription", "The attachment description is required");
                    return View(ticket);
                }
                else if (files != null && files[0] != null && !String.IsNullOrWhiteSpace(attachementsdescription))
                {

                    if (files.Any(f => !(new string[] { ".jpg", ".png" }).Contains(Path.GetExtension(f.FileName).ToLower())))
                    {
                        ModelState.AddModelError("Files", "One or more attachment types is not supported.");
                        return View(ticket);
                    }


                    List<string> filesPath = new List<string>();
                    var attachement = new TicketAttachement
                    {
                        TicketId = ticket.Id,
                        Description = attachementsdescription,
                        UserId = User.Identity.GetUserId(),
                        Created = DateTimeOffset.Now
                    };
                    /*Lopp for multiple files*/
                    foreach (HttpPostedFileBase file in files)
                    {
                        if (file != null)
                        {
                            string filename = System.IO.Path.GetFileName(file.FileName);
                            string uniqueFileNamePre = ticket.Id.ToString() + "_" + filename;

                            //Saving the file in server folder
                            file.SaveAs(Path.Combine(Server.MapPath("~/ticketsattachemntsfiles/" + uniqueFileNamePre)));
                            //file.SaveAs(Path.Combine(Server.MapPath("~/ticketsattachemntsfiles/"), uniqueFileNamePre));
                            filesPath.Add("~/ticketsattachemntsfiles/" + uniqueFileNamePre);
                        }
                    }
                    attachement.FilesPath = SavingHelper.ListOfStringsToString(filesPath);
                    db.TicketAttachements.Add(attachement);
                }


                db.Tickets.Add(ticket);
                db.SaveChanges();


                //Create the notification
                UserNotificationsHelper.Notify_CreatedTicket(ticket.OwnerUserId, ticket.Id);
                if (ticket.AssignedToUserId != null)
                    UserNotificationsHelper.Notify_AsignedToTicket(ticket.AssignedToUserId, ticket.Id);

                return RedirectToAction("Index");
            }

            //ViewBag.Controller = "Tickets";
            //@ViewBag.Action = "Create";

            //ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            //ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            //ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "id", "Name", ticket.TicketStatusId);
            //ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            //ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName");
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [OnlyOwnerMakesTicketsChangesAuthorization(Roles = "Project Manager")]
        public ActionResult Edit(int? id)
        {
            ViewBag.Controller = "Tickets";
            @ViewBag.Action = "Edit";

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = id.Value;
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);

            //The asignet to user is only shown after the project has been selected
            //Because we need to pass the list of users in that project
            var defaultProject = db.Projects.OrderBy(u => u.Name).FirstOrDefault();
            UserProjectsHelper projectHelper = new UserProjectsHelper();
            var projectUsers = new List<ApplicationUser>();
            foreach (var u in db.Users)
                if (projectHelper.IsOnProject(u.Id, defaultProject.Id))
                    projectUsers.Add(u);

            ViewBag.AssignedToUserId = new SelectList(projectUsers.OrderBy(u => u.DisplayName), "Id", "DisplayName");

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OnlyOwnerMakesTicketsChangesAuthorization(Roles = "Project Manager")]
        public ActionResult Edit([Bind(Include = "Id,Title,DescriptionId,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket,
            HttpPostedFileBase[] files, string attachementsdescription)
        {

            //Guests cannot save any changes
            if (User.IsInRole("Guest"))
                return RedirectToAction("Index");

            ViewBag.Controller = "Tickets";
            @ViewBag.Action = "Edit";

            ViewBag.Id = ticket.Id;
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.AssignedToUserId);

            if (ModelState.IsValid)
            {
                ticket.Updated = DateTimeOffset.Now;
               
                if (files != null  && files[0]!=null && String.IsNullOrWhiteSpace(attachementsdescription))
                {
                    ModelState.AddModelError("AttachmentDescription", "The attachment description is required");
                    return View(ticket);
                }
                else if (files != null && files[0] != null && !String.IsNullOrWhiteSpace(attachementsdescription))
                {

                    if(files.Any(f=>!(new string[] {".jpg", ".png"} ).Contains(Path.GetExtension(f.FileName).ToLower()) ))
                    {
                        ModelState.AddModelError("Files", "One or more attachment types is not supported.");
                        return View(ticket);
                    }


                    List<string> filesPath = new List<string>();
                    var attachement = new TicketAttachement
                    {
                        TicketId = ticket.Id,
                        Description = attachementsdescription,
                        UserId = User.Identity.GetUserId(),
                        Created = DateTimeOffset.Now
                    };
                    /*Lopp for multiple files*/
                    foreach (HttpPostedFileBase file in files)
                    {
                        if (file != null)
                        {
                            string filename = System.IO.Path.GetFileName(file.FileName);
                            string uniqueFileNamePre = ticket.Id.ToString() + "_" + filename;

                            //Saving the file in server folder
                            file.SaveAs(Path.Combine(Server.MapPath("~/ticketsattachemntsfiles/" + uniqueFileNamePre)));
                            //file.SaveAs(Path.Combine(Server.MapPath("~/ticketsattachemntsfiles/"), uniqueFileNamePre));
                            filesPath.Add("~/ticketsattachemntsfiles/" + uniqueFileNamePre);
                        }
                    }
                    attachement.FilesPath = SavingHelper.ListOfStringsToString(filesPath);
                    db.TicketAttachements.Add(attachement);
                }
                NotiffyTicketEdited(ticket);
                HistoryHelper.SaveTicketToHistory(ticket);//Has to be alled before we save the record otherwise it will change

                //db.Entry(ticket.Description).State =
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                //db.Entry(ticket).State = EntityState.Modified;                
                //db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View(ticket);
        }

        private void NotiffyTicketEdited(Ticket ticket)
        {
            UserNotificationsHelper.Notify_ChangedTicket(User.Identity.GetUserId(), ticket.Id);
            UserNotificationsHelper.Notify_ChangedTicket(ticket.OwnerUserId, ticket.Id);
            var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            if (oldTicket.AssignedToUser != ticket.AssignedToUser)
            {
                if (ticket.AssignedToUserId != null)
                {
                    UserNotificationsHelper.Notify_AsignedToTicket(ticket.AssignedToUserId, ticket.Id);
                    UserNotificationsHelper.Notify_UnasignedFromTicket(oldTicket.AssignedToUserId, ticket.Id);
                }
            }
            else if(ticket.AssignedToUserId != null)
            {
                UserNotificationsHelper.Notify_ChangedTicket(ticket.AssignedToUserId, ticket.Id);
            }
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Project Manager")]
        public ActionResult Delete(int? id)
        {
            ViewBag.Controller = "Tickets";
            @ViewBag.Action = "Delete";

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = id.Value;
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [Authorize(Roles = "Project Manager")]
        public ActionResult DeleteOne(int id, string query, int? page)
        {
            //Guests cannot save any changes
            if (User.IsInRole("Guest"))
                return RedirectToAction("Index", new { query = query, page = page });

            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();

            ViewBag.Controller = "Tickets";
            @ViewBag.Action = "Delete";

            return RedirectToAction("Index", new { query = query, page = page });
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            //Guests cannot save any changes
            if (User.IsInRole("Guest"))
                return RedirectToAction("Index");

            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();

            ViewBag.Controller = "Tickets";
            @ViewBag.Action = "Delete";

            return RedirectToAction("Index");
        }

        // GET: TicketComments/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult CreateComment(int? ticketId)
        {
            ViewBag.Id = db.Tickets.First(t=>t.Id == ticketId);
            ViewBag.TicketTitle = db.Tickets.First(t => t.Id == ticketId).Title;
            ViewBag.TicektId = ticketId;
            //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
            return View();
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
        public ActionResult CreateComment([Bind(Include = "Id,Description,Created,TicketId,UserId")] TicketComment ticketComment)
        {
            //Guests cannot save any changes
            if (User.IsInRole("Guest"))
                return RedirectToAction("Index", new { ticketId = ticketComment.TicketId });

            if (ModelState.IsValid)
            {
                ticketComment.Created = DateTimeOffset.Now;
                ticketComment.Ticket = db.Tickets.Find(ticketComment.TicketId);

                var user = db.Users.Find(User.Identity.GetUserId());
                ticketComment.UserId = user.Id;
                ticketComment.User = user;

                ticketComment.Ticket.TicketComments.Add(ticketComment);

                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
                return RedirectToAction("Index", new { ticketId = ticketComment.TicketId });

            }

            ViewBag.Id = ticketComment.TicketId;
            ViewBag.TicketTitle = db.Tickets.First(t => t.Id == ticketComment.TicketId).Title;
            return View(ticketComment);
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
