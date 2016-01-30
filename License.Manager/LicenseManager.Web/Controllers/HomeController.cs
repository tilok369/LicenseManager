using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using License.Manager.Db;
using License.Manager.Wrapper;
using License.Manager.Wrapper.Model;

namespace LicenseManager.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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

        [HttpPost]
        public JsonResult CreateLicense(string name, string email)
        {
            var connString = ConfigurationManager.ConnectionStrings["LicDbConn"].ConnectionString;
            var portableLicense = new PortableLicense();
            var licFile = "license_" + Guid.NewGuid().ToString().Substring(0, 10).Replace("-", "") +".lic";
            portableLicense.GenerateLicense(new ProdCustomer { Email = email, Name = name },
                Guid.NewGuid().ToString().Substring(0, 8), Path.Combine(Server.MapPath("~/Licenses"), licFile), 2);
            var file = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath("~/Licenses"), licFile));
            var db = new DbUtility(connString);
            var key = portableLicense.LicKey;
            var saved = db.SaveLicense(new LicenseModel
            {
                LicFile = file,
                LicId = key,
                PublicKey = portableLicense.PublicKey
            });
            if (saved)
            {
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/Licenses"), licFile));
                return Json(new {Success = true, Key = key});
            }
            else
                return Json(new { Success = false, Key = "" });
        }
    }
}