using AutoMapper;
using BIPortal.Data;
using BIPortal.Data.Roles;
using BIPortal.Data.Users;
using BIPortal.DTO;
using BIPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace BIPortalServices.Controllers
{
    public class UsersController : ApiController
    {

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/AddNewUser")]
        public IHttpActionResult AddNewUser(UsersModel AddUserModel)
        {
             try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest("Not a valid model");
                }
                else
                {
                    UsersData SaveUserData = new UsersData();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UsersModel, UsersDTO>();
                        cfg.CreateMap<UserRoleMappingModel, UserRoleMappingDTO>();
                        cfg.CreateMap<WorkFlowMasterModel, WorkFlowMasterDTO>();
                        cfg.CreateMap<WorkFlowDetailsModel, WorkFlowDetailsDTO>();
                        cfg.CreateMap<UserAccessRightsModel, UserAccessRightsDTO>();
                    });
                    IMapper mapper = config.CreateMapper();

                    var userData = mapper.Map<UsersModel, UsersDTO>(AddUserModel);

                    SaveUserData.SaveUsersData(userData);

                    return Created("api/AddNewUser", true);
                }


               

                //using (var ctx = new BIPortalEntities())
                //{
                //    ctx.UserMasters.Add(new UserMaster
                //    {
                //        Salutation = "Mr.",
                //        FirstName = AddUserModel.FirstName,
                //        LastName = AddUserModel.LastName,
                //        EmailID = AddUserModel.EmailID,
                //        PermissionID = 1,
                //        //CreatedDate = DateTime.Now,
                //        CreatedDate = AddUserModel.CreatedDate,
                //        CreatedBy = "SK",
                //        ModifiedDate = DateTime.Now,
                //        Active = AddUserModel.Active
                //    });
                //    ctx.SaveChanges();
                //}

                //return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest("Can't add new User");
            }
        }


        [Route("api/GetUsers")]
        public IHttpActionResult GetUsers()
        {
            try
            {
                UsersData usersData = new UsersData();
                var users = usersData.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }

       
        [Route("api/GetCurrentUser")]
        public IHttpActionResult GetCurrentUser(string emailID)
        {
            try
            {
                string sCurrentUserDetail = emailID;
                UsersData usersData = new UsersData();
                var users = usersData.GetCurrentUser(sCurrentUserDetail);

                return Ok(users);


            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }

        [Route("api/GetSelectedUser")]
        public IHttpActionResult GetSelectedUser(int iUSERID)
        {
            try
            {
                UsersData usersData = new UsersData();
                var users = usersData.GetSeletedUser(iUSERID);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }

        //[Route("api/GetSelectedUserRoles")]
        //public IHttpActionResult GetSelectedUserRoles(int iUSERID)
        //{
        //    try
        //    {
        //        UsersData usersData = new UsersData();
        //        var users = usersData.GetSelectedUserRoles(iUSERID);
        //        return Ok(users);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Could not fetch roles");
        //    }
        //}

        [Route("api/GetPendingRequests")]
        public IHttpActionResult GetPendingRequests()
        {
            try
            {
                UsersData usersData = new UsersData();
                var users = usersData.GetPendingRequests();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }

        [Route("api/GetUserRights")]
        //To get access rights
        public IHttpActionResult GetUserRights([FromUri] List<string> roleID)
        {
            try
            {

                RolesData roleRightsData = new RolesData();
                UsersData addUserData = new UsersData();
                var rights = addUserData.GetRights(roleID);

                var workspaceReports = roleRightsData.GetWorkspacesAndReports();

                //WorkSpaceData workSpaceData = new WorkSpaceData();
                //var workSpaceAndReports = workSpaceData.GetPowerBIWorkspace();

                List<RoleRightsMappingDTO> roleRightsMappingsList = new List<RoleRightsMappingDTO>();

                foreach (var b in workspaceReports)
                {
                    RoleRightsMappingDTO a = new RoleRightsMappingDTO();
                    //a.ID = 0;
                    a.WorkspaceID = b.WorkspaceID;
                    a.WorkspaceName = b.WorkspaceName;
                    a.ReportID = b.ReportID;
                    a.ReportName = b.ReportName;
                    roleRightsMappingsList.Add(a);
                }
                List<RoleRightsMappingDTO> updatedRoleRightsMappingsList = new List<RoleRightsMappingDTO>();

                foreach (var s in roleRightsMappingsList)
                {
                    updatedRoleRightsMappingsList.Add(s);
                }

                foreach (var l1 in roleRightsMappingsList)
                {
                    bool existed = false;
                    foreach (var l2 in rights)
                    {
                        if (l1.WorkspaceID == l2.WorkspaceID && l1.ReportID == l2.ReportID)
                        {
                            existed = true;
                            break;
                        }
                    }
                    if (existed)
                    {
                        updatedRoleRightsMappingsList.Remove(l1);
                    }
                }

                rights = rights.Concat(updatedRoleRightsMappingsList);
                return Ok(rights);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch rights");
            }
        }


        [Route("api/GetEditUserRights")]
        //To get access rights
        public IHttpActionResult GetEditUserRights([FromUri] List<string> roleID, int userID)
        {
            try
            {

                RolesData roleRightsData = new RolesData();
                UsersData addUserData = new UsersData();

                var roleRights = addUserData.GetRights(roleID);

                var accessRights = addUserData.GetEditUserRights(userID);

                var workspaceReports = roleRightsData.GetWorkspacesAndReports();


                // User Access right mapping to roles rights
                List<RoleRightsMappingDTO> roleAccessRightsMappingsList = new List<RoleRightsMappingDTO>();

                foreach (var b in accessRights)
                {
                    RoleRightsMappingDTO c = new RoleRightsMappingDTO();
                    c.WorkspaceID = b.WorkspaceID;
                    c.WorkspaceName = b.WorkspaceName;
                    c.ReportID = b.ReportID;
                    c.ReportName = b.ReportName;
                    c.Active = b.Active;
                    roleAccessRightsMappingsList.Add(c);
                }

                List<RoleRightsMappingDTO> updateUserRightsMappingsList = new List<RoleRightsMappingDTO>();

                foreach (var s in roleAccessRightsMappingsList)
                {
                    updateUserRightsMappingsList.Add(s);
                }

                foreach (var l1 in roleAccessRightsMappingsList)
                {
                    bool existed = false;
                    foreach (var l2 in roleRights)
                    {
                        if (l1.WorkspaceID == l2.WorkspaceID && l1.ReportID == l2.ReportID)
                        {
                            existed = true;
                            break;
                        }
                    }
                    if (existed)
                    {
                        updateUserRightsMappingsList.Remove(l1);
                    }
                }

                roleRights = roleRights.Concat(updateUserRightsMappingsList);


                List<RoleRightsMappingDTO> roleRightsMappingsList = new List<RoleRightsMappingDTO>();

                foreach (var b in workspaceReports)
                {
                    RoleRightsMappingDTO a = new RoleRightsMappingDTO();
                    //a.ID = 0;
                    a.WorkspaceID = b.WorkspaceID;
                    a.WorkspaceName = b.WorkspaceName;
                    a.ReportID = b.ReportID;
                    a.ReportName = b.ReportName;
                    roleRightsMappingsList.Add(a);
                }
                List<RoleRightsMappingDTO> updatedRoleRightsMappingsList = new List<RoleRightsMappingDTO>();

                foreach (var s in roleRightsMappingsList)
                {
                    updatedRoleRightsMappingsList.Add(s);
                }

                foreach (var l1 in roleRightsMappingsList)
                {
                    bool existed = false;
                    foreach (var l2 in roleRights)
                    {
                        if (l1.WorkspaceID == l2.WorkspaceID && l1.ReportID == l2.ReportID)
                        {
                            existed = true;
                            break;
                        }
                    }
                    if (existed)
                    {
                        updatedRoleRightsMappingsList.Remove(l1);
                    }
                }

                roleRights = roleRights.Concat(updatedRoleRightsMappingsList);

                return Ok(roleRights);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch rights");
            }
        }


        [AcceptVerbs("GET", "POST")]
        [Route("api/UpdateUser")]
        public IHttpActionResult UpdateUser(UsersModel EditUserModel)
        {
            try
            {

                //if (!ModelState.IsValid)
                //    return BadRequest("Not a valid model");

                if (!ModelState.IsValid)
                {
                    return BadRequest("Not a valid model");
                }
                else
                {
                    UsersData updateUserData = new UsersData();
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UsersModel, UsersDTO>();
                        cfg.CreateMap<UserRoleMappingModel, UserRoleMappingDTO>();
                        cfg.CreateMap<WorkFlowMasterModel, WorkFlowMasterDTO>();
                        cfg.CreateMap<WorkFlowDetailsModel, WorkFlowDetailsDTO>();
                        cfg.CreateMap<UserAccessRightsModel, UserAccessRightsDTO>();
                    });
                    IMapper mapper = config.CreateMapper();

                    var updateUserDatamapping = mapper.Map<UsersModel, UsersDTO>(EditUserModel);

                    updateUserData.UpdateUsersData(updateUserDatamapping);

                    return Created("api/UpdateUser", true);
                }

            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }



        [Route("api/GetPermissionTypes")]
        public IHttpActionResult GetPermissionTypes()
        {
            try
            {
                List<PermissionMasterDTO> PermissionMasterDTO = new List<PermissionMasterDTO>();
                using (var context = new BIPortalEntities())
                {
                    
                    /*Getting data from database*/
                    var  objpermissiontypeslist = (from data in context.PermissionMasters
                                                             select data).ToList();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<PermissionMaster, PermissionMasterDTO>();                        
                    });
                    IMapper mapper = config.CreateMapper();


                    PermissionMasterDTO = mapper.Map<List<PermissionMaster>, List<PermissionMasterDTO>>(objpermissiontypeslist).ToList();
                }
                return Ok(PermissionMasterDTO);
            }

            catch (Exception ex)
            {
                return BadRequest("Could not fetch Permission Types");
            }
        }

    }
}
