using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
                var status = "PENDING";
                var workFlowResult = context.WorkFlowMasters.Where(x => x.Status == status).ToList();
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
                    cfg.CreateMap<WorkFlowDetail, WorkFlowDetailsDTO>();
                    cfg.CreateMap<UserMaster, UsersDTO>();
                    cfg.CreateMap<PermissionMaster, PermissionMasterDTO>();
                    cfg.CreateMap<UserRoleMapping, UserRoleMappingDTO>();
                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<List<WorkFlowMaster>, List<WorkFlowMasterDTO>>(workFlowResult);
            }
        }

        public void ApproveRequest(List<WorkFlowMasterDTO> workFlowMasterDTO)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WorkFlowMasterDTO, WorkFlowMaster>();
                cfg.CreateMap<WorkFlowDetailsDTO, WorkFlowDetail>();
            });
            IMapper mapper = config.CreateMapper();

            var workFlowMasterDetails = mapper.Map<List<WorkFlowMasterDTO>, List<WorkFlowMaster>>(workFlowMasterDTO);

            using (var context = new BIPortalEntities())
            {
                foreach (var f in workFlowMasterDTO)
                {
                    var workflowMasterEntity = context.WorkFlowMasters.FirstOrDefault(x => x.RequestID == f.RequestID);

                    workflowMasterEntity.Status = f.Status;
                    workflowMasterEntity.ProcessedDate = f.ProcessedDate;

                    context.WorkFlowMasters.AddOrUpdate(workflowMasterEntity);
                    context.SaveChanges();

                    foreach (var e in f.WorkFlowDetails)
                    {
                        var workflowDetailEntity = context.WorkFlowDetails.FirstOrDefault(x => x.RequestID == e.RequestID && x.ReportID==e.ReportID);

                        workflowDetailEntity.Status = e.Status;
                        workflowDetailEntity.ProcessedDate = e.ProcessedDate;

                        context.WorkFlowDetails.AddOrUpdate(workflowDetailEntity);
                        context.SaveChanges();
                    }
                }               
            }

        }

        public void RejectRequest(List<WorkFlowMasterDTO> workFlowMasterDTO)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WorkFlowMasterDTO, WorkFlowMaster>();
                cfg.CreateMap<WorkFlowDetailsDTO, WorkFlowDetail>();
            });
            IMapper mapper = config.CreateMapper();

            var workFlowMasterDetails = mapper.Map<List<WorkFlowMasterDTO>, List<WorkFlowMaster>>(workFlowMasterDTO);

            using (var context = new BIPortalEntities())
            {
                foreach (var f in workFlowMasterDTO)
                {
                    var workflowMasterEntity = context.WorkFlowMasters.FirstOrDefault(x => x.RequestID == f.RequestID);

                    workflowMasterEntity.Status = f.Status;
                    workflowMasterEntity.ProcessedDate = f.ProcessedDate;

                    context.WorkFlowMasters.AddOrUpdate(workflowMasterEntity);
                    context.SaveChanges();

                    foreach (var e in f.WorkFlowDetails)
                    {
                        var workflowDetailEntity = context.WorkFlowDetails.FirstOrDefault(x => x.RequestID == e.RequestID && x.ReportID == e.ReportID);

                        workflowDetailEntity.Status = e.Status;
                        workflowDetailEntity.ProcessedDate = e.ProcessedDate;

                        context.WorkFlowDetails.AddOrUpdate(workflowDetailEntity);
                        context.SaveChanges();
                    }
                }
            }

        }
    }
}
