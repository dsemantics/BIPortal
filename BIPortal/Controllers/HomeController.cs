using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BIPortalServices;
using BIPortal.DTO;
using BIPortal.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using AutoMapper;
using WebGrease;
using BIPortal.Mapping;

namespace BIPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMapper mapper;

        public HomeController(IMapper mapper)
            {
            this.mapper = mapper;
            }

        public HomeController()
        {

        }
        
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

    }
}