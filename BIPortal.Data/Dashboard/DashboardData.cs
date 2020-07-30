using AutoMapper;
using BIPortal.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.Data.Dashboard
{
    public class DashboardData
    {
        //To get approved workspaces for the users
        public IEnumerable<WorkFlowMasterDTO> GetUsersWorkspaces(string emailID)
        {
            using (var context = new BIPortalEntities())
            {
                var status = "APPROVED";                
                var workFlowResult = context.WorkFlowMasters.Where(x => x.Status == status && x.RequestFor == emailID).ToList();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<WorkFlowMaster, WorkFlowMasterDTO>();
                    cfg.CreateMap<WorkFlowDetail, WorkFlowDetailsDTO>();
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
