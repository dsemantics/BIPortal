using BIPortal.Data.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BIPortalServices.Controllers
{
    public class DashboardController : ApiController
    {
        [Route("api/GetUsersWorkspaces")]
        //To get roles
        public IHttpActionResult GetUsersWorkspaces(string emailID)
        {
            try
            {
                DashboardData approvedWorkspacesData = new DashboardData();
                var approvedWorkspaces = approvedWorkspacesData.GetUsersWorkspaces(emailID);

                return Ok(approvedWorkspaces);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch users approved workspaces");
            }
        }
    }
}
