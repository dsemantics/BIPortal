using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Web.UI;
using System.Web;
using BIPortal.DTO;
using BIPortal.Data;
using BIPortal.Data.WorkSpaces;
using BIPortal.Models;
using AutoMapper;

namespace BIPortalServices.Controllers
{
    
    public class WorkSpacesController : ApiController
    {
        //Get Workspaces
        //[HttpGet]  
        [Route("api/GetWorkSpaces")]
        public IHttpActionResult GetPowerBIWorkspace()
        {
            try
            {
                WorkSpaceData workSpaceData = new WorkSpaceData();
                var workSpaceAndReports = workSpaceData.GetPowerBIWorkspace();
                return Ok(workSpaceAndReports);

            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch workspace details");
            }
        }

        [Route("api/GetWorkSpaceOwner")]
        public IHttpActionResult GetWorkSpaceOwner()
        {
            try
            {
                WorkSpaceData workSpaceData = new WorkSpaceData();
                var workSpaceAndReports = workSpaceData.GetWorkSpaceOwner();
                return Ok(workSpaceAndReports);

            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch workspace details");
            }
        }

        [Route("api/GetReportsAndOwner")]
        public IHttpActionResult GetReportsAndOwner(string workspaceid)
        {
            try
            {
                WorkSpaceData workSpaceData = new WorkSpaceData();
                var reportsOwners = workSpaceData.GetReportsAndOwner(workspaceid);
                return Ok(reportsOwners);

            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch workspace details");
            }
        }

        [HttpPost]
        [Route("api/SaveWorkspaceOwner")]
        //To save role and access rights
        public IHttpActionResult SaveWorkspaceOwner(WorkSpaceOwnerModel workSpaceOwnerModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    WorkSpaceData workSpaceData = new WorkSpaceData();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<WorkSpaceOwnerModel, WorkSpaceOwnerDTO>();                        
                    });
                    IMapper mapper = config.CreateMapper();

                    var workspaceOwner = mapper.Map<WorkSpaceOwnerModel, WorkSpaceOwnerDTO>(workSpaceOwnerModel);

                    workSpaceData.SaveWorkspaceOwner(workspaceOwner);

                    return Created("api/SaveWorkspaceOwner", true);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Could not save workspace Owner");
            }
        }

        //Get Workspaces
        //[HttpGet]  
        //[Route("api/GetWorkSpace")]
        //public IHttpActionResult GetWorkspaces()
        //{
        //    try
        //    {
        //        WorkSpaceData workSpaceData = new WorkSpaceData();
        //        var workSpaceAndReports = workSpaceData.GetWorkspaces();
        //        return Ok(workSpaceAndReports);

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Could not fetch workspace details");
        //    }
        //}


        //Add a new user to a workspace
        public string AddPowerBIWorkspaceUser()
        {
            // Create the InitialSessionState Object
            InitialSessionState iss = InitialSessionState.CreateDefault2();

            // Initialize PowerShell Engine
            var shell2 = PowerShell.Create(iss);
            shell2.Commands.AddCommand("Add-PowerBIWorkspaceUser");
            shell2.Commands.AddParameter("Id", "ea5ec2c2-def9-4b74-9133-305511d96fdf");
            shell2.Commands.AddParameter("UserEmailAddress", "venkata.murakunda@datasemantics.in");
            shell2.Commands.AddParameter("AccessRight", "Member");
            var resaddUser = shell2.Invoke();
            //AddResult.Text = "User added to workspace successfully";
            return "User added to workspace successfully";
        }

        //Remove a user from a given workspace
        public string RemovePowerBIWorkspaceUser()
        {
            // Create the InitialSessionState Object
            InitialSessionState iss = InitialSessionState.CreateDefault2();

            // Initialize PowerShell Engine
            var shell2 = PowerShell.Create(iss);
            shell2.Commands.AddCommand("Remove-PowerBIWorkspaceUser");
            shell2.Commands.AddParameter("Id", "ea5ec2c2-def9-4b74-9133-305511d96fdf");
            shell2.Commands.AddParameter("UserEmailAddress", "venkata.murakunda@datasemantics.in");
            var resaddUser = shell2.Invoke();
            //AddResult.Text = "User Removed from workspace successfully";
            return "User Removed from workspace successfully";
        }

        public string GetPowerBIWorkspaceMigrationStatus()
        {
            InitialSessionState iss = InitialSessionState.CreateDefault2();
            
            var shell2 = PowerShell.Create(iss);
            shell2.Commands.AddCommand("Get-PowerBIWorkspaceMigrationStatus");
            shell2.Commands.AddParameter("Id", "ea5ec2c2-def9-4b74-9133-305511d96fdf");           
            var resMigrationStatus = shell2.Invoke();
            
            return "Migration status";            
        }

        public string NewPowerBIWorkspace()
        {
            InitialSessionState iss = InitialSessionState.CreateDefault2();

            var shell2 = PowerShell.Create(iss);
            shell2.Commands.AddCommand("New-PowerBIWorkspace");
            shell2.Commands.AddParameter("Name", "New Workspace");
            var resNewWorkspace = shell2.Invoke();

            return "New Workspace Created Successfully.";            
        }
        public string RestorePowerBIWorkspace()
        {
            InitialSessionState iss = InitialSessionState.CreateDefault2();

            var shell2 = PowerShell.Create(iss);
            shell2.Commands.AddCommand("Restore-PowerBIWorkspace");
            shell2.Commands.AddParameter("Id", "ea5ec2c2-def9-4b74-9133-305511d96fdf");
            shell2.Commands.AddParameter("RestoredName", "TestWorkspace");
            shell2.Commands.AddParameter("AdminEmailAddress", "john@contoso.com");
            var resRestoreWorkspace = shell2.Invoke();

            return "Workspace Restored Successfully.";            
        }
        public string SetPowerBIWorkspace()
        {
            InitialSessionState iss = InitialSessionState.CreateDefault2();

            var shell2 = PowerShell.Create(iss);
            shell2.Commands.AddCommand("Set-PowerBIWorkspace");
            shell2.Commands.AddParameter("Scope", "Organization");
            shell2.Commands.AddParameter("Id", "ea5ec2c2-def9-4b74-9133-305511d96fdf");
            shell2.Commands.AddParameter("Name", "Test Name");
            shell2.Commands.AddParameter("Description", "Test Description");
            var resSetWorkspace = shell2.Invoke();

            return "Workspace Updated Successfully.";
        }

    }
}
