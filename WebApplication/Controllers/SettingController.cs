using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class SettingController : Controller
    {
        private mysqldbEntities1 db = new mysqldbEntities1();
        // GET: Setting
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ImageEdit()
        {
            return View("ImageEdit", "_LayoutCrop");
        }

        public async Task<ActionResult> ProfileView(int? mode)
        {
            if (mode == null) mode = 0;
            ViewBag.mode = mode;
            String userID = User.Identity.GetUserId();
            if (userID == null || userID.Length == 0) return View();

            switch (mode)
            {
                case 0://Profile
                {
                        string query = "SELECT * FROM userinfo WHERE userinfo.Id = '" + userID + "'";                        
                        userinfo UserInfo = await db.userinfoes.SqlQuery(query).SingleOrDefaultAsync();

                        query = "SELECT * FROM aspnetusers WHERE Id = '" + userID + "'";
                        aspnetuser aspnetUserInfo = await db.aspnetusers.SqlQuery(query).SingleOrDefaultAsync();

                        Models.RegisterViewModel model = new Models.RegisterViewModel();
                        
                        model.Email = User.Identity.Name;
                        //model.Password = "";
                        //model.ConfirmPassword = "";
                        model.UserName = UserInfo.UserName;
                        model.Birthday = (DateTime)(UserInfo.Date_Of_Birth == null ? new DateTime() : UserInfo.Date_Of_Birth);
                        model.Picture = UserInfo.Profile_Pic;
                        model.Gender = (int)UserInfo.Gender;
                        model.GenderPref = (int)UserInfo.Gender_Pref;
                        model.PhoneNumber = aspnetUserInfo.PhoneNumber;
                        model.Ethnicity = UserInfo.Ethnicity;
                        model.Interests = UserInfo.Interests;
                        model.City = UserInfo.City;
                        model.County = UserInfo.County;
                        model.Country = UserInfo.Country;
                        model.Postcode = UserInfo.Postcode;
                        model.AccountType = (int)(UserInfo.Account_Type == null ? 0 : UserInfo.Account_Type);

                        ViewBag.GalleryPics = UserInfo.Gallery_Pics;
                        ViewBag.PrivateGalleryPics = UserInfo.Private_Gallery_Pics;

                        return View("ProfileView", model);
                }
                case 1://Gallery
                {   
                    String GalleryPics = "";
                    if (userID != null)
                    {
                        string query = "SELECT * FROM userinfo WHERE userinfo.Id = '" + userID + "'";
                        System.Data.Entity.Infrastructure.DbSqlQuery<userinfo> userInfos = db.userinfoes.SqlQuery(query);

                        userinfo UserInfo = null;
                        if (userInfos != null) UserInfo = await userInfos.SingleOrDefaultAsync();

                        if (UserInfo != null && UserInfo.Gallery_Pics != null) GalleryPics = UserInfo.Gallery_Pics;
                    }
                    return View("ProfileView", (object)GalleryPics);
                }
                case 2://Private Gallery
                {
                    String GalleryPics = "";
                    if (userID != null)
                    {
                        string query = "SELECT * FROM userinfo WHERE userinfo.Id = '" + userID + "'";
                        System.Data.Entity.Infrastructure.DbSqlQuery<userinfo> userInfos = db.userinfoes.SqlQuery(query);

                        userinfo UserInfo = null;
                        if (userInfos != null) UserInfo = await userInfos.SingleOrDefaultAsync();

                        if (UserInfo != null && UserInfo.Gallery_Pics != null) GalleryPics = UserInfo.Private_Gallery_Pics;
                    }
                    return View("ProfileView", (object)GalleryPics);
                }
            }

            return View();
        }

        /// <summary>

        /// Process image and save in predefined path

        /// </summary>

        /// <param name="croppedImage"></param>

        /// <returns></returns>

        private void ProcessImage(string croppedImage, string filePath)
        {
            try
            {
                string base64 = croppedImage;
                byte[] bytes = Convert.FromBase64String(base64.Split(',')[1]);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
        }

        public async Task<ActionResult> ProfileEdit(RegisterViewModel model, HttpPostedFileBase picture_selector, String cropImgParam)
        {
            String userID = User.Identity.GetUserId();
            String Img = cropImgParam;
            ViewBag.GalleryPics = "";
            ViewBag.PrivateGalleryPics = "";
            if ( userID != null && userID.Length > 0 )
            {
                //{{Upload Picture
                String FileName = "";
                string PictureDirectory = Server.MapPath("~/Content/UserPicture");
                if (!Directory.Exists(PictureDirectory)) Directory.CreateDirectory(PictureDirectory);                
                if (picture_selector != null)
                {
                    FileName = userID + "." + picture_selector.FileName.Substring(picture_selector.FileName.Length - 3, 3);
                    var path = System.IO.Path.Combine(PictureDirectory, FileName);
                    var data = new byte[picture_selector.ContentLength];
                    picture_selector.InputStream.Read(data, 0, picture_selector.ContentLength);
                    using (var sw = new FileStream(path, FileMode.Create))
                    {
                        sw.Write(data, 0, data.Length);
                    }
                }
                else if(cropImgParam.Length > 0)
                {
                    FileName = userID + ".png";
                    var path = System.IO.Path.Combine(PictureDirectory, FileName);
                    ProcessImage(cropImgParam, path);
                }
                //}}Upload Picture


                ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                userManager.RemoveFromRoles(userID, new string[] {"member", "client" });
                userManager.AddToRole(userID, (model.AccountType == 0 ? "member" : "client"));

                //validate age with 18+
                if (DateTime.Today.AddYears(-18) < model.Birthday)
                {
                    ModelState.AddModelError("Birthday", "Members or Clients need to be 18 or above to register.");
                }

                ModelState.Remove("Password");
                if (this.ModelState.IsValid)
                {
                    aspnetuser aspuser = this.db.aspnetusers.SingleOrDefault(r => r.Id == userID);
                    aspuser.Email = model.Email;
                    aspuser.PhoneNumber = model.PhoneNumber;
                    if (model.Password != null && model.Password.Length > 0)
                    {
                        aspuser.PasswordHash = userManager.PasswordHasher.HashPassword(model.Password);
                    }

                    userinfo ui = this.db.userinfoes.SingleOrDefault(r=>r.Id== userID);
                    ViewBag.GalleryPics = ui.Gallery_Pics;
                    ViewBag.PrivateGalleryPics = ui.Private_Gallery_Pics;

                    ui.UserName = model.UserName;
                    ui.Account_Type = model.AccountType;
                    ui.Date_Of_Birth = model.Birthday;
                    ui.Gender = Convert.ToSByte(model.Gender);
                    ui.Gender_Pref = model.GenderPref;
                    ui.Credit_Amount = 0;
                    if (picture_selector != null || cropImgParam.Length > 0)
                    {
                        ui.Profile_Pic = FileName;
                        model.Picture = FileName;
                    }                    
                    ui.City = model.City;
                    ui.County = model.County;
                    ui.Country = model.Country;
                    ui.Postcode = model.Postcode;
                    this.db.SaveChanges();
                }
            }

            ViewBag.mode = 0;
            return View("ProfileView", model);
        }

        public async Task<ActionResult> AddGallery(int mode, HttpPostedFileBase picture_selector, IEnumerable<string> gallery)
        {
            String userID = User.Identity.GetUserId();
            String GalleryPics = "";
            if(mode == 0)
            {
                //{{Upload Picture
                String FileName = picture_selector.FileName;
                if (picture_selector != null && userID != null)
                {
                    string PictureDirectory = Server.MapPath("~/Content/UserPicture/Gallery/");
                    if (!Directory.Exists(PictureDirectory)) Directory.CreateDirectory(PictureDirectory);

                    PictureDirectory += userID;
                    if (!Directory.Exists(PictureDirectory)) Directory.CreateDirectory(PictureDirectory);

                    if (System.IO.File.Exists(PictureDirectory + "/" + FileName))
                    {
                        FileName = FileName.Insert(FileName.Length - 4, "(2)");
                    }

                    var path = PictureDirectory + "/" + FileName;
                    var data = new byte[picture_selector.ContentLength];
                    picture_selector.InputStream.Read(data, 0, picture_selector.ContentLength);

                    using (var sw = new FileStream(path, FileMode.Create))
                    {
                        sw.Write(data, 0, data.Length);
                    }

                    string query = "SELECT * FROM userinfo WHERE userinfo.Id = '" + userID + "'";
                    System.Data.Entity.Infrastructure.DbSqlQuery<userinfo> userInfos = db.userinfoes.SqlQuery(query);

                    userinfo UserInfo = null;
                    if (userInfos != null) UserInfo = await userInfos.SingleOrDefaultAsync();

                    if (UserInfo != null && UserInfo.Gallery_Pics != null) GalleryPics = UserInfo.Gallery_Pics;

                    GalleryPics += (GalleryPics.Length > 0 ? "," : "") + FileName;
                    db.Database.ExecuteSqlCommand("UPDATE userinfo SET Gallery_Pics = {0} where Id={1}", GalleryPics, userID);
                }
                //}}Upload Picture
            }
            else if(mode == 1 && gallery != null)//Remove picture
            {
                string query = "SELECT * FROM userinfo WHERE userinfo.Id = '" + userID + "'";
                System.Data.Entity.Infrastructure.DbSqlQuery<userinfo> userInfos = db.userinfoes.SqlQuery(query);

                userinfo UserInfo = null;
                if (userInfos != null) UserInfo = await userInfos.SingleOrDefaultAsync();

                if (UserInfo != null && UserInfo.Gallery_Pics != null) GalleryPics = UserInfo.Gallery_Pics;

                string PictureDirectory = Server.MapPath("~/Content/UserPicture/Gallery/");
                PictureDirectory += userID;
                foreach (String name in gallery)
                {
                    if (System.IO.File.Exists(PictureDirectory + "/" + name))
                    {
                        System.IO.File.Delete(PictureDirectory + "/" + name);
                    }
                    GalleryPics = GalleryPics.Replace(name, "");
                }
                string pattern = ",+";
                Regex rgx = new Regex(pattern);
                GalleryPics = rgx.Replace(GalleryPics, ",");
                db.Database.ExecuteSqlCommand("UPDATE userinfo SET Gallery_Pics = {0} where Id={1}", GalleryPics, userID);
            }


            ViewBag.mode = 1;
            return View("ProfileView", (object)GalleryPics);
        }

        public async Task<ActionResult> AddPrivateGallery(int mode, HttpPostedFileBase picture_selector, IEnumerable<string> gallery)
        {
            String userID = User.Identity.GetUserId();
            String GalleryPics = "";
            if (mode == 0)
            {
                //{{Upload Picture
                String FileName = picture_selector.FileName;
                if (picture_selector != null && userID != null)
                {
                    string PictureDirectory = Server.MapPath("~/Content/UserPicture/Private/");
                    if (!Directory.Exists(PictureDirectory)) Directory.CreateDirectory(PictureDirectory);

                    PictureDirectory += userID;
                    if (!Directory.Exists(PictureDirectory)) Directory.CreateDirectory(PictureDirectory);

                    if (System.IO.File.Exists(PictureDirectory + "/" + FileName))
                    {
                        FileName = FileName.Insert(FileName.Length - 4, "(2)");
                    }

                    var path = PictureDirectory + "/" + FileName;
                    var data = new byte[picture_selector.ContentLength];
                    picture_selector.InputStream.Read(data, 0, picture_selector.ContentLength);

                    using (var sw = new FileStream(path, FileMode.Create))
                    {
                        sw.Write(data, 0, data.Length);
                    }

                    string query = "SELECT * FROM userinfo WHERE userinfo.Id = '" + userID + "'";
                    System.Data.Entity.Infrastructure.DbSqlQuery<userinfo> userInfos = db.userinfoes.SqlQuery(query);

                    userinfo UserInfo = null;
                    if (userInfos != null) UserInfo = await userInfos.SingleOrDefaultAsync();

                    if (UserInfo != null && UserInfo.Gallery_Pics != null) GalleryPics = UserInfo.Private_Gallery_Pics;

                    GalleryPics += (GalleryPics.Length > 0 ? "," : "") + FileName;
                    db.Database.ExecuteSqlCommand("UPDATE userinfo SET Private_Gallery_Pics = {0} where Id={1}", GalleryPics, userID);
                }
                //}}Upload Picture
            }
            else if(mode == 1 && gallery != null)
            {
                string query = "SELECT * FROM userinfo WHERE userinfo.Id = '" + userID + "'";
                System.Data.Entity.Infrastructure.DbSqlQuery<userinfo> userInfos = db.userinfoes.SqlQuery(query);

                userinfo UserInfo = null;
                if (userInfos != null) UserInfo = await userInfos.SingleOrDefaultAsync();

                if (UserInfo != null && UserInfo.Private_Gallery_Pics != null) GalleryPics = UserInfo.Private_Gallery_Pics;

                string PictureDirectory = Server.MapPath("~/Content/UserPicture/Private/");
                PictureDirectory += userID;
                foreach (String name in gallery)
                {
                    if (System.IO.File.Exists(PictureDirectory + "/" + name))
                    {
                        System.IO.File.Delete(PictureDirectory + "/" + name);
                    }
                    GalleryPics = GalleryPics.Replace(name, "");
                }
                string pattern = ",+";
                Regex rgx = new Regex(pattern);
                GalleryPics = rgx.Replace(GalleryPics, ",");
                db.Database.ExecuteSqlCommand("UPDATE userinfo SET Private_Gallery_Pics = {0} where Id={1}", GalleryPics, userID);
            }

            ViewBag.mode = 2;
            return View("ProfileView", (object)GalleryPics);
        }
    }
}