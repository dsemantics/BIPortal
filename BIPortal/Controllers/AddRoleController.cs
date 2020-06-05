using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BIPortal.Controllers
{
    public class AddRoleController : Controller
    {
        // GET: AddRole
        public ActionResult Index()
        {
            ViewBag.Message = "Add Role";
            return View();
        }
    }
}