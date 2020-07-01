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
                var roleMaster = new RoleMaster
                {
                    RoleName = roleAndRights.RoleName,
                    CreatedDate = DateTime.Now,
                    CreatedBy = "Venkat",
                    Active = true              
                };

                foreach (var d in roleAndRights.RoleRightsMappings)
                {
                    var rolerightsmapping = new RoleRightsMapping
                    {
                        WorkspaceID = d.WorkspaceID,
                        WorkspaceName = d.WorkspaceName,
                        ReportID = d.ReportID,
                        ReportName = d.ReportName,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Venkat",
                        Active = true
                    };
                    roleMaster.RoleRightsMappings.Add(rolerightsmapping);                    
                }

                context.RoleMasters.Add(roleMaster);               

                context.SaveChanges();                           
            }
        }
    }
}
