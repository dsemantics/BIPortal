using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.Models
{
    public class RoleRightsMappingModel
    {
        public int ID { get; set; }
        public int? RoleID { get; set; }
        public string WorkspaceID { get; set; }
        public string WorkspaceName { get; set; }
        public string ReportID { get; set; }
        public string ReportName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool Active { get; set; }
        public IEnumerable<RoleRightsMappingModel> Roles { get; set; }
    }
}
