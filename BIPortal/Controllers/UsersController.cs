using AutoMapper;
using BIPortal.Data;
using BIPortal.DTO;
using BIPortal.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BIPortal.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        
        public ActionResult AddUser()
        {
            ViewBag.Message = "Add Users Page";

            if (Session["UserName"] == null || Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            //saluation
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Mr.", Value = "1", Selected = true });
            items.Add(new SelectListItem { Text = "Miss.", Value = "2" });
            items.Add(new SelectListItem { Text = "Mrs.", Value = "3" });
            items.Add(new SelectListItem { Text = "Ms.", Value = "4" });

            IEnumerable<PermissionMasterModel> PermissionTypeList = null;
            IEnumerable<RolesModel> rolesList = null;

            //string Baseurl = "https://localhost:44383/";
            string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync("api/GetPermissionTypes");
                responseTask.Wait();

                //Permission Type List
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<PermissionMasterDTO>>();
                    readTask.Wait();
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<PermissionMasterDTO, PermissionMasterModel>();
                    });
                    IMapper mapper = config.CreateMapper();
                    PermissionTypeList = mapper.Map<List<PermissionMasterDTO>, List<PermissionMasterModel>>(readTask.Result);
                }

                //Roles List
                var rolesResponseTask = client.GetAsync("api/GetRoles");
                rolesResponseTask.Wait();
                var rolesresult = rolesResponseTask.Result;
                if (rolesresult.IsSuccessStatusCode)
                {
                    var readRolesTask = rolesresult.Content.ReadAsAsync<List<RolesDTO>>();
                    readRolesTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<RolesDTO, RolesModel>();
                        cfg.CreateMap<RoleRightsMappingDTO, RoleRightsMappingModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    rolesList = mapper.Map<List<RolesDTO>, List<RolesModel>>(readRolesTask.Result);
                }

            }


            ViewBag.Salutation = new SelectList(items, "Text", "Text"); 
            ViewBag.PermissionTypes = new SelectList(PermissionTypeList.ToList(), "PermissionID", "PermissionName");
            ViewBag.Roles = new SelectList(rolesList.ToList(), "RoleID", "RoleName");


            return View();
        }

        // GET: MyProfile
        public ActionResult MyProfile()
        {
            ViewBag.Message = "My Profile Page";
            IEnumerable<UsersModel> UsersList = null;

            if (Session["UserName"] == null)
            {
                return View();
            }
            else
            {
                var emailID = Session["UserName"].ToString();

                string Baseurl = ConfigurationManager.AppSettings["baseURL"];

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //var responseTask = client.GetAsync("api/GetCurrentUser");
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

                //ViewBag.Name = "Selva";
                //ViewBag.EmailID = UsersList.Select(l => l.EmailID).ToString();

                foreach (var item in UsersList)
                {
                    ViewBag.EmailID = item.EmailID;
                    ViewBag.FirstName = item.FirstName;
                    ViewBag.LastName = item.LastName;
                    //ViewBag.Creationdate = item.CreatedDate;
                    ViewBag.Creationdate = item.CreatedDate.ToString("dd/MM/yyyy");
                    ViewBag.PermissionType = item.PermissionMaster.PermissionName;
                    ViewBag.Saluation = item.Salutation;
                }

                return View(UsersList);
            }
        }


        public ActionResult ViewUser()
        {
            ViewBag.Message = "View Users Page";


            if (Session["UserName"] == null || Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            IEnumerable<UsersModel> UsersList = null;


            //string Baseurl = "https://localhost:44383/";
            string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync("api/GetUsers");
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


            return View(UsersList);
        }

        
        public ActionResult LoadEditUser(int UserId)
        {
            ViewBag.Message = "Edit User Page";

            if (Session["UserName"] == null || Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            //saluation
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Mr.", Value = "1", Selected = true });
            items.Add(new SelectListItem { Text = "Miss.", Value = "2" });
            items.Add(new SelectListItem { Text = "Mrs.", Value = "3" });
            items.Add(new SelectListItem { Text = "Ms.", Value = "4" });

            IEnumerable<UsersModel> UsersList = null;
            IEnumerable<PermissionMasterModel> PermissionTypeList = null;
            List<RolesModel> rolesList = null;

            var LoadEditUserModel = new UsersModel();

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //var responseTask = client.GetAsync("api/GetSelectedUser");
                var responseTask = client.GetAsync(string.Format("api/GetSelectedUser/?iUSERID={0}", UserId));
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

                //Permission Type List
                var permisResponseTask = client.GetAsync("api/GetPermissionTypes");
                permisResponseTask.Wait();
                var permissionResult = permisResponseTask.Result;
                if (permissionResult.IsSuccessStatusCode)
                {
                    var permisReadTask = permissionResult.Content.ReadAsAsync<List<PermissionMasterDTO>>();
                    permisReadTask.Wait();
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<PermissionMasterDTO, PermissionMasterModel>();
                    });
                    IMapper mapper = config.CreateMapper();
                    PermissionTypeList = mapper.Map<List<PermissionMasterDTO>, List<PermissionMasterModel>>(permisReadTask.Result);
                }


                //Roles List
                //var rolesResponseTask = client.GetAsync(string.Format("api/GetSelectedUserRoles/?iUSERID={0}", UserId));
                var rolesResponseTask = client.GetAsync("api/GetRoles");
                rolesResponseTask.Wait();
                var rolesresult = rolesResponseTask.Result;
                if (rolesresult.IsSuccessStatusCode)
                {
                    var readRolesTask = rolesresult.Content.ReadAsAsync<List<RolesDTO>>();
                    readRolesTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<RolesDTO, RolesModel>();
                        cfg.CreateMap<RoleRightsMappingDTO, RoleRightsMappingModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    rolesList = mapper.Map<List<RolesDTO>, List<RolesModel>>(readRolesTask.Result);
                }
            }

            

            foreach (var item in UsersList)
            {
                LoadEditUserModel.EmailID = item.EmailID;
                LoadEditUserModel.FirstName = item.FirstName;
                LoadEditUserModel.LastName = item.LastName;
                LoadEditUserModel.CreatedDate = item.CreatedDate;
                LoadEditUserModel.PermissionID = item.PermissionMaster.PermissionID;
                LoadEditUserModel.Salutation = item.Salutation;
                LoadEditUserModel.UserID = item.UserID;
                LoadEditUserModel.CreatedBy = item.CreatedBy;
                LoadEditUserModel.Active = item.Active;


                // existing roleselection 
                LoadEditUserModel.SelectedRolesValues = new int[item.UserRoleMappings.Count];
                for (int i = 0; i < item.UserRoleMappings.Count; i++)
                {
                    LoadEditUserModel.SelectedRolesValues[i] = item.UserRoleMappings[i].RoleID;
                }
            }

            ViewBag.Salutation = new SelectList(items, "Text", "Text");
            ViewBag.PermissionTypes = new SelectList(PermissionTypeList.ToList(), "PermissionID", "PermissionName");

            ViewBag.RoleSelection = new SelectList(rolesList.ToList(), "RoleID", "RoleName");

            return View(LoadEditUserModel);
            
        }



        //To get access rights for a given roleid
        public JsonResult GetRights(List<string> roleID)
        {
            List<RoleRightsMappingModel> roleRights = new List<RoleRightsMappingModel>();

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // concating roleid list to pass web api
                var roleIDList = roleID.Select(d => string.Format("roleID={0}", UrlEncode(d)));
                var query = string.Join("&", roleIDList);
                var uri = Baseurl + "api/GetUserRights?" + query;
                var responseTask = client.GetAsync(uri);

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
           // ViewBag.Json = (new JavaScriptSerializer()).Serialize(nodes);

            //var nodesJason = (new JavaScriptSerializer()).Serialize(nodes);
            return new JsonResult { Data = nodes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //To get access rights for given userid
        public JsonResult GetEditUserRights(List<string> roleID,int userID)
        {
            List<RoleRightsMappingModel> roleRights = new List<RoleRightsMappingModel>();

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // concating roleid list to pass web api
                var roleIDList = roleID.Select(d => string.Format("roleID={0}", UrlEncode(d)));
                var query = string.Join("&", roleIDList);
                //var uri = Baseurl + "api/GetUserRights?" + query;
                var uri = Baseurl + "api/GetEditUserRights?" + query + "&userID=" + userID;                
                var responseTask = client.GetAsync(uri);

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
            // ViewBag.Json = (new JavaScriptSerializer()).Serialize(nodes);

            //var nodesJason = (new JavaScriptSerializer()).Serialize(nodes);
            return new JsonResult { Data = nodes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        static string UrlEncode(string stringToEscape)
        {
            return stringToEscape != null ? Uri.EscapeDataString(stringToEscape)
                .Replace("!", "%21")
                .Replace("'", "%27")
                .Replace("(", "%28")
                .Replace(")", "%29")
                .Replace("*", "%2A") : string.Empty;
        }

        [HttpPost]
        public ActionResult AddUser(UsersModel AddUserModel)
        {
            string Baseurl = ConfigurationManager.AppSettings["baseURL"] + "/" + "api/AddNewUser";
            //string Baseurl = "https://localhost:44383/api/AddNewUser";

            var loggedinUser = Session["UserName"].ToString();

            var  treelist = Request.Form["selectedItems"];

            var treeViewModel = JsonConvert.DeserializeObject<List<TreeViewNode>>(treelist);

            List<UserAccessRightsModel> userAccessRightsList = new List<UserAccessRightsModel>();

            // Mapping UserAccessRights table
            foreach (var a in treeViewModel)
            {
                if (a.parent == "#")
                {
                    UserAccessRightsModel userAccesrightsvalues = new UserAccessRightsModel() // Mapping UserAccessRights table
                    {
                        WorkspaceID = a.id,
                        WorkspaceName = a.text,
                        ReportID = null,
                        ReportName = null,
                        //CreatedBy = "Selva",
                        CreatedBy = loggedinUser,
                        ModifiedBy = AddUserModel.EmailID,
                        CreatedDate = DateTime.Now,
                        Active = AddUserModel.Active

                    };
                    userAccessRightsList.Add(userAccesrightsvalues);
                }
                else
                {
                    UserAccessRightsModel userAccesrightsvalues = new UserAccessRightsModel()
                    {
                        ReportID = a.id,
                        ReportName = a.text,
                        //CreatedBy = "Selva",
                        CreatedBy = loggedinUser,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = AddUserModel.EmailID,
                        Active = AddUserModel.Active,
                        WorkspaceID = a.parent,
                        WorkspaceName = a.parenttext,
                    };
                    userAccessRightsList.Add(userAccesrightsvalues);
                }
            }


            // Mapping Workflowmaster and workflowdetails table

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
                        RequestFor = AddUserModel.EmailID, // email id text box
                        //RequestedBy = "Venkat",
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
                        RequestFor = AddUserModel.EmailID, // email id text box
                       // RequestedBy = "Venkat",
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
                        OwnerEmail = workspaceOnwer.UserMaster.EmailID, // ownere email id
                        RequestedBy = workFlowMasterList[i].RequestedBy,
                        RequestedDate = workFlowMasterList[i].RequestedDate,
                        Status = workFlowMasterList[i].Status,
                       // RequestFor = workFlowMasterList[i].RequestFor,
                        RequestFor = AddUserModel.EmailID,
                        WorkFlowDetails = workFlowDetailsList
                    };
                    workspaces.Add(workFlowModel);
                }
            }


            UsersModel userModel = new UsersModel()
            {
                Salutation = AddUserModel.Salutation,
                FirstName = AddUserModel.FirstName,
                LastName = AddUserModel.LastName,
                EmailID = AddUserModel.EmailID,
                PermissionID = AddUserModel.PermissionID,
                CreatedDate = DateTime.Now,
                //CreatedBy = "SK",
                CreatedBy = loggedinUser,
                ModifiedDate = DateTime.Now,
                Active = AddUserModel.Active,
                SelectedRolesValues = AddUserModel.SelectedRolesValues,
                UserAccessRightsMappings = userAccessRightsList,
                WorkFlowMasterMappings = workspaces
                
            };



            using (var client = new HttpClient())
            {

                //HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP POST
                //var postTask = client.PostAsJsonAsync<UsersModel>(Baseurl,AddUserModel);
                var postTask = client.PostAsJsonAsync<UsersModel>(Baseurl, userModel);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    // email intergation
                    var subject = "New request created";
                    var link = ConfigurationManager.AppSettings["redirectUri"] + "PendingApprovals/ViewPendingApprovals";
                    var body = "Kindly approve the request by clicking following link.<br>" + "\n\n<a Href= " + link + "> Click here </a>";
                    //send email
                    string baseurl = ConfigurationManager.AppSettings["baseURL"] + "api/SendEmail";
                    foreach (var a in workspaces)
                    {
                        EmailModel emailModel = new EmailModel();
                        emailModel.ToEmail = a.OwnerEmail;
                        emailModel.Subject = subject;
                        emailModel.Body = body;
                        using (var client1 = new HttpClient())
                        {
                            client1.BaseAddress = new Uri(baseurl);
                            client1.DefaultRequestHeaders.Accept.Clear();
                            client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            //HTTP POST
                            var postEmailTask = client1.PostAsJsonAsync<EmailModel>(baseurl, emailModel);
                            postEmailTask.Wait();
                            var emailResult = postEmailTask.Result;
                            if (emailResult.IsSuccessStatusCode)
                            {
                               // return RedirectToAction("ViewUser");
                            }
                        }
                    }
                    return RedirectToAction("ViewUser");
                }

                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.CreateMap<UsersDTO, UsersModel>();
                //    cfg.CreateMap<UserRoleMappingDTO, UserRoleMappingModel>();
                //});
                //IMapper mapper = config.CreateMapper();

                //UsersRolemappingList = mapper.Map<List<UserRoleMappingDTO>, List<UserRoleMappingModel>>(postTask.Result);

            }

            

            return View(AddUserModel);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditUser(UsersModel EditUserModel)
        {
            string Baseurl = ConfigurationManager.AppSettings["baseURL"] + "api/UpdateUser";

            var loggedinUser = Session["UserName"].ToString();

            var treelist = Request.Form["selectedItems"];

            var treeViewModel = JsonConvert.DeserializeObject<List<TreeViewNode>>(treelist);

            List<UserAccessRightsModel> userAccessRightsList = new List<UserAccessRightsModel>();


            foreach (var a in treeViewModel)
            {
                if (a.parent == "#")
                {
                    UserAccessRightsModel userAccesrightsvalues = new UserAccessRightsModel() // Mapping UserAccessRights table
                    {
                        WorkspaceID = a.id,
                        WorkspaceName = a.text,
                        ReportID = null,
                        ReportName = null,
                        //CreatedBy = "Selva",
                        CreatedBy =  loggedinUser,
                        CreatedDate = DateTime.Now,
                        Active = EditUserModel.Active

                    };
                    userAccessRightsList.Add(userAccesrightsvalues);
                }
                else
                {
                    UserAccessRightsModel userAccesrightsvalues = new UserAccessRightsModel()
                    {
                        ReportID = a.id,
                        ReportName = a.text,
                        // CreatedBy = "Selva",
                        CreatedBy = loggedinUser,
                        CreatedDate = DateTime.Now,
                        Active = EditUserModel.Active,
                        WorkspaceID = a.parent,
                        WorkspaceName = a.parenttext,
                    };
                    userAccessRightsList.Add(userAccesrightsvalues);
                }
            }



            // Mapping UserAccessRights table
            foreach (var a in treeViewModel)
            {
                if (a.parent == "#")
                {
                    UserAccessRightsModel userAccesrightsvalues = new UserAccessRightsModel() // Mapping UserAccessRights table
                    {
                        WorkspaceID = a.id,
                        WorkspaceName = a.text,
                        ReportID = null,
                        ReportName = null,
                        //CreatedBy = "Selva",
                        CreatedBy = loggedinUser,
                        CreatedDate = DateTime.Now,
                        Active = EditUserModel.Active

                    };
                    userAccessRightsList.Add(userAccesrightsvalues);
                }
                else
                {
                    UserAccessRightsModel userAccesrightsvalues = new UserAccessRightsModel()
                    {
                        ReportID = a.id,
                        ReportName = a.text,
                        //CreatedBy = "Selva",
                        CreatedBy = loggedinUser,
                        CreatedDate = DateTime.Now,
                        Active = EditUserModel.Active,
                        WorkspaceID = a.parent,
                        WorkspaceName = a.parenttext,
                    };
                    userAccessRightsList.Add(userAccesrightsvalues);
                }
            }


            // Mapping Workflowmaster and workflowdetails table

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
                        //RequestedBy = "Venkat",
                        RequestedBy = loggedinUser,
                        RequestFor = EditUserModel.EmailID,
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
                        // RequestedBy = "Venkat",
                        RequestedBy = loggedinUser,
                        RequestFor = EditUserModel.EmailID,
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
                        OwnerEmail = workspaceOnwer.UserMaster.EmailID, // ownere email id
                        RequestedBy = workFlowMasterList[i].RequestedBy,
                        RequestedDate = workFlowMasterList[i].RequestedDate,
                        Status = workFlowMasterList[i].Status,
                        RequestFor = EditUserModel.EmailID,
                        WorkFlowDetails = workFlowDetailsList
                    };
                    workspaces.Add(workFlowModel);
                }
            }


            UsersModel updateUserModel = new UsersModel()
            {
                UserID = EditUserModel.UserID,
                Salutation = EditUserModel.Salutation,
                FirstName = EditUserModel.FirstName,
                LastName = EditUserModel.LastName,
                EmailID = EditUserModel.EmailID,
                PermissionID = EditUserModel.PermissionID,
                CreatedDate = DateTime.Now,
                //CreatedBy = "SK",
                CreatedBy = loggedinUser,
                ModifiedDate = DateTime.Now,
                Active = EditUserModel.Active,
                SelectedRolesValues = EditUserModel.SelectedRolesValues,
                UserAccessRightsMappings = userAccessRightsList,
                WorkFlowMasterMappings = workspaces
            };

            using (var client = new HttpClient())
            {
                //IEnumerable<UsersModel> AddUserList = null;


                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP POST
                var postTask = client.PostAsJsonAsync<UsersModel>(Baseurl, updateUserModel);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    // email intergation
                    var subject = "New request created";
                    var link = ConfigurationManager.AppSettings["redirectUri"] + "PendingApprovals/ViewPendingApprovals";
                    var body = "Kindly approve the request by clicking following link.<br>" + "\n\n<a Href= " + link + "> Click here </a>";
                    //send email
                    string baseurl = ConfigurationManager.AppSettings["baseURL"] + "api/SendEmail";
                    foreach (var a in workspaces)
                    {
                        EmailModel emailModel = new EmailModel();
                        emailModel.ToEmail = a.OwnerEmail;
                        emailModel.Subject = subject;
                        emailModel.Body = body;
                        using (var client1 = new HttpClient())
                        {
                            client1.BaseAddress = new Uri(baseurl);
                            client1.DefaultRequestHeaders.Accept.Clear();
                            client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            //HTTP POST
                            var postEmailTask = client1.PostAsJsonAsync<EmailModel>(baseurl, emailModel);
                            postEmailTask.Wait();
                            var emailResult = postEmailTask.Result;
                            if (emailResult.IsSuccessStatusCode)
                            {
                               // return RedirectToAction("ViewUser");
                            }
                        }
                    }
                    return RedirectToAction("ViewUser");
                }
            }
            return View(EditUserModel);
        }

    }
}








