using BugTracker.Controllers;
using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Common
{
    public class OnlyOwnerMakesTicketsChangesAuthorization : AuthorizeAttribute
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// If the logged user is the same user that created the post 
        /// set this to true
        /// </summary>
        private bool sameAsLogedUser { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            ControllerBase controller = filterContext.Controller;
            IPrincipal logedUser = controller.ControllerContext.HttpContext.User;
            //ApplicationDbContext database=null;

            ApplicationUser ticket_Author = null;
            var where = (string)filterContext.Controller.ControllerContext.RouteData.Values["controller"];
            if (where == "Tickets")
            {
                TicketsController ticketController = controller as TicketsController;
                var ticket_ID = Convert.ToInt32(ticketController.RouteData.Values["id"]);
                ticket_Author = ((Ticket)db.Tickets.AsNoTracking().First(p => p.Id == ticket_ID)).OwnerUser;
            }




            if (ticket_Author == null)
            {
                sameAsLogedUser = false;
            }
            else
            {
                if (!logedUser.Identity.IsAuthenticated)
                    sameAsLogedUser = false;
                else
                {
                    //Get the full loged in user so that we can access it's id
                    var actualLogedUser = db.Users.FirstOrDefault(u => u.UserName == logedUser.Identity.Name);

                    if (actualLogedUser.Id == ticket_Author.Id)
                        sameAsLogedUser = true;
                    else
                        sameAsLogedUser = false;
                }
            }


            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            // Make sure the user is authenticated.
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            if (sameAsLogedUser)
                return true;


            if (!httpContext.User.IsInRole(Roles)) //test if the user has one of the specified roles
                return false;

            return true;
        }
    }
}