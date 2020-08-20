using AutoMapper;
using BIPortal.DTO;
using BIPortal.Models;
using Newtonsoft.Json;
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
               nodes.Add(new TreeViewNode { id = workspace.WorkspaceID.ToString().Trim(), parent = "#", text = workspace.WorkspaceName.Trim(), parenttext = workspace.WorkspaceName.Trim(), type = "default", state = new TreeAttributes { id = workspace.WorkspaceID.ToString().Trim(), selected = true } });
               
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



        [HttpPost]
        public ActionResult SendRemainderMail(List<TreeViewNode> selectednodes)
        {
            List<WorkFlowMasterModel> pendingApprovals = new List<WorkFlowMasterModel>();

            //List<TreeViewNode> nodes = new List<TreeViewNode>();

            var treelist = Request.Form["selectedItems"];

            var loggedinUser = Session["UserName"].ToString();

            List<WorkFlowMasterModel> workFlowMasterList = new List<WorkFlowMasterModel>();

            foreach (var a in selectednodes)
            {
                if (a.parent == "#")
                {
                    WorkFlowMasterModel workFlowMasterValues = new WorkFlowMasterModel()
                    {
                        WorkspaceID = a.id,
                        WorkspaceName = a.text,
                        ReportID = null,
                        ReportName = null,
                        RequestFor = loggedinUser,
                        RequestedBy = loggedinUser,
                        RequestedDate = DateTime.Now,
                        Status = "PENDING"
                    };
                    workFlowMasterList.Add(workFlowMasterValues);
                }
                else
                {
                    WorkFlowMasterModel workFlowMasterValues = new WorkFlowMasterModel()
                    {
                        WorkspaceID = a.parent,
                        WorkspaceName = a.parenttext,
                        ReportID = a.id,
                        ReportName = a.text,
                        RequestFor = loggedinUser,
                        RequestedBy = loggedinUser,
                        RequestedDate = DateTime.Now,
                        Status = "PENDING"
                    };
                    workFlowMasterList.Add(workFlowMasterValues);
                }
            }



            List<WorkFlowMasterModel> workspaces = new List<WorkFlowMasterModel>();

            for (int i = 0; i < workFlowMasterList.Count; i++)
            {
                bool duplicate = false;
                for (int z = 0; z < i; z++)
                {
                    if (workFlowMasterList[z].WorkspaceID == workFlowMasterList[i].WorkspaceID)
                    {
                        duplicate = true;
                        break;
                    }
                }
                if (!duplicate)
                {
                    List<WorkFlowDetailsModel> workFlowDetailsList = new List<WorkFlowDetailsModel>();
                    foreach (var b in workFlowMasterList)
                    {
                        if (b.ReportID != null && b.WorkspaceID == workFlowMasterList[i].WorkspaceID)
                        {
                            WorkFlowDetailsModel workFlowDetails = new WorkFlowDetailsModel()
                            {
                                ReportID = b.ReportID,
                                ReportName = b.ReportName,
                                RequestedDate = b.RequestedDate,
                                Status = b.Status
                            };
                            workFlowDetailsList.Add(workFlowDetails);
                        }
                    }
                    //to get the workspace owner for a given workspace
                    WorkSpaceOwnerModel workspaceOnwer = new WorkSpaceOwnerModel();
                    string baseurl = ConfigurationManager.AppSettings["baseURL"];
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseurl);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var responseTask = client.GetAsync(string.Format("api/GetWorkSpaceOwnerByWorkspaceId/?workspaceId={0}", workFlowMasterList[i].WorkspaceID));
                        responseTask.Wait();

                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<WorkSpaceOwnerDTO>();
                            readTask.Wait();

                            var config = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<WorkSpaceOwnerDTO, WorkSpaceOwnerModel>();
                                cfg.CreateMap<UsersDTO, UsersModel>();
                                cfg.CreateMap<PermissionMasterDTO, PermissionMasterModel>();
                                cfg.CreateMap<UserRoleMappingDTO, UserRoleMappingModel>();
                            });
                            IMapper mapper = config.CreateMapper();

                            workspaceOnwer = mapper.Map<WorkSpaceOwnerDTO, WorkSpaceOwnerModel>(readTask.Result);
                            if (workspaceOnwer == null)
                            {
                                ViewBag.ErrorMessage = "Workspace Owner is not assigned to the workspace " + workFlowMasterList[i].WorkspaceName + ",kindly contact Admin";
                                List<TreeViewNode> nodes = new List<TreeViewNode>();
                                nodes = (List<TreeViewNode>)Session["Nodes"];
                                ViewBag.Json = (new JavaScriptSerializer()).Serialize(nodes);

                                return View();
                            }
                        }
                    }

                    WorkFlowMasterModel workFlowModel = new WorkFlowMasterModel()
                    {
                        WorkspaceID = workFlowMasterList[i].WorkspaceID,
                        WorkspaceName = workFlowMasterList[i].WorkspaceName,
                        OwnerID = workspaceOnwer.OwnerID,
                        OwnerEmail = workspaceOnwer.UserMaster.EmailID,
                        RequestFor = workFlowMasterList[i].RequestFor,
                        RequestedBy = workFlowMasterList[i].RequestedBy,
                        RequestedDate = workFlowMasterList[i].RequestedDate,
                        Status = workFlowMasterList[i].Status,
                        WorkFlowDetails = workFlowDetailsList
                    };
                    workspaces.Add(workFlowModel);

                }
            }

            var subject = "Remainder to approve the request";
            var link = ConfigurationManager.AppSettings["redirectUri"] + "PendingApprovals/ViewPendingApprovals";
            var body = "Kindly approve the request by clicking following link.<br>" + "\n\n<a Href= " + link + "> Click here </a>";
            //send email
            string mailbaseurl = ConfigurationManager.AppSettings["baseURL"] + "api/SendEmail";
            foreach (var a in workspaces)
            {
                EmailModel emailModel = new EmailModel();
                emailModel.ToEmail = a.OwnerEmail;
                emailModel.Subject = subject;
                emailModel.Body = body;
                using (var client1 = new HttpClient())
                {
                    client1.BaseAddress = new Uri(mailbaseurl);
                    client1.DefaultRequestHeaders.Accept.Clear();
                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //HTTP POST
                    var postEmailTask = client1.PostAsJsonAsync<EmailModel>(mailbaseurl, emailModel);
                    postEmailTask.Wait();
                    var emailResult = postEmailTask.Result;
                    if (emailResult.IsSuccessStatusCode)
                    {
                        //return RedirectToAction("CreateNewRequest");
                    }
                }
            }


            return View();
        }
    }
}