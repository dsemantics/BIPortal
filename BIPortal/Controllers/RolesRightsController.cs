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
        public ActionResult AddRole()
        {
            ViewBag.Message = "AddRole Page";
            return View();
        }
        
        public ActionResult ViewRoles()
        {
            ViewBag.Message = "View Roles Page";
            return View();
        }
    }
}