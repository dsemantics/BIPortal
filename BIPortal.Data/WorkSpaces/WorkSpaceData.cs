using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BIPortal.DTO;
using BIPortal.Data.Email;

namespace BIPortal.Data.WorkSpaces
{
    public class WorkSpaceData
    {
        public IEnumerable<WorkspaceDTO> GetPowerBIWorkspace()
        {
            List<WorkspaceDTO> workspaceDTOList = new List<WorkspaceDTO>();

            // Create the InitialSessionState Object
            InitialSessionState iss = InitialSessionState.CreateDefault2();

            // Initialize PowerShell Engine
            var shell = PowerShell.Create(iss);
            shell.Commands.AddCommand("Connect-PowerBIServiceAccount");

            var userName = "biassistant@datasemantics.in";
            var pwd = "Dac21568";

            System.Security.SecureString theSecureString = new NetworkCredential(userName, pwd).SecurePassword;
            PSCredential cred = new PSCredential(userName, theSecureString);

            shell.Commands.AddParameter("Credential", cred);            

            var results = shell.Invoke();

            if (results.Count > 0)
            {
                var shell1 = PowerShell.Create(iss);
                shell1.Commands.AddCommand("Get-PowerBIWorkspace");
                //shell1.Commands.AddParameter("Scope", "Individual");

                var res = shell1.Invoke();

                if (res.Count > 0)
                {
                    foreach (var psObject in res)
                    {
                        //WorkspaceDTO workspaceDTO = new WorkspaceDTO();

                        var workSpaceId = psObject.Properties["Id"].Value;
                        //var workSpaceUser = psObject.Properties["User"].Value;                                                        
                        //workspaceDTO.WorkSpaceId = psObject.Properties["Id"].Value.ToString();
                        //workspaceDTO.WorkSpaceName = psObject.Properties["Name"].Value.ToString();

                        List<ReportsDTO> reportsDTOList = new List<ReportsDTO>();
                        var shell2 = PowerShell.Create(iss);
                        shell2.Commands.AddCommand("Get-PowerBIReport");
                        shell2.Commands.AddParameter("WorkspaceId", workSpaceId);
                        var result = shell2.Invoke();
                        if (result.Count > 0)
                        {
                            foreach (var psObjectReport in result)
                            {
                                //ReportsDTO reportsDTO = new ReportsDTO();
                                //reportsDTO.ReportId = psObjectReport.Properties["Id"].Value.ToString();
                                //reportsDTO.ReportName = psObjectReport.Properties["Name"].Value.ToString();
                                //reportsDTOList.Add(reportsDTO);
                                WorkspaceDTO workspaceDTO = new WorkspaceDTO();
                                workspaceDTO.WorkSpaceId = psObject.Properties["Id"].Value.ToString();
                                workspaceDTO.WorkSpaceName = psObject.Properties["Name"].Value.ToString();
                                workspaceDTO.ReportId= psObjectReport.Properties["Id"].Value.ToString();
                                workspaceDTO.ReportName= psObjectReport.Properties["Name"].Value.ToString();
                                workspaceDTO.ReportCount = result.Count;
                                workspaceDTOList.Add(workspaceDTO);
                            }                            
                            //workspaceDTO.Reports = reportsDTOList;
                        }
                        else
                        {
                            WorkspaceDTO workspaceDTO = new WorkspaceDTO();
                            workspaceDTO.WorkSpaceId = psObject.Properties["Id"].Value.ToString();
                            workspaceDTO.WorkSpaceName = psObject.Properties["Name"].Value.ToString();                          
                            workspaceDTOList.Add(workspaceDTO);
                        }
                       
                    }
                }
            }
            return workspaceDTOList;
        }

        //To get workspaces
        public IEnumerable<WorkSpaceOwnerDTO> GetWorkSpaceOwner()
        {
            using (var context = new BIPortalEntities())
            {
                //var workspaceOwnerResult = context.WorkspaceReportsMasters.Include("WorkSpaceOwnerMaster").ToList();
                var workspaceOwnerResult = context.GetWorkspaceOwner().ToList();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<GetWorkspaceOwner_Result1, WorkSpaceOwnerDTO>();                    
                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<List<GetWorkspaceOwner_Result1>, List<WorkSpaceOwnerDTO>>(workspaceOwnerResult);
            }
        }

        //To get workspaceowner by workspace id
        public WorkSpaceOwnerDTO GetWorkSpaceOwnerByWorkspaceId(string workspaceId)
        {
            using (var context = new BIPortalEntities())
            {
                var workspaceOwnerResult = context.WorkSpaceOwnerMasters.Where(x => x.WorkspaceID == workspaceId).FirstOrDefault();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<WorkSpaceOwnerMaster, WorkSpaceOwnerDTO>();
                    cfg.CreateMap<UserMaster, UsersDTO>();
                    cfg.CreateMap<PermissionMaster, PermissionMasterDTO>();
                    cfg.CreateMap<UserRoleMapping, UserRoleMappingDTO>();
                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<WorkSpaceOwnerMaster, WorkSpaceOwnerDTO>(workspaceOwnerResult);
            }
        }

