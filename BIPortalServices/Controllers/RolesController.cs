using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using BIPortal.Data;
using BIPortal.DTO;
using BIPortal.Data.Roles;
using BIPortal.Data.WorkSpaces;

namespace BIPortalServices.Controllers
{
    public class RolesController : ApiController
    {
        [Route("api/GetRoles")]
        public IHttpActionResult GetRoles()
        {
            try
            {
                RolesData rolesData = new RolesData();
                var roles = rolesData.GetRoles();

                WorkSpaceData workSpaceData = new WorkSpaceData();
                var workSpaceAndReports = workSpaceData.GetPowerBIWorkspace();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }
    }
}
