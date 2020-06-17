using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using BIPortal.Data;
using BIPortal.DTO;

namespace BIPortalServices.Controllers
{
    public class RolesController : ApiController
    {
        [Route("api/GetRoles")]
        public IHttpActionResult GetRoles()
        {
            try
            {
                List<RolesDTO> rolesDTO = new List<RolesDTO>();
                using (var context = new BIPortalEntities())
                {
                    var rolesResult = context.RoleMasters.ToList();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<RoleMaster, RolesDTO>();
                    });
                    IMapper mapper = config.CreateMapper();

                    rolesDTO = mapper.Map<List<RoleMaster>, List<RolesDTO>>(rolesResult);
                }
                return Ok(rolesDTO);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }
    }
}
