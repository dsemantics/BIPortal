using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BIPortal.Models
{
    public class UsersModel
    {
        public int UserID { get; set; }
        public string Salutation { get; set; }

        [Required(ErrorMessage = "Firstame is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = " Valid Email is required.")]
        public string EmailID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool Active { get; set; }
        public int PermissionID { get; set; }
        public PermissionMasterModel PermissionMaster { get; set; }
        [Required(ErrorMessage = "Role Selection is required.")]
        public int[] SelectedRolesValues { get; set; }

        public List<UserRoleMappingModel> UserRoleMappings { get; set; }
        //public IEnumerable<WorkFlowMasterModel> WorkFlowMasterMappings { get; set; }
        //public IEnumerable<WorkFlowDetailsModel> WorkFlowDetailsMappings { get; set; }


    }
}
