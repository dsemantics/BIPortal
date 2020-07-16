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

namespace BIPortal.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        
        public ActionResult AddUser()
        {
            ViewBag.Message = "Add Users Page";

            

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

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync("api/GetCurrentUser");
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


        public ActionResult ViewUser()
        {
            ViewBag.Message = "View Users Page";
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

                //foreach (var rolesitem in item.UserRoleMappings)
                //{
                //     rolesitem.RoleID ;
                //}
                //rolesList.Add(item.SelectedRolesValues.ToList());
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


            var  treelist = Request.Form["selectedItems"];

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
                        CreatedBy = "Selva",
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
                        CreatedBy = "Selva",
                        CreatedDate = DateTime.Now,
                        Active = AddUserModel.Active,
                        WorkspaceID = a.parent,
                        WorkspaceName = a.parenttext,
                    };
                    userAccessRightsList.Add(userAccesrightsvalues);
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
                CreatedBy = "SK",
                ModifiedDate = DateTime.Now,
                Active = AddUserModel.Active,
                SelectedRolesValues = AddUserModel.SelectedRolesValues,
                UserAccessRightsMappings = userAccessRightsList
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
                    //return RedirectToAction("AddUser");
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
                        CreatedBy = "Selva",
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
                        CreatedBy = "Selva",
                        CreatedDate = DateTime.Now,
                        Active = EditUserModel.Active,
                        WorkspaceID = a.parent,
                        WorkspaceName = a.parenttext,
                    };
                    userAccessRightsList.Add(userAccesrightsvalues);
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
                CreatedBy = "SK",
                ModifiedDate = DateTime.Now,
                Active = EditUserModel.Active,
                SelectedRolesValues = EditUserModel.SelectedRolesValues,
                UserAccessRightsMappings = userAccessRightsList
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
                    //return RedirectToAction("AddUser");
                    return RedirectToAction("ViewUser");
                }

            }

            return View(EditUserModel);
        }

    }
}








