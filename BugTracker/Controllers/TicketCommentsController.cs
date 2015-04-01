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

namespace BugTracker.Controllers
{
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketComments
        public ActionResult Index(int? ticketId)
        {
            //var ticketComments = db.TicketComments.Include(t => t.Ticket);

            ViewBag.TicketId = ticketId;
            var ticketComments = db.TicketComments.Where(c => c.TicketId == ticketId);
            return View(ticketComments.ToList());
        }

        // GET: TicketComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // GET: TicketComments/Create
        public ActionResult Create(int? ticketId, string returnToController, string returnToAction)
        {
            ViewBag.ReturnToController = returnToController;
            ViewBag.ReturnToAction = returnToAction;
            ViewBag.TicketTitle = db.Tickets.First(t => t.Id == ticketId).Title;
            ViewBag.TicketId = ticketId;
            var ticket = db.Tickets.FirstOrDefault(t => t.Id == ticketId);
            ViewBag.Ticket = ticket;
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "id", "Name", ticket.TicketStatusId);

            var comment = new TicketComment();
            comment.Ticket = ticket;
            comment.TicketId = ticket.Id;

            return View(comment);
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,Created,TicketId,UserId")] TicketComment ticketComment,
            string returnToController, string returnToAction, int? TicketStatusId)
        {
            //Guests cannot save any changes
            if (User.IsInRole("Guest"))
            {
                if (returnToController == null)
                    return RedirectToAction("Index", new { ticketId = ticketComment.TicketId });
                else
                    return RedirectToAction(returnToAction, returnToController, new { id = ticketComment.TicketId });
            }

            if (ModelState.IsValid)
            {
                ticketComment.Created = DateTimeOffset.Now;
                ticketComment.Ticket = db.Tickets.Find(ticketComment.TicketId);

                if(TicketStatusId != null )
                {
                    ticketComment.Ticket.TicketStatusId = (int)TicketStatusId;
                    ticketComment.Ticket.TicketStatus = db.TicketStatus.First(s => s.id == TicketStatusId);
                }
                    

                var user = db.Users.Find(User.Identity.GetUserId());
                ticketComment.UserId = user.Id;
                ticketComment.User = user;

                

                ticketComment.Ticket.TicketComments.Add(ticketComment);

                db.TicketComments.Add(ticketComment);
                db.SaveChanges();

                if (returnToController == null)
                    return RedirectToAction("Index", new { ticketId = ticketComment.TicketId });
                else
                    return RedirectToAction(returnToAction, returnToController, new { id = ticketComment.TicketId });
                
            }

            //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);

            ViewBag.TicketTitle = db.Tickets.First(t => t.Id == ticketComment.TicketId).Title;
            return View(ticketComment);
        }

        // GET: TicketComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }

            ViewBag.TicketTitle = ticketComment.Ticket.Title;
            ViewBag.TicektId = ticketComment.Ticket.Id;
            
            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,Created,TicketId,UserId")] TicketComment ticketComment)
        {
            //Guests cannot save any changes
            if (User.IsInRole("Guest"))
                return RedirectToAction("Index", new { ticketId = ticketComment.TicketId });

            if (ModelState.IsValid)
            {
                ticketComment.Ticket = db.Tickets.Find(ticketComment.TicketId);
                ticketComment.User = db.Users.Find(ticketComment.UserId);

                db.Entry(ticketComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { ticketId = ticketComment.TicketId });
            }

            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            
            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.TicketComments.Find(id);

            //Guests cannot save any changes
            if (User.IsInRole("Guest"))
                return RedirectToAction("Index", new { ticketId = ticketComment.TicketId });

            
            db.TicketComments.Remove(ticketComment);
            db.SaveChanges();
            return RedirectToAction("Index", new { ticketId = ticketComment.TicketId });
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
