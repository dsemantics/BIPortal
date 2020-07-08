using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BIPortal.DTO;

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
