using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using System.IO;

using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class TicketAttachementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketAttachements
        public ActionResult Index(int ticketId)
        {
            //var ticketAttachements = db.TicketAttachements.Include(t => t.Ticket).Include(t => t.User);

            ViewBag.TicketId = ticketId;
            var ticketAttachements = db.TicketAttachements.Where(t => t.TicketId == ticketId);
            return View(ticketAttachements.ToList());
        }

        // GET: TicketAttachements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachement ticketAttachement = db.TicketAttachements.Find(id);
            if (ticketAttachement == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachement);
        }

        // GET: TicketAttachements/Create
        public ActionResult Create(int ticketId)
        {
            ViewBag.TicketTitle = db.Tickets.First(t => t.Id == ticketId).Title;
            ViewBag.TicektId = ticketId;

            //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
            //ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");

            return View();
        }

        // POST: TicketAttachements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,TicketId,FilePath,Description,Created,UserId,FileUrl")] TicketAttachement ticketAttachement,
            HttpPostedFileBase fileToUpload)
        {
            if (ModelState.IsValid)
            {
                var ticket = db.Tickets.First(t => t.Id == ticketAttachement.TicketId);

                if (fileToUpload != null && fileToUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fileToUpload.FileName);
                    fileToUpload.SaveAs(Path.Combine(Server.MapPath("~/Files/TicketsAttachements/"), fileName));
                    ticketAttachement.Created = DateTimeOffset.Now;
                    ticketAttachement.Ticket = ticket;
                    ticketAttachement.TicketId = ticketAttachement.TicketId;
                    //ticketAttachement.FilePath = "~/Files/TicketsAttachements/" + fileName;
                    ticketAttachement.UserId = User.Identity.GetUserId();
                    ticketAttachement.User = db.Users.First(u => u.Id == ticketAttachement.UserId);

                    ticket.TicketAttachments.Add(ticketAttachement);

                    db.TicketAttachements.Add(ticketAttachement);

                    db.SaveChanges();
                    return RedirectToAction("Index", "TicketAttachements", new { ticketId = ticketAttachement.TicketId });
                }
            }

            ViewBag.TicketTitle = db.Tickets.First(t => t.Id == ticketAttachement.TicketId).Title;
            ViewBag.TicektId = ticketAttachement.TicketId;

            //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachement.TicketId);
            //ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachement.UserId);
            return View(ticketAttachement);
        }

        // GET: TicketAttachements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachement ticketAttachement = db.TicketAttachements.Find(id);
            if (ticketAttachement == null)
            {
                return HttpNotFound();
            }
            //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachement.TicketId);
            //ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachement.UserId);

            ViewBag.TicketTitle = ticketAttachement.Ticket.Title;
            ViewBag.TicketId = ticketAttachement.Ticket.Id;
            //ViewBag.PrevFileName = Path.GetFileName(ticketAttachement.FilePath);

            return View(ticketAttachement);
        }

        // POST: TicketAttachements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,TicketId,FilePath,Description,Created,UserId,FileUrl")] TicketAttachement ticketAttachement,
           HttpPostedFileBase fileToUpload)
        {
            if (ModelState.IsValid)
            {
                if (fileToUpload != null && fileToUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fileToUpload.FileName);
                    fileToUpload.SaveAs(Path.Combine(Server.MapPath("~/Files/TicketsAttachements/"), fileName));
                    //ticketAttachement.FilePath = "~/Files/TicketsAttachements/" + fileName;
                }
                    ticketAttachement.Ticket = db.Tickets.Find(ticketAttachement.TicketId);
                    ticketAttachement.User = db.Users.Find(ticketAttachement.UserId);

                    db.Entry(ticketAttachement).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "TicketAttachements", new { ticketId = ticketAttachement.TicketId });
            }
            var attachement = db.TicketAttachements.Find(ticketAttachement.id);
            ViewBag.TicketTitle = attachement.Ticket.Title;
            ViewBag.TicketId = attachement.Ticket.Id;
            //@ViewBag.PrevFileName = Path.GetFileName(attachement.FilePath);
            //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachement.TicketId);
            //ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachement.UserId);
            return View(ticketAttachement);
        }

        // GET: TicketAttachements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachement ticketAttachement = db.TicketAttachements.Find(id);
            if (ticketAttachement == null)
            {
                return HttpNotFound();
            }

            return View(ticketAttachement);
        }

        // POST: TicketAttachements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachement ticketAttachement = db.TicketAttachements.Find(id);
            db.TicketAttachements.Remove(ticketAttachement);
            db.SaveChanges();
            return RedirectToAction("Index", "TicketAttachements", new { ticketId = ticketAttachement.TicketId });
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
