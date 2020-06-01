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

namespace BIPortal.Controllers
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

        public ActionResult Login()
        {
            ViewBag.Message = "Your Login page.";
            return View();
        }

        public ActionResult AddRole()
        {
            ViewBag.Message = "Add Role";
            return View();
        }

        public ActionResult Administration()
        {
            ViewBag.Message = "Administration Page";
            return View();
        }

        public ActionResult WorkSpaces_Reports()
        {
            ViewBag.Message = "WorkSpaces and Reports Page";
            IEnumerable<WorkspaceDTO> workspacesList = null;
            
            //Hosted web API REST Service base url  
            string Baseurl = "https://localhost:44383/";
            
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);               
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage Res = await client.GetAsync("api/Employee/GetAllEmployees");

                //var responseTask = client.GetAsync("api/workspaces/GetPowerBIWorkspace");
                var responseTask = client.GetAsync("api/GetWorkSpaces");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<WorkspaceDTO>>();
                    readTask.Wait();

                    workspacesList = readTask.Result;
                }
            }
                return View();

        }

        public ActionResult Roles_Rights()
        {
            ViewBag.Message = "Roles and Rights Page";
            return View();
        }

        public ActionResult Dashboard()
        {
            ViewBag.Message = "Dashboard Page";
            return View();
        }

    }
}