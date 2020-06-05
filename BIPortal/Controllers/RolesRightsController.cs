using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BIPortal.Controllers
{
    public class RolesRightsController : Controller
    {
        // GET: RolesRights
        public ActionResult Index()
        {
            ViewBag.Message = "Roles and Rights Page";
            return View();
        }
    }
}