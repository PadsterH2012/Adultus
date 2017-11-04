using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using WebApplication.Models;
using Microsoft.AspNet.Identity;

namespace WebApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private mysqldbEntities1 db = new mysqldbEntities1();
        public ActionResult Index(string searchString, string GenderPref, string OnlineOnly, int? page)
        {
            int GenderPrefIdx = 0;
            if (GenderPref != null) GenderPrefIdx = int.Parse(GenderPref);

            if (searchString != null) page = 1;
            else searchString = "";

            var users = from s in db.userlists
                           select s;

            bool bGenderPref = (GenderPrefIdx > 0 ? false : true);
            bool bOnline = (OnlineOnly != null ? false : true);

            users = users.Where((s => (s.Email.Contains(searchString)
                                       || s.UserName.Contains(searchString))
                                       && (s.Gender_Pref == GenderPrefIdx || bGenderPref )
                                       && (s.Online_Status == 1 || bOnline )
                                ));

            users = users.OrderBy(s => s.Join_Date);

            int pagesize = 10;
            ViewBag.pagesize = pagesize;
            int pageNumber = (page ?? 1);

            var userID = User.Identity.GetUserId();

            //Using aspnetclaim rules to store roles which have been manually inputted
            //NOT ADDED BY IDS
            aspnetuserclaim aspUserClaim = new aspnetuserclaim();
            aspUserClaim = db.aspnetuserclaims.Where(i => i.UserId == userID).Single();

            //Create new HomeViewModel which contains userlist and aspnetuserclaims objects
            List<HomeViewModel> homeViewModelList = new List<HomeViewModel>();
            HomeViewModel item = new HomeViewModel();
            item.userList = users.ToList();
            item.aspUserClaim = aspUserClaim;
            homeViewModelList.Add(item);

            return View(homeViewModelList.ToPagedList(pageNumber, pagesize));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}