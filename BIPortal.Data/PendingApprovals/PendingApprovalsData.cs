using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BIPortal.DTO;

namespace BIPortal.Data.PendingApprovals
{
    public class PendingApprovalsData
    {
        //To get pending approvals
        public IEnumerable<WorkFlowMasterDTO> GetPendingApprovals()
        {
            //List<RolesDTO> rolesDTO = new List<RolesDTO>();
            using (var context = new BIPortalEntities())
            {
                //var rolesResult = context.RoleMasters.Include("RoleRightsMappings").Select(p => p).ToList();
                var workFlowResult = context.WorkFlowMasters.ToList();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<WorkFlowMaster, WorkFlowMasterDTO>();
                    cfg.CreateMap<UserMaster, UsersDTO>();
                    cfg.CreateMap<PermissionMaster, PermissionMasterDTO>();
                    cfg.CreateMap<UserRoleMapping, UserRoleMappingDTO>();
                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<List<WorkFlowMaster>, List<WorkFlowMasterDTO>>(workFlowResult);
            }
        }

        //To get request details for the given requestID
        public IEnumerable<WorkFlowMasterDTO> GetRequestDetails(int requestID)
        {
            using (var context = new BIPortalEntities())
            {
                //var rolesResult = context.RoleMasters.Include("RoleRightsMappings").Select(p => p).ToList();
                var workFlowResult = context.WorkFlowMasters.Where(x => x.RequestID == requestID).ToList();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<WorkFlowMaster, WorkFlowMasterDTO>();
                    cfg.CreateMap<UserMaster, UsersDTO>();
                    cfg.CreateMap<PermissionMaster, PermissionMasterDTO>();
                    cfg.CreateMap<UserRoleMapping, UserRoleMappingDTO>();
                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<List<WorkFlowMaster>, List<WorkFlowMasterDTO>>(workFlowResult);
            }
        }
    }
}
