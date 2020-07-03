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

                    Salutation = "Mr.",
                    FirstName = SaveUserDetails.FirstName,
                    LastName = SaveUserDetails.LastName,
                    EmailID = SaveUserDetails.EmailID,
                    PermissionID = 1,
                    //CreatedDate = DateTime.Now,
                    CreatedDate = SaveUserDetails.CreatedDate,
                    CreatedBy = "SK",
                    ModifiedDate = DateTime.Now,
                    Active = SaveUserDetails.Active

                };

                var testUsermap = new UserRoleMapping
                {
                    RoleID = 1, //should come from UI
                    CreatedDate = DateTime.Now,
                    CreatedBy = "Selva",
                    Active = true
                };
                saveUserMaster.UserRoleMappings.Add(testUsermap);



                // Insert UserRoleMapping
                //foreach (var d in SaveUserDetails.UserRoleMappings)
                //{
                //    var userrolemapping = new UserRoleMapping
                //    {
                //        //UserID = d.UserID,
                //        //RoleID = d.RoleID,
                //        RoleID = 1, //should come from UI
                //        CreatedDate = DateTime.Now,
                //        CreatedBy = "Selva",
                //        Active = d.Active
                //    };
                //    UserMaster.UserRoleMappings.Add(userrolemapping);
                //}

                // Insert WorkFlowMaster
                //foreach (var e in SaveUserDetails.WorkFlowMasters)
                //{
                //    var userworkflowmastermapping = new WorkFlowMaster
                //    {
                //        //RequestID  = e.RequestID,
                //        WorkspaceID = e.WorkspaceID
                        

                //    };
                //    UserMaster.WorkFlowMasters.Add(userworkflowmastermapping);
                //}

                context.UserMasters.Add(saveUserMaster);

                context.SaveChanges();
            }
        }

    }
}
