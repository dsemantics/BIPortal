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
        //Load Addrole view
        [HttpGet]
        public ActionResult AddRole()
        {
            ViewBag.Message = "AddRole Page";
            RolesModel rolesModel = new RolesModel();
            return View(rolesModel);
        }

        //To get access rights for a given roleid
        public JsonResult GetRights(int roleId)
        {
            List<RoleRightsMappingModel> roleRights = new List<RoleRightsMappingModel>();

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync(string.Format("api/GetRights/?roleID={0}", roleId));
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<RoleRightsMappingDTO>>();
                    readTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<RolesDTO, RolesModel>();
                        cfg.CreateMap<RoleRightsMappingDTO, RoleRightsMappingModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    roleRights = mapper.Map<List<RoleRightsMappingDTO>, List<RoleRightsMappingModel>>(readTask.Result);
                }
            }

            return new JsonResult { Data = roleRights, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Get workspaces and reports from powerBI
        public JsonResult GetWorkSpaceReports()
        {
            List<WorkspaceModel> workspacesList = null;

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var responseTask = client.GetAsync("api/GetWorkSpaces");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<WorkspaceDTO>>();
                    readTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<WorkspaceDTO, WorkspaceModel>();
                        cfg.CreateMap<ReportsDTO, ReportsModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    workspacesList = mapper.Map<List<WorkspaceDTO>, List<WorkspaceModel>>(readTask.Result);
                }
            }

            return new JsonResult { Data = workspacesList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //To show roles in View roles page
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
                        cfg.CreateMap<RoleRightsMappingDTO, RoleRightsMappingModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    rolesList = mapper.Map<List<RolesDTO>, List<RolesModel>>(readTask.Result);
                }
            }

            return View(rolesList);
        }

        [HttpPost]
        public ActionResult SaveRoleAndRights(List<RoleRightsMappingModel> WorkspaceandReportList, string RoleName)
        {
            //RoleRightsViewModel roleRightsViewModel = new RoleRightsViewModel();

            //RolesModel roleModel = new RolesModel()
            //{
            //    RoleName = RoleName,
            //    CreatedBy="Venkat",
            //    CreatedDate = DateTime.Now
            //};
            //roleRightsViewModel.Roles = roleModel;

            //List<RoleRightsMappingModel> roleRightsMappingModel = new List<RoleRightsMappingModel>();
            //roleRightsMappingModel = WorkspaeandReportList;

            //roleRightsViewModel.RoleRightsMapping = roleRightsMappingModel;

            RolesModel rolesModel = new RolesModel()
            {
                RoleName = RoleName,
                RoleRightsMappings = WorkspaceandReportList
            };

            string Baseurl = ConfigurationManager.AppSettings["baseURL"] + "api/SaveRoleAndRights";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP POST
                var postTask = client.PostAsJsonAsync<RolesModel>(Baseurl, rolesModel);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("AddRole");
                }
            }
            return View();
        }
        [HttpPost]
        //public ActionResult UpdateRoleAndRights(List<RoleRightsMappingModel> WorkspaceandReportList, int RoleID)
        public ActionResult UpdateRoleAndRights(List<RoleRightsMappingModel> WorkspaceandReportList)
        {
            string Baseurl = ConfigurationManager.AppSettings["baseURL"] + "api/UpdateRoleAndRights";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                
                //HTTP POST
                var postTask = client.PostAsJsonAsync<List<RoleRightsMappingModel>>(Baseurl, WorkspaceandReportList);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("ViewRoles");
                }
            }

            return View();
        }
    }
}