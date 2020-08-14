using AutoMapper;
using BIPortal.DTO;
using BIPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace BIPortal.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            ViewBag.Message = "Dashboard Page";

            List<UsersModel> UsersList = null;
            List<WorkFlowMasterModel> approvedWorkspaces = new List<WorkFlowMasterModel>();

            if (Session["UserName"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var emailID = Session["UserName"].ToString();

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync(string.Format("api/GetCurrentUser/?emailID={0}", emailID));
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<UsersDTO>>();
                    readTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UsersDTO, UsersModel>();
                        cfg.CreateMap<PermissionMasterDTO, PermissionMasterModel>();
                        cfg.CreateMap<UserRoleMappingDTO, UserRoleMappingModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    UsersList = mapper.Map<List<UsersDTO>, List<UsersModel>>(readTask.Result);
                }
            }
            if (UsersList.Count() == 0)
            {
                ViewBag.InfoMessage = "You don't have access to the portal, kindly contact Admin to enable";
            }
            else
            {
                Session["UserID"] = UsersList[0].UserID;                

                //string Baseurl = ConfigurationManager.AppSettings["baseURL"];
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var responseTask = client.GetAsync(string.Format("api/GetUsersWorkspaces/?emailID={0}", emailID));
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<List<WorkFlowMasterDTO>>();
                        readTask.Wait();

                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<WorkFlowMasterDTO, WorkFlowMasterModel>();
                            cfg.CreateMap<WorkFlowDetailsDTO, WorkFlowDetailsModel>();
                            cfg.CreateMap<UsersDTO, UsersModel>();
                            cfg.CreateMap<PermissionMasterDTO, PermissionMasterModel>();
                            cfg.CreateMap<UserRoleMappingDTO, UserRoleMappingModel>();
                        });
                        IMapper mapper = config.CreateMapper();

                        approvedWorkspaces = mapper.Map<List<WorkFlowMasterDTO>, List<WorkFlowMasterModel>>(readTask.Result);
                    }
                }
            }

            return View(approvedWorkspaces);
            //return View();
        }

        //To get reports and embeded url for a given workspaceid
        public JsonResult GetReportsAndEmbedUrl(string requestId)
        {
            //List<RoleRightsMappingModel> roleRights = new List<RoleRightsMappingModel>();
            var reportsModel = new ReportsModel();

            if (Session["UserName"] == null)
            {
                return Json(new { code = 1 });
            }

            var loginModel = new LoginModel();
            loginModel.UserName = Session["UserName"].ToString();
            loginModel.Password = Session["Pwd"].ToString();

            string Baseurl = ConfigurationManager.AppSettings["baseURL"] + "api/AuthenticatePowerBIUser";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP POST
                var postTask = client.PostAsJsonAsync<LoginModel>(Baseurl, loginModel);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<string>();
                    readTask.Wait();
                    if (readTask.Result == "PowerBI Authentication failed")
                    {
                        ViewBag.ErrorMessage = "The Email and/or Password are incorrect";
                    }
                    else
                    {
                        Session["PowerBIAccessToken"] = readTask.Result;                        
                    }
                }
            }


            Baseurl = ConfigurationManager.AppSettings["baseURL"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync(string.Format("api/GetReportsAndEmbedUrl/?requestId={0}", requestId));
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
                    if (Session["PowerBIAccessToken"] != null)
                        reportsModel.PowerBIAccessToken = Session["PowerBIAccessToken"].ToString();
                }
            }

            return new JsonResult { Data = reportsModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}