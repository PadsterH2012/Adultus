using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication;

namespace WebApplication.Controllers
{
    public class userinfoesController : Controller
    {
        private mysqldbEntities1 db = new mysqldbEntities1();

        // GET: userinfoes
        public ActionResult Index()
        {
            return View(db.userinfoes.ToList());
        }

        // GET: userinfoes/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userinfo userinfo = db.userinfoes.Find(id);
            if (userinfo == null)
            {
                return HttpNotFound();
            }
            return View(userinfo);
        }

        // GET: userinfoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: userinfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserName,Account_Type,Date_Of_Birth,Gender,Gender_Pref,Credit_Amount,Join_Date,Last_Login,Profile_Pic,Gallery_Pics,Private_Gallery_Pics,City,County,Country,Postcode,Online_Status,Ranking,Score")] userinfo userinfo)
        {
            if (ModelState.IsValid)
            {
                db.userinfoes.Add(userinfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userinfo);
        }

        // GET: userinfoes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userinfo userinfo = db.userinfoes.Find(id);
            if (userinfo == null)
            {
                return HttpNotFound();
            }
            return View(userinfo);
        }

        // POST: userinfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,Account_Type,Date_Of_Birth,Gender,Gender_Pref,Credit_Amount,Join_Date,Last_Login,Profile_Pic,Gallery_Pics,Private_Gallery_Pics,City,County,Country,Postcode,Online_Status,Ranking,Score")] userinfo userinfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userinfo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userinfo);
        }

        // GET: userinfoes/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userinfo userinfo = db.userinfoes.Find(id);
            if (userinfo == null)
            {
                return HttpNotFound();
            }
            return View(userinfo);
        }

        // POST: userinfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            userinfo userinfo = db.userinfoes.Find(id);
            db.userinfoes.Remove(userinfo);
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
