using AutoMapper;
using BIPortal.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.Data.Users
{
    public class UsersData
    {
        public IEnumerable<UsersDTO> GetUsers()
        {
            
            using (var context = new BIPortalEntities())
            {
                
                var usersResult = context.UserMasters.ToList();

                var config = new MapperConfiguration(cfg =>
                {
 
                    cfg.CreateMap<UserMaster, UsersDTO>();
                    cfg.CreateMap<PermissionMaster, PermissionMasterDTO>();
                    cfg.CreateMap<UserRoleMapping, UserRoleMappingDTO>();
                    cfg.CreateMap<WorkFlowMaster, WorkFlowMasterDTO>();


                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<List<UserMaster>, List<UsersDTO>>(usersResult);

            }
        }

        public IEnumerable<UsersDTO> GetCurrentUser(string sCurrentUserDetail)
        {

            using (var context = new BIPortalEntities())
            {
                var CurusersResult = (from u in context.UserMasters
                            where u.EmailID == sCurrentUserDetail
                            select u).ToList();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserMaster, UsersDTO>();
                    cfg.CreateMap<PermissionMaster, PermissionMasterDTO>();
                    cfg.CreateMap<UserRoleMapping, UserRoleMappingDTO>();
                    cfg.CreateMap<WorkFlowMaster, WorkFlowMasterDTO>();
                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<List<UserMaster>, List<UsersDTO>>(CurusersResult);

            }
        }

        public IEnumerable<UsersDTO> GetSeletedUser(int iUSERID)
        {

            using (var context = new BIPortalEntities())
            {
                var CurusersResult = (from u in context.UserMasters
                                      where u.UserID == iUSERID
                                      select u).ToList();

                // Join 2 Tables
                //var CurusersResult = (from a in context.UserMasters
                //                      join b in context.UserRoleMappings on a.UserID equals b.UserID
                //                      where a.UserID == iUSERID
                //                      select a).ToList();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserMaster, UsersDTO>();
                    cfg.CreateMap<PermissionMaster, PermissionMasterDTO>();
                    cfg.CreateMap<UserRoleMapping, UserRoleMappingDTO>();
                    cfg.CreateMap<WorkFlowMaster, WorkFlowMasterDTO>();
                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<List<UserMaster>, List<UsersDTO>>(CurusersResult);

            }
        }


        //public IEnumerable<UsersDTO> GetSelectedUserRoles(int iUSERID)
        //{
        //    using (var context = new BIPortalEntities())
        //    {
        //        var curUsersRolesResult = (from u in context.UserRoleMappings
        //                                   where u.UserID == iUSERID
        //                                   select u).FirstOrDefault();

        //        // Join 2 Tables
        //        //var CurusersResult = (from a in context.UserMasters
        //        //                      join b in context.UserRoleMappings on a.UserID equals b.UserID
        //        //                      where a.UserID == iUSERID
        //        //                      select a).ToList();

        //        var config = new MapperConfiguration(cfg =>
        //        {
        //            cfg.CreateMap<UserMaster, UsersDTO>();
        //            cfg.CreateMap<PermissionMaster, PermissionMasterDTO>();
        //            cfg.CreateMap<UserRoleMapping, UserRoleMappingDTO>();
        //            cfg.CreateMap<WorkFlowMaster, WorkFlowMasterDTO>();
        //        });
        //        IMapper mapper = config.CreateMapper();

        //        //return mapper.Map<List<UserRoleMapping>, List<UserRoleMappingDTO>>(curUsersRolesResult);

        //        return mapper.Map<List<UserRoleMapping>, List<UserRoleMappingDTO>>(curUsersRolesResult);

        //    }
        //}

        public void SaveUsersData(UsersDTO userDTO)
         {

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UsersDTO, UserMaster>();
                cfg.CreateMap<UserRoleMappingDTO, UserRoleMapping>();
                cfg.CreateMap<WorkFlowMasterDTO, WorkFlowMaster>();
                //cfg.CreateMap<WorkFlowDetailsDTO, WorkFlowDetail>();
                cfg.CreateMap<UserAccessRightsDTO, UserAccessRight>();

            });
            IMapper mapper = config.CreateMapper();

            var SaveUserDetails = mapper.Map<UsersDTO, UserMaster>(userDTO);

            using (var context = new BIPortalEntities())
            {
                var saveUserMaster = new UserMaster
                {
                    //Salutation = "Mr.",
                    Salutation = SaveUserDetails.Salutation,
                    FirstName = SaveUserDetails.FirstName,
                    LastName = SaveUserDetails.LastName,
                    EmailID = SaveUserDetails.EmailID,
                    //PermissionID = 1,
                    PermissionID = SaveUserDetails.PermissionID,
                    CreatedDate = DateTime.Now,
                    //CreatedDate = SaveUserDetails.CreatedDate,
                    CreatedBy = "SK",
                    ModifiedDate = DateTime.Now,
                    Active = SaveUserDetails.Active

                };

                // Insert UserRoleMapping
                foreach (var d in userDTO.SelectedRolesValues)
                {
                    var userrolemapping = new UserRoleMapping
                    {
                        //RoleID = 1, //should come from UI
                        RoleID = d,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Selva",
                        Active = true
                    };
                    saveUserMaster.UserRoleMappings.Add(userrolemapping);
                }


                // Insert UserAcessRights
                foreach (var e in userDTO.UserAccessRightsMappings)
                {
                    var userRightsAccessMapping = new UserAccessRight
                    {
                        UserID = SaveUserDetails.UserID,
                        WorkspaceID = e.WorkspaceID,
                        WorkspaceName = e.WorkspaceName,
                        ReportID = e.ReportID,
                        ReportName = e.ReportName,
                        //CreatedDate = e.CreatedDate,
                        CreatedDate = DateTime.Now,
                        CreatedBy = e.CreatedBy,
                        Active = e.Active
                    };
                    saveUserMaster.UserAccessRights.Add(userRightsAccessMapping);
                }

                foreach (var f in userDTO.UserAccessRightsMappings)
                {
                    // Insert WorkFlowMaster
                    var ownerIDResult = (from u in context.WorkSpaceOwnerMasters
                                            where u.WorkspaceID == f.WorkspaceID
                                            select u).ToList();

                    var userWorkFlowMasterMapping = new WorkFlowMaster
                    {
                        WorkspaceID = f.WorkspaceID,
                        WorkspaceName = f.WorkspaceName,
                        //ReportID = f.ReportID,
                        //ReportName = f.ReportName,
                        OwnerID = ownerIDResult[0].OwnerID,
                        RequestedBy = "selva", // user(logged In) email address should come here
                        RequestedDate = DateTime.Now,
                        Status = "PENDING"
                    };
                    saveUserMaster.WorkFlowMasters.Add(userWorkFlowMasterMapping);
                }

                context.UserMasters.Add(saveUserMaster);
                context.SaveChanges();
            }
        }

        public void UpdateUsersData(UsersDTO userDTO)
        {

            try
            {

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UsersDTO, UserMaster>();
                    cfg.CreateMap<UserRoleMappingDTO, UserRoleMapping>();
                    cfg.CreateMap<WorkFlowMasterDTO, WorkFlowMaster>();
                   // cfg.CreateMap<WorkFlowDetailsDTO, WorkFlowDetail>();
                    cfg.CreateMap<UserAccessRightsDTO, UserAccessRight>();

                });
                IMapper mapper = config.CreateMapper();

                var updateUserDetails = mapper.Map<UsersDTO, UserMaster>(userDTO);

                using (var context = new BIPortalEntities())
                {
                    var EditUserData = context.UserMasters.Where(x => x.UserID == updateUserDetails.UserID).FirstOrDefault();

                    if (EditUserData != null)
                    {
                        EditUserData.EmailID = updateUserDetails.EmailID;
                        EditUserData.FirstName = updateUserDetails.FirstName;
                        EditUserData.LastName = updateUserDetails.LastName;
                        EditUserData.PermissionID = updateUserDetails.PermissionID;
                        EditUserData.Salutation = updateUserDetails.Salutation;
                        //EditUserData.CreatedDate = EditUserModel.CreatedDate;
                        EditUserData.ModifiedBy = "Selva";
                        EditUserData.ModifiedDate = DateTime.Now;
                        //EditUserData.CreatedBy = EditUserModel.CreatedBy;
                        EditUserData.Active = updateUserDetails.Active;
                        context.SaveChanges();
                    }


                    // remove existing UserRoleMapping for userid
                    var userRoleMappingRoleExists = context.UserRoleMappings.Where(x => x.UserID == updateUserDetails.UserID);
                    if (userRoleMappingRoleExists != null)
                    {
                        context.UserRoleMappings.RemoveRange(userRoleMappingRoleExists);
                        context.SaveChanges();
                    }

                    // Insert UserRoleMapping
                    foreach (var d in userDTO.SelectedRolesValues)
                    {
                        var userrolemapping = new UserRoleMapping
                        {
                            //RoleID = 1, //should come from UI
                            RoleID = d,
                            CreatedDate = DateTime.Now,
                            CreatedBy = "Selva",
                            Active = true
                        };
                        EditUserData.UserRoleMappings.Add(userrolemapping);
                    }
                    context.SaveChanges();

                    // remove existing UserAccessRights for userid
                    var userUserAccessRightsMappingUserExists = context.UserAccessRights.Where(x => x.UserID == updateUserDetails.UserID);
                    if (userUserAccessRightsMappingUserExists != null)
                    {
                        context.UserAccessRights.RemoveRange(userUserAccessRightsMappingUserExists);
                        context.SaveChanges();
                    }

                    //Insert UserAccessRights
                    foreach (var e in userDTO.UserAccessRightsMappings)
                    {
                        var userRightsAccessMapping = new UserAccessRight
                        {
                            UserID = updateUserDetails.UserID,
                            WorkspaceID = e.WorkspaceID,
                            WorkspaceName = e.WorkspaceName,
                            ReportID = e.ReportID,
                            ReportName = e.ReportName,
                            //CreatedDate = e.CreatedDate,
                            CreatedDate = DateTime.Now,
                            CreatedBy = e.CreatedBy,
                            //ModifiedDate = e.ModifiedDate,
                            ModifiedDate = DateTime.Now,
                            ModifiedBy = e.ModifiedBy,
                            Active = e.Active
                        };
                        EditUserData.UserAccessRights.Add(userRightsAccessMapping);
                    }
                    context.SaveChanges();

                    // save only active users
                    if (userDTO.Active)
                    {
                        foreach (var f in userDTO.UserAccessRightsMappings)
                        {
                            // Insert WorkFlowMaster
                            var ownerIDResult = (from u in context.WorkSpaceOwnerMasters
                                                 where u.WorkspaceID == f.WorkspaceID
                                                 select u).ToList();

                            var userWorkFlowMasterMapping = new WorkFlowMaster
                            {
                                WorkspaceID = f.WorkspaceID,
                                WorkspaceName = f.WorkspaceName,
                                //ReportID = f.ReportID,
                                //ReportName = f.ReportName,
                                OwnerID = ownerIDResult[0].OwnerID,
                                RequestedBy = "selva", // user(logged In) email address should come here
                                RequestedDate = DateTime.Now,
                                Status = "PENDING"
                            };
                            EditUserData.WorkFlowMasters.Add(userWorkFlowMasterMapping);
                        }
                        context.SaveChanges();
                    }

                    

                }

            }

            catch (Exception e)
            { 
                Console.WriteLine(e.StackTrace);
            }
           
        }

        //To get access rights for the given roleid
        public IEnumerable<RoleRightsMappingDTO> GetRights(List<string> roleID)
        {
            using (var context = new BIPortalEntities())
            {


                //int[] arrrr = Enumerable.Range(0, 400).ToArray();

                //var rightsResult = context.RoleRightsMappings.Where(x => x.RoleID == roleID).ToList();

                // RoleId In condition 

                //var rightsResult = context.RoleRightsMappings.Where(s => roleID.Contains(s.RoleID)).ToList();

                //string[] sarray = myList.ToArray();
                var rightsResult = context.RoleRightsMappings.Where(s => roleID.Contains(s.RoleID.ToString())).ToList();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<RoleMaster, RolesDTO>();
                    cfg.CreateMap<RoleRightsMapping, RoleRightsMappingDTO>();
                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<List<RoleRightsMapping>, List<RoleRightsMappingDTO>>(rightsResult.ToList());
            }
        }


        public IEnumerable<UserAccessRightsDTO> GetEditUserRights(int userID)
        {
            using (var context = new BIPortalEntities())
            {

                var edituserResult = context.UserAccessRights.Where(x => x.UserID == userID).ToList();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserMaster, UsersDTO>();
                    cfg.CreateMap<UserAccessRight, UserAccessRightsDTO>();
                });
                IMapper mapper = config.CreateMapper();
                return mapper.Map<List<UserAccessRight>, List<UserAccessRightsDTO>>(edituserResult.ToList());
            }
        }

    }
}
