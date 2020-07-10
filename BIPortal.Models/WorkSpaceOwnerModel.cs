using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.Models
{
    public class WorkSpaceOwnerModel
    {
        public int ID { get; set; }
        public string WorkspaceID { get; set; }
        public Nullable<int> OwnerID { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool Active { get; set; }
        public string OwnerName { get; set; }
        public string WorkspaceName { get; set; }
        public Nullable<int> ReportCount { get; set; }

        public virtual UsersModel UserMaster { get; set; }
    }
}
