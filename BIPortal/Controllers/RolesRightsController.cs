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
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace BIPortal.Controllers
{
    public class RolesRightsController : Controller
    {
        //Load Addrole view
        [HttpGet]
        public ActionResult AddRole()
        {
            ViewBag.Message = "AddRole Page";
            if (Session["UserName"] == null || Session["UserID"] == null)
            {
                return RedirectToAction("Index","Login");
            }
            RolesModel rolesModel = new RolesModel();


            //List<WorkSpaceMasterModel> workspaces = new List<WorkSpaceMasterModel>();

            //string Baseurl = ConfigurationManager.AppSettings["baseURL"];
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(Baseurl);
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    var responseTask = client.GetAsync("api/GetWorkSpace");
            //    responseTask.Wait();

            //    var result = responseTask.Result;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        var readTask = result.Content.ReadAsAsync<List<WorkSpaceMasterDTO>>();
            //        readTask.Wait();

            //        var config = new MapperConfiguration(cfg =>
            //        {
            //            cfg.CreateMap<WorkSpaceMasterDTO, WorkSpaceMasterModel>();
            //            //cfg.CreateMap<RoleRightsMappingDTO, RoleRightsMappingModel>();
            //        });
            //        IMapper mapper = config.CreateMapper();

            //        workspaces = mapper.Map<List<WorkSpaceMasterDTO>, List<WorkSpaceMasterModel>>(readTask.Result);
            //    }
            //}


            //List<ReportsMasterModel> reports = new List<ReportsMasterModel>();

            ////string Baseurl = ConfigurationManager.AppSettings["baseURL"];
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(Baseurl);
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    var responseTask = client.GetAsync("api/GetReports");
            //    responseTask.Wait();

            //    var result = responseTask.Result;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        var readTask = result.Content.ReadAsAsync<List<ReportsMasterDTO>>();
            //        readTask.Wait();

            //        var config = new MapperConfiguration(cfg =>
            //        {
            //            cfg.CreateMap<ReportsMasterDTO, ReportsMasterModel>();
            //            //cfg.CreateMap<RoleRightsMappingDTO, RoleRightsMappingModel>();
            //        });
            //        IMapper mapper = config.CreateMapper();

            //        reports = mapper.Map<List<ReportsMasterDTO>, List<ReportsMasterModel>>(readTask.Result);
            //    }
            //}

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
                //nodes.Add(new TreeViewNode { id = workspace.WorkspaceID.ToString().Trim(), parent = "#", text = workspace.WorkspaceName.Trim() });
                nodes.Add(new TreeViewNode { id = workspace.WorkspaceID.ToString().Trim(), parent = "#", text = workspace.WorkspaceName.Trim(), parenttext = workspace.WorkspaceName.Trim() });
            }            

            //Loop and add the Child Nodes.
            foreach (var report in roleRights)
            {
                if (report.ReportID != null)
                {
                    //nodes.Add(new TreeViewNode { id = report.WorkspaceID.ToString().Trim() + "/" + report.ReportID.ToString().Trim(), parent = report.WorkspaceID.ToString().Trim(), text = report.ReportName.Trim() });
                    nodes.Add(new TreeViewNode { id = report.ReportID.ToString().Trim(), parent = report.WorkspaceID.ToString().Trim(), text = report.ReportName.Trim(), parenttext = report.WorkspaceName.Trim() });
                }               
            }

            //Serialize to JSON string.
            ViewBag.Json = (new JavaScriptSerializer()).Serialize(nodes);


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

            List<RoleRightsMappingModel> workspaces = new List<RoleRightsMappingModel>();

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
                if (workspace.ReportID == null)
                {
                    nodes.Add(new TreeViewNode { id = workspace.WorkspaceID.ToString().Trim(), parent = "#", text = workspace.WorkspaceName.Trim(), parenttext = workspace.WorkspaceName.Trim(), type = "default", state = new TreeAttributes { id = workspace.WorkspaceID.ToString().Trim(), selected = workspace.Active } });
                }
                else
                {
                    nodes.Add(new TreeViewNode { id = workspace.WorkspaceID.ToString().Trim(), parent = "#", text = workspace.WorkspaceName.Trim(), parenttext = workspace.WorkspaceName.Trim(), type = "default" });
                }
                
            }

            //Loop and add the Child Nodes.
            foreach (var report in roleRights)
            {
                if (report.ReportID != null)
                {
                    nodes.Add(new TreeViewNode
                    {
                        id = report.ReportID.ToString().Trim(),
                        parent = report.WorkspaceID.ToString().Trim(),
                        text = report.ReportName.Trim(),
                        parenttext = report.WorkspaceName.Trim(),
                        type = "element",
                        state = new TreeAttributes { id = report.ReportID.ToString().Trim(), selected = report.Active }
                    });
                }
            }

            //Serialize to JSON string.
            ViewBag.Json = (new JavaScriptSerializer()).Serialize(nodes);

            var nodesJason= (new JavaScriptSerializer()).Serialize(nodes);
            //return new JsonResult { Data = roleRights, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return new JsonResult { Data = nodes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Get workspaces and reports from powerBI
        //public JsonResult GetWorkSpaceReports()
        public ActionResult GetWorkSpaceReports()
        {
            //List<WorkspaceModel> workspacesList = null;

            //string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(Baseurl);
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //    var responseTask = client.GetAsync("api/GetWorkSpaces");
            //    responseTask.Wait();
            //    var result = responseTask.Result;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        var readTask = result.Content.ReadAsAsync<List<WorkspaceDTO>>();
            //        readTask.Wait();

            //        var config = new MapperConfiguration(cfg =>
            //        {
            //            cfg.CreateMap<WorkspaceDTO, WorkspaceModel>();
            //            cfg.CreateMap<ReportsDTO, ReportsModel>();
            //        });
            //        IMapper mapper = config.CreateMapper();

            //        workspacesList = mapper.Map<List<WorkspaceDTO>, List<WorkspaceModel>>(readTask.Result);
            //    }
            //}

            return View();

            //return new JsonResult { Data = workspacesList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //To show roles in View roles page
        public ActionResult ViewRoles()
        {
            ViewBag.Message = "View Roles Page";
            if (Session["UserName"] == null || Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

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

            //ViewBag.Json = (new JavaScriptSerializer()).Serialize(rolesList);
            //ViewBag.Json = string.Empty;

            return View(rolesList);
        }

        [HttpPost]
        //public ActionResult SaveRoleAndRights(List<RoleRightsMappingModel> WorkspaceandReportList, string RoleName)
        public ActionResult SaveRoleAndRights(List<TreeViewNode> WorkspaceandReportList, string RoleName)
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

            List<RoleRightsMappingModel> roleRightsMappingModelList = new List<RoleRightsMappingModel>();
            var loggedinUser = Session["UserName"].ToString();

            foreach (var a in WorkspaceandReportList)
            {
                if (a.parent == "#")
                {
                    RoleRightsMappingModel roleRightsMapping = new RoleRightsMappingModel()
                    {

                        WorkspaceID = a.id,
                        WorkspaceName = a.text,
                        ReportID = null,
                        ReportName = null,
                        CreatedDate = DateTime.Now,
                        CreatedBy = loggedinUser,
                        Active = true
                    };
                    roleRightsMappingModelList.Add(roleRightsMapping);
                }
                else
                {
                    RoleRightsMappingModel roleRightsMapping = new RoleRightsMappingModel()
                    {

                        WorkspaceID = a.parent,
                        WorkspaceName = a.parenttext,
                        ReportID = a.id,
                        ReportName = a.text,
                        CreatedDate = DateTime.Now,
                        CreatedBy = loggedinUser,
                        Active = true
                    };
                    roleRightsMappingModelList.Add(roleRightsMapping);
                }
            }

            RolesModel rolesModel = new RolesModel()
            {
                RoleName = RoleName,
                CreatedDate = DateTime.Now,
                CreatedBy = loggedinUser,
                Active = true,
                RoleRightsMappings = roleRightsMappingModelList
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
        //public ActionResult UpdateRoleAndRights(List<RoleRightsMappingModel> WorkspaceandReportList)
        public ActionResult UpdateRoleAndRights(List<TreeViewNode> WorkspaceandReportList, int RoleID)
        {
            var loggedinUser = Session["UserName"].ToString();

            List<RoleRightsMappingModel> roleRightsMappingModelList = new List<RoleRightsMappingModel>();

            foreach (var a in WorkspaceandReportList)
            {
                if (a.parent == "#")
                {
                    RoleRightsMappingModel roleRightsMapping = new RoleRightsMappingModel()
                    {
                        RoleID = RoleID,
                        WorkspaceID = a.id,
                        WorkspaceName = a.text,
                        ReportID = null,
                        ReportName = null,
                        CreatedDate = DateTime.Now,
                        CreatedBy = loggedinUser,
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = loggedinUser,
                        Active = true
                    };
                    roleRightsMappingModelList.Add(roleRightsMapping);
                }
                else
                {
                    RoleRightsMappingModel roleRightsMapping = new RoleRightsMappingModel()
                    {
                        RoleID = RoleID,
                        WorkspaceID = a.parent,
                        WorkspaceName = a.parenttext,
                        ReportID = a.id,
                        ReportName = a.text,
                        CreatedDate = DateTime.Now,
                        CreatedBy = loggedinUser,
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = loggedinUser,
                        Active = true
                    };
                    roleRightsMappingModelList.Add(roleRightsMapping);
                }
            }

            string Baseurl = ConfigurationManager.AppSettings["baseURL"] + "api/UpdateRoleAndRights";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP POST
                var postTask = client.PostAsJsonAsync<List<RoleRightsMappingModel>>(Baseurl, roleRightsMappingModelList);
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