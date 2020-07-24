using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BIPortal.Data.Email;
using BIPortal.DTO;

namespace BIPortal.Data.PendingApprovals
{
    public class PendingApprovalsData
    {
        //To get pending approvals based on owner id
        public IEnumerable<WorkFlowMasterDTO> GetPendingApprovals(int? ownerID)
        {
            //List<RolesDTO> rolesDTO = new List<RolesDTO>();
            using (var context = new BIPortalEntities())
            {
                var status = "PENDING";
                var workFlowResult = context.WorkFlowMasters.Where(x => x.Status == status && x.OwnerID == ownerID).ToList();
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

        //To get pending approvals forthe users
        public IEnumerable<WorkFlowMasterDTO> GetPendingApprovals(string emailID)
        {
            //List<RolesDTO> rolesDTO = new List<RolesDTO>();
            using (var context = new BIPortalEntities())
            {
                var status = "PENDING";
                //var workFlowResult = context.WorkFlowMasters.Where(x => x.Status == status).ToList();
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
                    if (workflowMasterEntity != null)
                    {
                        workflowMasterEntity.Status = f.Status;
                        workflowMasterEntity.ProcessedDate = f.ProcessedDate;

                        context.WorkFlowMasters.AddOrUpdate(workflowMasterEntity);
                        context.SaveChanges();
                    }
                    foreach (var e in f.WorkFlowDetails)
                    {
                        var workflowDetailEntity = context.WorkFlowDetails.FirstOrDefault(x => x.RequestID == e.RequestID && x.ReportID == e.ReportID);
                        if (workflowDetailEntity != null)
                        {
                            workflowDetailEntity.Status = e.Status;
                            workflowDetailEntity.ProcessedDate = e.ProcessedDate;

                            context.WorkFlowDetails.AddOrUpdate(workflowDetailEntity);
                            context.SaveChanges();
                        }
                    }                   
                }
            }
        }

        public void RejectRequest(List<WorkFlowMasterDTO> workFlowMasterDTO, string powerBIUserName, string powerBIPWD, string smtpHost, int smtpPort)
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
                    if (workflowMasterEntity != null)
                    {
                        workflowMasterEntity.Status = f.Status;
                        workflowMasterEntity.ProcessedDate = f.ProcessedDate;

                        context.WorkFlowMasters.AddOrUpdate(workflowMasterEntity);
                        context.SaveChanges();

                        //send email
                        var subject = "Your request is rejected.";
                        var body = "Your request for access to workspace " + workflowMasterEntity.WorkspaceName + " has been rejected.";
                        EmailData emailData = new EmailData();
                        emailData.SendEmail(powerBIUserName, powerBIPWD, smtpHost, smtpPort, workflowMasterEntity.RequestFor, subject, body);
                    }
                    foreach (var e in f.WorkFlowDetails)
                    {
                        var workflowDetailEntity = context.WorkFlowDetails.FirstOrDefault(x => x.RequestID == e.RequestID && x.ReportID == e.ReportID);
                        if (workflowDetailEntity != null)
                        {
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
}
