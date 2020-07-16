
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.DTO
{
   public class UsersDTO
    {
        public int UserID { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool Active { get; set; }
        public int PermissionID { get; set; }
        public PermissionMasterDTO PermissionMaster { get; set; }
        public int[] SelectedRolesValues { get; set; }
        public List<UserRoleMappingDTO> UserRoleMappings { get; set; }
        public string UserName { get; set; }
        public List<WorkFlowMasterDTO> WorkFlowMasterMappings { get; set; }
        public List<WorkFlowDetailsDTO> WorkFlowDetailsMappings { get; set; }
        public List<UserAccessRightsDTO> UserAccessRightsMappings { get; set; }
        

    }
}
