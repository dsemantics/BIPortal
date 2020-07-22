using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BIPortal.DTO;

namespace BIPortal.Data.Roles
{
    public class RolesData
    {
        //To get roles
        public IEnumerable<RolesDTO> GetRoles()
        {
            //List<RolesDTO> rolesDTO = new List<RolesDTO>();
            using (var context = new BIPortalEntities())
            {
                //var rolesResult = context.RoleMasters.Include("RoleRightsMappings").Select(p => p).ToList();
                var rolesResult = context.RoleMasters.ToList();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<RoleMaster, RolesDTO>();
                    cfg.CreateMap<RoleRightsMapping,RoleRightsMappingDTO>();
                });
                IMapper mapper = config.CreateMapper();

                return  mapper.Map<List<RoleMaster>, List<RolesDTO>>(rolesResult);
            }
        }

        //To get access rights for the given roleid
        public IEnumerable<RoleRightsMappingDTO> GetRights(int roleID)
        {
            using (var context = new BIPortalEntities())
            {
                //var rolesResult = context.RoleMasters.Include("RoleRightsMappings").Select(p => p).ToList();
                var rightsResult = context.RoleRightsMappings.Where(x => x.RoleID == roleID).ToList();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<RoleMaster, RolesDTO>();
                    cfg.CreateMap<RoleRightsMapping, RoleRightsMappingDTO>();
                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<List<RoleRightsMapping>, List<RoleRightsMappingDTO>>(rightsResult);
            }
        }

        public IEnumerable<WorkspaceReportsDTO> GetWorkspacesAndReports()
        {
            using (var context = new BIPortalEntities())
            {
                var workspaceReportsResult = context.WorkspaceReportsMasters.ToList();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<WorkspaceReportsMaster, WorkspaceReportsDTO>();                    
                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<List<WorkspaceReportsMaster>, List<WorkspaceReportsDTO>>(workspaceReportsResult);
            }
        }

        //Save role and access rights
        public void SaveRoleAndRights(RolesDTO rolesDTO)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RolesDTO, RoleMaster>();
                cfg.CreateMap<RoleRightsMappingDTO, RoleRightsMapping>();
            });
            IMapper mapper = config.CreateMapper();

            var roleAndRights = mapper.Map<RolesDTO, RoleMaster>(rolesDTO);

            using (var context = new BIPortalEntities())
            {
                //var roleExists = context.RoleMasters.FirstOrDefault(c => c.RoleName.ToLower() == rolesDTO.RoleName.ToLower());
                if (context.RoleMasters.Any(o => o.RoleName.ToLower() == rolesDTO.RoleName.ToLower())) return;
                var roleMaster = new RoleMaster
                {
                    RoleName = roleAndRights.RoleName,
                    CreatedDate = roleAndRights.CreatedDate,
                    CreatedBy = roleAndRights.CreatedBy,
                    Active = roleAndRights.Active              
                };

                foreach (var d in roleAndRights.RoleRightsMappings)
                {
                    var rolerightsmapping = new RoleRightsMapping
                    {
                        WorkspaceID = d.WorkspaceID,
                        WorkspaceName = d.WorkspaceName,
                        ReportID = d.ReportID,
                        ReportName = d.ReportName,
                        CreatedDate = d.CreatedDate,
                        CreatedBy = d.CreatedBy,
                        Active = d.Active
                    };
                    roleMaster.RoleRightsMappings.Add(rolerightsmapping);                    
                }

                context.RoleMasters.Add(roleMaster);               

                context.SaveChanges();                           
            }
        }

        //Update access rights
        public void UpdateRoleAndRights(List<RoleRightsMappingDTO> roleRights)
        {
            var config = new MapperConfiguration(cfg =>
            {
                //cfg.CreateMap<RolesDTO, RoleMaster>();
                cfg.CreateMap<RoleRightsMappingDTO, RoleRightsMapping>();
            });
            IMapper mapper = config.CreateMapper();

            var roleAndRights = mapper.Map<List<RoleRightsMappingDTO>, List<RoleRightsMapping>>(roleRights);

            var roleID = roleAndRights[0].RoleID;
            using (var context = new BIPortalEntities())
            {
                var roleRightsMappingExists = context.RoleRightsMappings.Where(c => c.RoleID == roleID);

                if (roleRightsMappingExists != null)
                {
                    context.RoleRightsMappings.RemoveRange(roleRightsMappingExists);
                    context.SaveChanges();
                }

                foreach (var d in roleAndRights)
                {
                    var rolerightsmapping = new RoleRightsMapping
                    {
                        RoleID = d.RoleID,
                        WorkspaceID = d.WorkspaceID,
                        WorkspaceName = d.WorkspaceName,
                        ReportID = d.ReportID,
                        ReportName = d.ReportName,
                        CreatedDate = d.CreatedDate,
                        CreatedBy = d.CreatedBy,
                        ModifiedDate = d.ModifiedDate,
                        ModifiedBy = d.ModifiedBy,
                        Active = d.Active
                    };
                    context.RoleRightsMappings.Add(rolerightsmapping);
                }
                context.SaveChanges();
            }
        }        
    }
}
