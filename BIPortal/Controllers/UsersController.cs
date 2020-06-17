using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BIPortal.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        public ActionResult AddUser()
        {
            ViewBag.Message = "Add User Page";
            return View();
        }

        public ActionResult ViewUser()
        {
            ViewBag.Message = "View User Page";
            return View();
        }

        public ActionResult EditUser()
        {
            ViewBag.Message = "Edit User Page";
            return View();
        }
    }
}