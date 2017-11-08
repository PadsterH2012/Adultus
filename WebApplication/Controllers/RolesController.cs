using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class RolesController : Controller
    {
        private mysqldbEntities1 db = new mysqldbEntities1();

        public ActionResult Index()
        {
            var roles = from s in db.aspnetuserclaims
                        select s;

            return View("~/Views/Roles/Index.cshtml", "_LayoutCrop", roles);
        }
    }
}