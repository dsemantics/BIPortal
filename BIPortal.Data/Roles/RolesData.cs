using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BIPortal.DTO;

namespace BIPortal.Data.Roles
{
    public class RolesData
    {
        public IEnumerable<RolesDTO> GetRoles()
        {
            //List<RolesDTO> rolesDTO = new List<RolesDTO>();
            using (var context = new BIPortalEntities())
            {
                var rolesResult = context.RoleMasters.ToList();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<RoleMaster, RolesDTO>();
                });
                IMapper mapper = config.CreateMapper();

                return  mapper.Map<List<RoleMaster>, List<RolesDTO>>(rolesResult);
            }
        }
    }
}
