using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using BIPortal.Models;
using BIPortal.DTO;
using AutoMapper;
using System.Configuration;

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
            IEnumerable<RolesModel> rolesList = null;

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync("api/GetRoles");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<RolesDTO>>();
                    readTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<RolesDTO, RolesModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    rolesList = mapper.Map<List<RolesDTO>, List<RolesModel>>(readTask.Result);
                }
            }

            return View(rolesList);
        }
    }
}