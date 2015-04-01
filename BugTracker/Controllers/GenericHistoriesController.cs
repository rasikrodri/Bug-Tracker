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
    public class GenericHistoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: GenericHistories
        public ActionResult Index()
        {
            return View(db.GenericHistories.ToList());
        }

        // GET: GenericHistories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GenericHistory genericHistory = db.GenericHistories.Find(id);
            if (genericHistory == null)
            {
                return HttpNotFound();
            }
            return View(genericHistory);
        }

        // GET: GenericHistories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GenericHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ObjectId,Changed")] GenericHistory genericHistory)
        {
            if (ModelState.IsValid)
            {
                db.GenericHistories.Add(genericHistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(genericHistory);
        }

        // GET: GenericHistories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GenericHistory genericHistory = db.GenericHistories.Find(id);
            if (genericHistory == null)
            {
                return HttpNotFound();
            }
            return View(genericHistory);
        }

        // POST: GenericHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ObjectId,Changed")] GenericHistory genericHistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(genericHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(genericHistory);
        }

        // GET: GenericHistories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GenericHistory genericHistory = db.GenericHistories.Find(id);
            if (genericHistory == null)
            {
                return HttpNotFound();
            }
            return View(genericHistory);
        }

        // POST: GenericHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GenericHistory genericHistory = db.GenericHistories.Find(id);
            db.GenericHistories.Remove(genericHistory);
            db.SaveChanges();
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
