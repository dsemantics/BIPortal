using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using BIPortal.Models;
using BIPortal.DTO;
using AutoMapper;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace BIPortal.Controllers
{
    public class CreateRequestController : Controller
    {
        // GET: CreateRequest        
        public ActionResult CreateNewRequest()
        {
            List<WorkspaceReportsModel> roleRights = new List<WorkspaceReportsModel>();

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync("api/GetWorkspacesAndReports");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<WorkspaceReportsDTO>>();
                    readTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<WorkspaceReportsDTO, WorkspaceReportsModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    roleRights = mapper.Map<List<WorkspaceReportsDTO>, List<WorkspaceReportsModel>>(readTask.Result);
                }
            }

            List<WorkspaceReportsModel> workspaces = new List<WorkspaceReportsModel>();

            for (int i = 0; i < roleRights.Count; i++)
            {
                bool duplicate = false;
                for (int z = 0; z < i; z++)
                {
                    if (roleRights[z].WorkspaceID == roleRights[i].WorkspaceID)
                    {
                        duplicate = true;
                        break;
                    }
                }
                if (!duplicate)
                {
                    workspaces.Add(roleRights[i]);
                }
            }


            List<TreeViewNode> nodes = new List<TreeViewNode>();

            //Loop and add the Parent Nodes.

            foreach (var workspace in workspaces)
            {
                nodes.Add(new TreeViewNode { id = workspace.WorkspaceID.ToString().Trim(), parent = "#", text = workspace.WorkspaceName.Trim(), parenttext = workspace.WorkspaceName.Trim() });
            }

            //Loop and add the Child Nodes.
            foreach (var report in roleRights)
            {
                if (report.ReportID != null)
                {
                    nodes.Add(new TreeViewNode { id = report.ReportID.ToString().Trim(), parent = report.WorkspaceID.ToString().Trim(), text = report.ReportName.Trim(), parenttext = report.WorkspaceName.Trim() });
                }
            }

            //Serialize to JSON string.
            ViewBag.Json = (new JavaScriptSerializer()).Serialize(nodes);
            Session["Nodes"] = nodes;
            return View();
        }

        [HttpPost]
        public ActionResult CreateNewRequest(WorkFlowMasterModel workFlowMasterModel)
        {
            var treelist = Request.Form["selectedItems"];

            var treeViewModel = JsonConvert.DeserializeObject<List<TreeViewNode>>(treelist);

            List<WorkFlowMasterModel> workFlowMasterList = new List<WorkFlowMasterModel>();

            foreach (var a in treeViewModel)
            {
                if (a.parent == "#")
                {
                    WorkFlowMasterModel workFlowMasterValues = new WorkFlowMasterModel() 
                    {
                        WorkspaceID = a.id,
                        WorkspaceName = a.text,
                        ReportID = null,
                        ReportName = null,
                        RequestedBy = "Venkat",
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
                        RequestedBy = "Venkat",
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
                        OwnerID=workspaceOnwer.OwnerID,
                        RequestedBy = workFlowMasterList[i].RequestedBy,
                        RequestedDate = workFlowMasterList[i].RequestedDate,
                        Status = workFlowMasterList[i].Status,
                        WorkFlowDetails = workFlowDetailsList
                    };
                    workspaces.Add(workFlowModel);
                }
            }           


            string Baseurl = ConfigurationManager.AppSettings["baseURL"] + "api/AddNewRequest";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP POST
                var postTask = client.PostAsJsonAsync<List<WorkFlowMasterModel>>(Baseurl, workspaces);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("CreateNewRequest");
                }
            }

            return View();
        }
    }
}