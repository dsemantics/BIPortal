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

                //var CurusersResult = (from u in context.UserMasters
                //                      where u.EmailID == sCurrentUserDetail && u.UserID !=
                //                      select u).ToList();

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

        public IEnumerable<UsersDTO> CheckUserExists(int userID,string currentUserDetail)
        {

            using (var context = new BIPortalEntities())
            {
                //var CurusersResult = (from u in context.UserMasters
                //                      where u.EmailID == currentUserDetail
                //                      select u).ToList();

                var CurusersResult = (from u in context.UserMasters
                                      where u.EmailID == currentUserDetail && u.UserID != userID
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
                //var CurusersResult = (from u in context.UserMasters
                //                      where u.UserID == iUSERID
                //                      select u).ToList();

                // Join 2 Tables
                var CurusersResult = (from a in context.UserMasters
                                      join b in context.UserRoleMappings on a.UserID equals b.UserID
                                      where a.UserID == iUSERID
                                      select a).ToList();

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

        public void SaveUsersData(UsersDTO userDTO)
         {

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UsersDTO, UserMaster>();
                cfg.CreateMap<UserRoleMappingDTO, UserRoleMapping>();
                cfg.CreateMap<WorkFlowMasterDTO, WorkFlowMaster>();
                cfg.CreateMap<WorkFlowDetailsDTO, WorkFlowDetail>();
                cfg.CreateMap<UserAccessRightsDTO, UserAccessRight>();

            });
            IMapper mapper = config.CreateMapper();

            var SaveUserDetails = mapper.Map<UsersDTO, UserMaster>(userDTO);

            var workFlowMasterDetails = mapper.Map<List<WorkFlowMasterDTO>, List<WorkFlowMaster>>(userDTO.WorkFlowMasterMappings);

            using (var context = new BIPortalEntities())
            {
                var saveUserMaster = new UserMaster
                {
                    Salutation = SaveUserDetails.Salutation,
                    FirstName = SaveUserDetails.FirstName,
                    LastName = SaveUserDetails.LastName,
                    EmailID = SaveUserDetails.EmailID,
                    PermissionID = SaveUserDetails.PermissionID,
                    CreatedDate = DateTime.Now,
                    CreatedBy = SaveUserDetails.CreatedBy,
                    ModifiedDate = DateTime.Now,
                    Active = SaveUserDetails.Active,
                    BIObjectType = SaveUserDetails.BIObjectType
                };

                // Insert UserRoleMapping
                foreach (var d in userDTO.SelectedRolesValues)
                {
                    var userrolemapping = new UserRoleMapping
                    {
                        //RoleID = 1, //should come from UI
                        RoleID = d,
                        CreatedDate = DateTime.Now,
                        CreatedBy = SaveUserDetails.CreatedBy,
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
                context.UserMasters.Add(saveUserMaster);
                context.SaveChanges();

                context.WorkFlowMasters.AddRange(workFlowMasterDetails);
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
                    cfg.CreateMap<WorkFlowDetailsDTO, WorkFlowDetail>();
                    cfg.CreateMap<UserAccessRightsDTO, UserAccessRight>();

                });
                IMapper mapper = config.CreateMapper();

                var updateUserDetails = mapper.Map<UsersDTO, UserMaster>(userDTO);

                var workFlowMasterDetails = mapper.Map<List<WorkFlowMasterDTO>, List<WorkFlowMaster>>(userDTO.WorkFlowMasterMappings);

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
                        EditUserData.CreatedDate = DateTime.Now;
                        EditUserData.ModifiedBy = updateUserDetails.ModifiedBy;
                        EditUserData.ModifiedDate = DateTime.Now;
                        EditUserData.Active = updateUserDetails.Active;
                        EditUserData.BIObjectType = updateUserDetails.BIObjectType;
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
                            //CreatedBy = "Selva",
                            CreatedBy = updateUserDetails.CreatedBy,
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
                            CreatedDate = DateTime.Now,
                            CreatedBy = e.CreatedBy,
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
                        context.WorkFlowMasters.AddRange(workFlowMasterDetails);
                        context.SaveChanges();

                    }
                }
            }
            catch (Exception e)
            { 
                Console.WriteLine(e.StackTrace);
            }
           
        }

        //To get access rights for the given roleids
        public IEnumerable<RoleRightsMappingDTO> GetRights(List<string> roleID)
        {
            using (var context = new BIPortalEntities())
            {
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

        //To get access rights for the given user
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

        public IEnumerable<WorkFlowMasterDTO> GetPendingRequests()
        {

            using (var context = new BIPortalEntities())
            {

                var workFlowPendingResult = context.WorkFlowMasters.ToList();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserMaster, UsersDTO>();
                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<List<WorkFlowMaster>, List<WorkFlowMasterDTO>>(workFlowPendingResult);

            }
        }

    }
}
