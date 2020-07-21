using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AutoMapper;
using BIPortal.DTO;
using BIPortal.Models;

namespace BIPortal.Controllers
{
    public class PendingApprovalsController : Controller
    {
        // GET: ViewPendingApprovals
        public ActionResult ViewPendingApprovals()
        {
            List<WorkFlowMasterModel> pendingApprovals = new List<WorkFlowMasterModel>();

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync("api/GetPendingApprovals");
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

            return View(pendingApprovals);
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

        [HttpPost]        
        public ActionResult ApproveRequest(List<TreeViewNode> WorkspaceandReportList, int RequestID)
        {
            List<WorkFlowMasterModel> workFlowMasterList = new List<WorkFlowMasterModel>();

            foreach (var a in WorkspaceandReportList)
            {
                if (a.parent == "#")
                {
                    WorkFlowMasterModel workFlowMasterValues = new WorkFlowMasterModel()
                    {
                        RequestID = RequestID,
                        WorkspaceID = a.id,
                        WorkspaceName = a.text,
                        ReportID = null,
                        ReportName = null,
                        //RequestedBy = "Venkat",
                        ProcessedDate = DateTime.Now,
                        Status = "APPROVED"
                    };
                    workFlowMasterList.Add(workFlowMasterValues);
                }
                else
                {
                    WorkFlowMasterModel workFlowMasterValues = new WorkFlowMasterModel()
                    {
                        RequestID = RequestID,
                        WorkspaceID = a.parent,
                        WorkspaceName = a.parenttext,
                        ReportID = a.id,
                        ReportName = a.text,
                        //RequestedBy = "Venkat",
                        ProcessedDate = DateTime.Now,
                        Status = "APPROVED"
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
                                RequestID = b.RequestID,
                                ReportID = b.ReportID,
                                ReportName = b.ReportName,
                                //RequestedDate = b.RequestedDate,
                                ProcessedDate = b.ProcessedDate,
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
                        }
                    }

                    WorkFlowMasterModel workFlowModel = new WorkFlowMasterModel()
                    {
                        RequestID = workFlowMasterList[i].RequestID,
                        WorkspaceID = workFlowMasterList[i].WorkspaceID,
                        WorkspaceName = workFlowMasterList[i].WorkspaceName,
                        OwnerID = workspaceOnwer.OwnerID,
                        //RequestedBy = workFlowMasterList[i].RequestedBy,
                        //RequestedDate = workFlowMasterList[i].RequestedDate,
                        ProcessedDate = workFlowMasterList[i].ProcessedDate,
                        Status = workFlowMasterList[i].Status,
                        WorkFlowDetails = workFlowDetailsList
                    };
                    workspaces.Add(workFlowModel);
                }
            }


            string Baseurl = ConfigurationManager.AppSettings["baseURL"] + "api/ApproveRequest";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP POST
                var postTask = client.PostAsJsonAsync<List<WorkFlowMasterModel>>(Baseurl, workspaces);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("ViewPendingApprovals");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult RejectRequest(List<TreeViewNode> WorkspaceandReportList, int RequestID)
        {
            List<WorkFlowMasterModel> workFlowMasterList = new List<WorkFlowMasterModel>();

            foreach (var a in WorkspaceandReportList)
            {
                if (a.parent == "#")
                {
                    WorkFlowMasterModel workFlowMasterValues = new WorkFlowMasterModel()
                    {
                        RequestID = RequestID,
                        WorkspaceID = a.id,
                        WorkspaceName = a.text,
                        ReportID = null,
                        ReportName = null,
                        //RequestedBy = "Venkat",
                        ProcessedDate = DateTime.Now,
                        Status = "REJECT"
                    };
                    workFlowMasterList.Add(workFlowMasterValues);
                }
                else
                {
                    WorkFlowMasterModel workFlowMasterValues = new WorkFlowMasterModel()
                    {
                        RequestID = RequestID,
                        WorkspaceID = a.parent,
                        WorkspaceName = a.parenttext,
                        ReportID = a.id,
                        ReportName = a.text,
                        //RequestedBy = "Venkat",
                        ProcessedDate = DateTime.Now,
                        Status = "REJECT"
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
                                RequestID = b.RequestID,
                                ReportID = b.ReportID,
                                ReportName = b.ReportName,
                                //RequestedDate = b.RequestedDate,
                                ProcessedDate = b.ProcessedDate,
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
                        }
                    }

                    WorkFlowMasterModel workFlowModel = new WorkFlowMasterModel()
                    {
                        RequestID = workFlowMasterList[i].RequestID,
                        WorkspaceID = workFlowMasterList[i].WorkspaceID,
                        WorkspaceName = workFlowMasterList[i].WorkspaceName,
                        OwnerID = workspaceOnwer.OwnerID,
                        //RequestedBy = workFlowMasterList[i].RequestedBy,
                        //RequestedDate = workFlowMasterList[i].RequestedDate,
                        ProcessedDate = workFlowMasterList[i].ProcessedDate,
                        Status = workFlowMasterList[i].Status,
                        WorkFlowDetails = workFlowDetailsList
                    };
                    workspaces.Add(workFlowModel);
                }
            }


            string Baseurl = ConfigurationManager.AppSettings["baseURL"] + "api/RejectRequest";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP POST
                var postTask = client.PostAsJsonAsync<List<WorkFlowMasterModel>>(Baseurl, workspaces);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("ViewPendingApprovals");
                }
            }
            return View();
        }
    }
}