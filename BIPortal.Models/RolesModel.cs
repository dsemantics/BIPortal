using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.Models
{
    public class RolesModel
    {
        public int RoleID { get; set; }
        [Required(ErrorMessage ="Please enter a role")]
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool Active { get; set; }
        public IEnumerable<RoleRightsMappingModel> RoleRightsMappings { get; set; }
        //public IEnumerable<WorkspaceModel> Workspaces { get; set; }
    }
}
