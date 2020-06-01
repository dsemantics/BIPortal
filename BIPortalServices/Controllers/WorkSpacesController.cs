﻿using System;
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

namespace BIPortalServices.Controllers
{
    
    public class WorkSpacesController : ApiController
    {
        //Get Workspaces
        //[HttpGet]  
        [Route("api/GetWorkSpaces")]
        public IHttpActionResult GetPowerBIWorkspace()
        {
            List<WorkspaceDTO> workspaceDTOList = new List<WorkspaceDTO>();
            //IList<WorkspaceDTO> workspaceDTOList = null;
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

            var builder = new StringBuilder();

            // Execute the script 
            try
            {
                var results = shell.Invoke();

                // display results, with BaseObject converted to string
                // Note : use |out-string for console-like output
                if (results.Count > 0)
                {

                    // We use a string builder to create our result text
                    //var builder = new StringBuilder();                   

                    var shell1 = PowerShell.Create(iss);
                    shell1.Commands.AddCommand("Get-PowerBIWorkspace");
                    //shell1.Commands.AddParameter("Scope", "Individual");
                   
                    var res = shell1.Invoke();
                   
                    //WorkspaceDTO workspaceDTO = new WorkspaceDTO();
                    if (res.Count > 0)
                    {
                        foreach (var psObject in res)
                        {
                            WorkspaceDTO workspaceDTO = new WorkspaceDTO();
                            // Convert the Base Object to a string and append it to the string builder.
                            // Add \r\n for line breaks
                            var workSpaceName = psObject.Properties["Name"].Value;

                            //var workSpaceUser = psObject.Properties["User"].Value;

                            builder.Append(psObject.Properties["Name"].Value + "\r\n");
                            workspaceDTO.Id= (Guid) psObject.Properties["Id"].Value;
                            workspaceDTO.Name= psObject.Properties["Name"].Value.ToString();
                            workspaceDTOList.Add(workspaceDTO);
                        }
                        //Result.Text = Server.HtmlEncode(builder.ToString());
                    }                    
                    
                }
            }
            catch (ActionPreferenceStopException Error) { //Result.Text = Error.Message; 
                workspaceDTOList = null;
            }
            catch (RuntimeException Error) { //Result.Text = Error.Message; 
                workspaceDTOList = null;
            };

            if (workspaceDTOList.Count == 0)
            {
                return NotFound();
            }

            //return workspaceDTOList;
            return Ok(workspaceDTOList);
        }

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
    }
}
