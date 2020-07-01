using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.DTO
{
    public class RolesDTO
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool Active { get; set; }

        public IEnumerable<RoleRightsMappingDTO> RoleRightsMappings { get; set; }
        //public IEnumerable<WorkspaceDTO> Workspaces { get; set; }
    }
}
