using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AssignRolesController : Controller
    {
        private mysqldbEntities1 db = new mysqldbEntities1();

        // GET: AssignRoles
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();

            //Using aspnetclaim rules to store roles which have been manually inputted
            //NOT ADDED BY IDS
            List<aspnetuserclaim> aspUserClaim = new List<aspnetuserclaim>();
            aspUserClaim = db.aspnetuserclaims.Where(i => i.UserId == userID).ToList();

            var roles = from r in db.roles
                        select r;

            //Create new HomeViewModel which contains userlist and aspnetuserclaims objects
            List<HomeViewModel> homeViewModelList = new List<HomeViewModel>();
            HomeViewModel item = new HomeViewModel();
            item.aspUserClaim = aspUserClaim;
            item.role = roles.ToList();
            homeViewModelList.Add(item);

            return View("~/Views/AssignRoles/Index.cshtml", "_Layout", homeViewModelList.ToPagedList(1, 1));
        }

        public ActionResult Create()
        {
            var userID = User.Identity.GetUserId();

            //Using aspnetclaim rules to store roles which have been manually inputted
            //NOT ADDED BY IDS
            List<aspnetuserclaim> aspUserClaim = new List<aspnetuserclaim>();
            aspUserClaim = db.aspnetuserclaims.Where(i => i.UserId == userID).ToList();


            var roles = from r in db.roles
                        select r;

            List<HomeViewModel> homeViewModelList = new List<HomeViewModel>();
            HomeViewModel item = new HomeViewModel();
            item.aspUserClaim = aspUserClaim;
            item.role = roles.ToList();
            homeViewModelList.Add(item);
            return View(homeViewModelList.ToPagedList(1, 1));
        }

        // POST: userinfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClaimType,ClaimValue,UserID")] aspnetuserclaim userClaim)
        {
            userClaim.Id = Guid.NewGuid().GetHashCode();
            userClaim.ClaimType = db.aspnetuserclaims.OrderBy(x => x.ClaimType).Single().ClaimType + 1;
            if (ModelState.IsValid)
            {
                db.aspnetuserclaims.Add(userClaim);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userClaim);
        }
    }
}