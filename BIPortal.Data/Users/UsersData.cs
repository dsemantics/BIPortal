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

            });
            IMapper mapper = config.CreateMapper();

            var SaveUserDetails = mapper.Map<UsersDTO, UserMaster>(userDTO);

            using (var context = new BIPortalEntities())
            {
                var saveUserMaster = new UserMaster
                {
                    //RoleName = roleAndRights.RoleName,
                    //CreatedDate = DateTime.Now,
                    //CreatedBy = "Venkat",
                    //Active = true

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

                // Insert WorkFlowMaster
                //foreach (var e in SaveUserDetails.WorkFlowMasters)
                //{
                //    var userworkflowmastermapping = new WorkFlowMaster
                //    {
                //        WorkspaceID = e.WorkspaceID
                //    };
                //    saveUserMaster.WorkFlowMasters.Add(userworkflowmastermapping);
                //}

                // Insert WorkFlowDetails
                //foreach (var e in SaveUserDetails.)
                //{
                //    var userworkflowmastermapping = new WorkFlowMaster
                //    {
                //        //RequestID  = e.RequestID,
                //        WorkspaceID = e.WorkspaceID
                //    };
                //    saveUserMaster.WorkFlowMasters.Add(userworkflowmastermapping);
                //}

                context.UserMasters.Add(saveUserMaster);

                context.SaveChanges();
            }
        }

    }
}
