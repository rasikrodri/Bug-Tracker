using BugTracker.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class UserRolesController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: UserRoles
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ViewBag.Controller = "UserRoles";

            var helper = new UserHelper();
            var userRoles = new List<UserRole>();
            foreach (IdentityRole role in db.Roles)
                userRoles.Add(new UserRole { 
                    Role = role,
                    //subscribedUsers = helper.UsersInRole(role.Name)
                });
            return View(userRoles);
            //return View(db.Roles.ToList());
        }

        // GET: UserRoles/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id)
        {
            return View();
        }

        //// GET: UserRoles/Create
        //[Authorize(Roles = "Admin")]
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: UserRoles/Create
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: UserRoles/Edit/5
        //[Authorize(Roles = "Admin")]
        //public ActionResult Edit(string id)
        //{
        //    var helper = new UserHelper();
        //    var model = new UserRole();
        //    model.Role = db.Roles.First(r => r.Id == id);
        //    //model.subscribedUsers = helper.UsersInRole(model.Role.Name);
        //    ViewBag.UsersNotInRole = helper.UsersNotInRole(model.Role.Name);

        //    return View(model);
        //}

        //// POST: UserRoles/Edit/5
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public ActionResult Edit(UserRole model, string doWhat)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here
                

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET:
        [Authorize(Roles = "Admin")]
        public ActionResult RolesAdd_RemoveUsers(string id)
        {
            ViewBag.RoleId = id;
            var helper = new UserHelper();
            var userRole = new UserRole();
            userRole.Role = db.Roles.First(r => r.Id == id);

            ViewBag.Controller = "UserRoles";
            ViewBag.Role = userRole.Role.Name;

            userRole.subscribedUsers = new MultiSelectList(helper.UsersInRole(userRole.Role.Name), "Id", "UserName");
            userRole.noneSubscribedUsers = new MultiSelectList(helper.UsersNotInRole(userRole.Role.Name), "Id", "UserName");
            return View(userRole);
        }
        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult RolesAdd_RemoveUsers(string id, [Bind(Include = "subscribedUsers,noneSubscribedUsers,selectedSubscribedUsersId, selecteNoneSubscribedUsers")] UserRole model,
            string removeButton, string addButton)
        {
            //Guests cannot save any changes
            if (User.IsInRole("Guest"))
                return RedirectToAction("RolesAdd_RemoveUsers", new { id = id });

            if (ModelState.IsValid)
            {
                model.Role = db.Roles.First(r => r.Id == id);
                var helper = new UserHelper();

                if (removeButton != null)
                {
                    if (model.selectedSubscribedUsersId != null)
                    {
                        bool allow = true;
                        if (model.Role.Name == "Admin")
                        {
                            int totalSubscribedUsers = helper.UsersInRole(model.Role.Name).Count();
                            if (totalSubscribedUsers - model.selectedSubscribedUsersId.Length < 1)
                                allow = false;
                        }

                        if (model.Role.Name == "Submitter")
                            allow = false;

                        if (allow == true)
                        {
                            foreach (string userId in model.selectedSubscribedUsersId)
                            {
                                helper.RemoveUserFromRole(userId, model.Role.Name);
                                UserNotificationsHelper.Notify_UnasignedFromRole(userId, model.Role.Name);
                            }                                
                        }
                    }
                }
                else if (addButton != null)
                {
                    if (model.selecteNoneSubscribedUsers != null)
                    {
                        foreach (string userId in model.selecteNoneSubscribedUsers)
                        {
                            helper.AddUserToRole(userId, model.Role.Name);
                            UserNotificationsHelper.Notify_AsignedToRole(userId, model.Role.Name);
                        }
                    } 
                }
                //return RedirectToAction("RolesAdd_RemoveUsers", new { id = id });
            }

            return RedirectToAction("RolesAdd_RemoveUsers", new { id = id });
        }      

        //// GET: UserRoles/Delete/5
        //[Authorize(Roles = "Admin")]
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: UserRoles/Delete/5
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
