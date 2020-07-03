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
using System.Web.Security;

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

                WorkSpaceData workSpaceData = new WorkSpaceData();
                var workSpaceAndReports = workSpaceData.GetPowerBIWorkspace();

                List<RoleRightsMappingDTO> roleRightsMappingsList = new List<RoleRightsMappingDTO>();
                                
                foreach(var b in workSpaceAndReports)
                {
                    RoleRightsMappingDTO a = new RoleRightsMappingDTO();
                    //a.ID = 0;
                    a.WorkspaceID = b.WorkSpaceId;
                    a.WorkspaceName = b.WorkSpaceName;
                    a.ReportID = b.ReportId;
                    a.ReportName = b.ReportName;                    
                    roleRightsMappingsList.Add(a);
                }
                List<RoleRightsMappingDTO> updatedRoleRightsMappingsList = new List<RoleRightsMappingDTO>();

                foreach (var s in roleRightsMappingsList)
                {
                    updatedRoleRightsMappingsList.Add(s);
                }

                foreach (var l1 in roleRightsMappingsList)
                {
                    bool existed = false;
                    foreach (var l2 in rights)
                    {
                        if (l1.WorkspaceID == l2.WorkspaceID && l1.ReportID == l2.ReportID)
                        {
                            existed = true;
                            break;
                        }
                    }
                    if (existed)
                    {
                        updatedRoleRightsMappingsList.Remove(l1);
                    }                    
                }

                //rights = rights.Concat(roleRightsMappingsList);
                rights = rights.Concat(updatedRoleRightsMappingsList);
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

        //[HttpPut]
        [Route("api/UpdateRoleAndRights")]
        //To update access rights
        public IHttpActionResult UpdateRoleAndRights(List<RoleRightsMappingModel> rolesRightsModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RolesData roleRightsData = new RolesData();

                    var config = new MapperConfiguration(cfg =>
                    {
                        //cfg.CreateMap<RolesModel, RolesDTO>();
                        cfg.CreateMap<RoleRightsMappingModel, RoleRightsMappingDTO>();
                    });
                    IMapper mapper = config.CreateMapper();

                    var roleRights = mapper.Map<List<RoleRightsMappingModel>, List<RoleRightsMappingDTO>>(rolesRightsModel);

                    roleRightsData.UpdateRoleAndRights(roleRights);

                    return Created("api/UpdateRoleAndRights", true);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Could not update rights");
            }
        }
    }
}
