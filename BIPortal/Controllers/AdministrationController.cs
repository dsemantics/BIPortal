using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BIPortal.Controllers
{
    public class AdministrationController : Controller
    {
        // GET: Administration
        public ActionResult Index()
        {
            ViewBag.Message = "Administration Page";
            return View();
        }
    }
}