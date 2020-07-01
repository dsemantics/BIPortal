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
using BIPortal.Models;

namespace BIPortalServices.Controllers
{
    public class RolesController : ApiController
    {
        [Route("api/GetRoles")]
        //To get roles
        public IHttpActionResult GetRoles()
        {
            try
            {
                RolesData rolesData = new RolesData();
                var roles = rolesData.GetRoles();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }

        [Route("api/GetRights")]
        //To get access rights
        public IHttpActionResult GetRights(int roleID)
        {
            try
            {
                RolesData roleRightsData = new RolesData();
                var rights = roleRightsData.GetRights(roleID);

                //WorkSpaceData workSpaceData = new WorkSpaceData();
                //var workSpaceAndReports = workSpaceData.GetPowerBIWorkspace();

                return Ok(rights);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch rights");
            }
        }

        [HttpPost]
        [Route("api/SaveRoleAndRights")]
        //To save role and access rights
        public IHttpActionResult SaveRoleAndRights(RolesModel rolesModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RolesData roleRightsData = new RolesData();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<RolesModel, RolesDTO>();
                        cfg.CreateMap<RoleRightsMappingModel, RoleRightsMappingDTO>();
                    });
                    IMapper mapper = config.CreateMapper();

                    var roleRights = mapper.Map<RolesModel, RolesDTO>(rolesModel);

                    roleRightsData.SaveRoleAndRights(roleRights);
                    
                    return Created("api/SaveRoleAndRights", true);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Could not save roles and rights");
            }
        }
    }
}
