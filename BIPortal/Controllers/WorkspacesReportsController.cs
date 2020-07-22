using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BIPortal.DTO;
using BIPortal.Models;
using AutoMapper;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;

namespace BIPortal.Controllers
{
    public class WorkspacesReportsController : Controller
    {
        // GET: WorkspacesReports
        //public ActionResult Index()
        //{
        //    ViewBag.Message = "WorkSpaces and Reports Page";
        //    IEnumerable<WorkspaceModel> workspacesList = null;

        //    //Hosted web API REST Service base url  
        //    string Baseurl = ConfigurationManager.AppSettings["baseURL"];

        //    using (var client = new HttpClient())
        //    {
        //        //Passing service base url  
        //        client.BaseAddress = new Uri(Baseurl);
        //        client.DefaultRequestHeaders.Clear();
        //        //Define request data format  
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
        //        //HttpResponseMessage Res = await client.GetAsync("api/Employee/GetAllEmployees");

        //        //var responseTask = client.GetAsync("api/workspaces/GetPowerBIWorkspace");
        //        var responseTask = client.GetAsync("api/GetWorkSpaces");
        //        responseTask.Wait();
        //        var result = responseTask.Result;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = result.Content.ReadAsAsync<List<WorkspaceDTO>>();
        //            readTask.Wait();

        //            var config = new MapperConfiguration(cfg => {
        //                cfg.CreateMap<WorkspaceDTO, WorkspaceModel>();
        //                cfg.CreateMap<ReportsDTO, ReportsModel>();
        //            });
        //            IMapper mapper = config.CreateMapper();

        //            workspacesList = mapper.Map<List<WorkspaceDTO>, List<WorkspaceModel>>(readTask.Result);
        //        }
        //    }
        //    return View(workspacesList);
        //}

        public ActionResult Index()
        {
            ViewBag.Message = "WorkSpaces and Reports Page";
            if (Session["UserName"] == null || Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            IEnumerable<WorkSpaceOwnerModel> workspaceOwnerList = null;

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync("api/GetWorkSpaceOwner");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<WorkSpaceOwnerDTO>>();
                    readTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<WorkSpaceOwnerDTO, WorkSpaceOwnerModel>();
                        //cfg.CreateMap<RoleRightsMappingDTO, RoleRightsMappingModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    workspaceOwnerList = mapper.Map<List<WorkSpaceOwnerDTO>, List<WorkSpaceOwnerModel>>(readTask.Result);
                }
            }
            return View(workspaceOwnerList);
        }

        //To get reports and Owner for a given workspaceid
        public JsonResult GetReportsAndOwner(string workspaceid)
        {
            //List<RoleRightsMappingModel> roleRights = new List<RoleRightsMappingModel>();
            var reportsModel = new ReportsModel();

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync(string.Format("api/GetReportsAndOwner/?workspaceid={0}", workspaceid));
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ReportsDTO>();
                    readTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<ReportsDTO, ReportsModel>();
                        cfg.CreateMap<UsersDTO, UsersModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    reportsModel = mapper.Map<ReportsDTO, ReportsModel>(readTask.Result);
                }
            }       
            
            return new JsonResult { Data = reportsModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };           
        }

        [HttpPost]
        public ActionResult SaveWorkspaceOwner(WorkSpaceOwnerModel WorkspaceOwnerdata)
        {
            var loggedinUser = Session["UserName"].ToString();

            WorkSpaceOwnerModel workspaceOwnerModel = new WorkSpaceOwnerModel()
            {
                WorkspaceID = WorkspaceOwnerdata.WorkspaceID,
                OwnerID = WorkspaceOwnerdata.OwnerID,
                CreatedDate = DateTime.Now,
                CreatedBy = loggedinUser,
                ModifiedDate = DateTime.Now,
                ModifiedBy = loggedinUser,
                Active = true
            };

            string Baseurl = ConfigurationManager.AppSettings["baseURL"] + "api/SaveWorkspaceOwner";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP POST
                var postTask = client.PostAsJsonAsync<WorkSpaceOwnerModel>(Baseurl, workspaceOwnerModel);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
    }
}