        //To get workspaces
        public ReportsDTO GetReportsAndOwner(string workspaceid)
        {
            using (var context = new BIPortalEntities())
            {
                var result = new ReportsDTO();
                result.Users = context.UserMasters.Where(x => x.Active == true).Select(x => new UsersDTO() { UserID = x.UserID, UserName = x.FirstName + " " + x.LastName }).ToList();
                
                result.Reports = context.WorkspaceReportsMasters.Where(x => x.WorkspaceID == workspaceid && x.ReportID != null).Select(x => new ReportsDTO() { ReportId = x.ReportID, ReportName = x.ReportName }).ToList();
                               
                return result;
            }
        }

        //Save workspace Owner
        public void SaveWorkspaceOwner(WorkSpaceOwnerDTO workspaceOwnerDTO)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WorkSpaceOwnerDTO, WorkSpaceOwnerMaster>();                
            });
            IMapper mapper = config.CreateMapper();

            var workspaceOwner = mapper.Map<WorkSpaceOwnerDTO, WorkSpaceOwnerMaster>(workspaceOwnerDTO);

            using (var context = new BIPortalEntities())
            {
                var workspaceOwnerExists = context.WorkSpaceOwnerMasters.FirstOrDefault(c => c.WorkspaceID == workspaceOwner.WorkspaceID);
                if (workspaceOwnerExists != null)
                {
                    workspaceOwnerExists.OwnerID = workspaceOwner.OwnerID;
                    workspaceOwnerExists.ModifiedDate = workspaceOwner.ModifiedDate;
                    workspaceOwnerExists.ModifiedBy = workspaceOwner.ModifiedBy;
                }
                else
                {
                    var workspaceOwnerMaster = new WorkSpaceOwnerMaster
                    {
                        WorkspaceID = workspaceOwner.WorkspaceID,
                        OwnerID = workspaceOwner.OwnerID,
                        CreatedDate = workspaceOwner.CreatedDate,
                        CreatedBy = workspaceOwner.CreatedBy,
                        Active = workspaceOwner.Active
                    };
                    context.WorkSpaceOwnerMasters.Add(workspaceOwnerMaster);
                }

                context.SaveChanges();
            }
        }

        //Add a new user to a workspace
        public string AddPowerBIWorkspaceUser(List<WorkFlowMasterDTO> workFlowMasterDTO, string powerBIUserName, string powerBIPWD, string smtpHost, int smtpPort)
        {
            // Create the InitialSessionState Object
            InitialSessionState iss = InitialSessionState.CreateDefault2();

            // Initialize PowerShell Engine
            var shell = PowerShell.Create(iss);
            shell.Commands.AddCommand("Connect-PowerBIServiceAccount");

            System.Security.SecureString theSecureString = new NetworkCredential(powerBIUserName, powerBIPWD).SecurePassword;
            PSCredential cred = new PSCredential(powerBIUserName, theSecureString);

            shell.Commands.AddParameter("Credential", cred);

            var results = shell.Invoke();
            if (results.Count > 0)
            {
                using (var context = new BIPortalEntities())
                {
                    foreach (var a in workFlowMasterDTO)
                    {
                        var workflowMasterEntity = context.WorkFlowMasters.FirstOrDefault(x => x.RequestID == a.RequestID);
                        if (workflowMasterEntity != null)
                        {
                            // Initialize PowerShell Engine
                            var shell2 = PowerShell.Create(iss);
                            shell2.Commands.AddCommand("Add-PowerBIWorkspaceUser");
                            shell2.Commands.AddParameter("Id", workflowMasterEntity.WorkspaceID);
                            shell2.Commands.AddParameter("UserEmailAddress", workflowMasterEntity.RequestFor);
                            shell2.Commands.AddParameter("AccessRight", "Member");
                            var resaddUser = shell2.Invoke();

                            //send email
                            var subject = "Your request is approved.";
                            var body = "Your request for access to workspace " + workflowMasterEntity.WorkspaceName + " has been approved.";
                            EmailData emailData = new EmailData();
                            emailData.SendEmail(powerBIUserName, powerBIPWD, smtpHost, smtpPort, workflowMasterEntity.RequestFor, subject, body);
                        }
                    }
                }
            }
            return "User added to workspace successfully";
        }

        //To get workspaces
        //public IEnumerable<WorkSpaceMasterDTO> GetWorkspaces()
        //{
        //    using (var context = new BIPortalEntities())
        //    {
        //        var workspaceResult = context.WorkSpaceMasters.ToList();
        //        var config = new MapperConfiguration(cfg =>
        //        {
        //            cfg.CreateMap<WorkSpaceMaster, WorkSpaceMasterDTO>();
        //            //cfg.CreateMap<RoleRightsMapping, RoleRightsMappingDTO>();
        //        });
        //        IMapper mapper = config.CreateMapper();

        //        return mapper.Map<List<WorkSpaceMaster>, List<WorkSpaceMasterDTO>>(workspaceResult);
        //    }
        //}

        //To get workspaces
        //public IEnumerable<ReportsMasterDTO> GetReports()
        //{
        //    using (var context = new BIPortalEntities())
        //    {
        //        var reportsResult = context.ReportsMasters.ToList();
        //        var config = new MapperConfiguration(cfg =>
        //        {
        //            cfg.CreateMap<ReportsMaster, ReportsMasterDTO>();
        //            //cfg.CreateMap<RoleRightsMapping, RoleRightsMappingDTO>();
        //        });
        //        IMapper mapper = config.CreateMapper();

        //        return mapper.Map<List<ReportsMaster>, List<ReportsMasterDTO>>(reportsResult);
        //    }
        //}
    }
}
