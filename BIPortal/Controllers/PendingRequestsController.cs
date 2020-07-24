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
using System.Web.Script.Serialization;

namespace BIPortal.Controllers
{
    public class PendingRequestsController : Controller
    {
        // GET: PendingRequests
        public ActionResult ViewPendingRequests()
        {

            if (Session["UserName"] == null || Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var loggedinUser = Session["UserName"].ToString();

            List<WorkFlowMasterModel> pendingRequests = new List<WorkFlowMasterModel>();

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //var responseTask = client.GetAsync("api/GetUsersPendingApprovals");
                var responseTask = client.GetAsync(string.Format("api/GetUsersPendingApprovals/?emailID={0}", loggedinUser));


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

                    pendingRequests = mapper.Map<List<WorkFlowMasterDTO>, List<WorkFlowMasterModel>>(readTask.Result);
                }
            }

            return View(pendingRequests);
        }



        //To get the access rights
        public JsonResult GetRequestDetails(int requestId)
        {
            List<WorkFlowMasterModel> pendingApprovals = new List<WorkFlowMasterModel>();

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync(string.Format("api/GetRequestDetails/?requestID={0}", requestId));
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

                    pendingApprovals = mapper.Map<List<WorkFlowMasterDTO>, List<WorkFlowMasterModel>>(readTask.Result);
                }
            }


            List<TreeViewNode> nodes = new List<TreeViewNode>();

            //Loop and add the Parent Nodes.

            foreach (var workspace in pendingApprovals)
            {
                //if (workspace.ReportID == null)
                //{
                nodes.Add(new TreeViewNode { id = workspace.WorkspaceID.ToString().Trim(), parent = "#", text = workspace.WorkspaceName.Trim(), parenttext = workspace.WorkspaceName.Trim(), type = "default", state = new TreeAttributes { id = workspace.WorkspaceID.ToString().Trim(), selected = true } });
                //}
                //else
                //{
                //    nodes.Add(new TreeViewNode { id = workspace.WorkspaceID.ToString().Trim(), parent = "#", text = workspace.WorkspaceName.Trim(), parenttext = workspace.WorkspaceName.Trim(), type = "default" });
                //}
                //Loop and add the Child Nodes.
                foreach (var report in workspace.WorkFlowDetails)
                {
                    nodes.Add(new TreeViewNode
                    {
                        id = report.ReportID.ToString().Trim(),
                        parent = workspace.WorkspaceID.ToString().Trim(),
                        text = report.ReportName.Trim(),
                        parenttext = workspace.WorkspaceName.Trim(),
                        type = "element",
                        state = new TreeAttributes { id = report.ReportID.ToString().Trim(), selected = true }
                    });
                }
            }

            //Serialize to JSON string.
            ViewBag.Json = (new JavaScriptSerializer()).Serialize(nodes);

            var nodesJason = (new JavaScriptSerializer()).Serialize(nodes);
            return new JsonResult { Data = nodes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}