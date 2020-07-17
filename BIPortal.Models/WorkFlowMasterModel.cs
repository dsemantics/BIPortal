using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.Models
{
    public class WorkFlowMasterModel
    {
        public int RequestID { get; set; }
        public string WorkspaceID { get; set; }
        public string WorkspaceName { get; set; }
        public string ReportID { get; set; }
        public string ReportName { get; set; }
        public Nullable<int> OwnerID { get; set; }
        public string RequestedBy { get; set; }
        public System.DateTime RequestedDate { get; set; }
        public Nullable<System.DateTime> ProcessedDate { get; set; }
        public string Status { get; set; }

        public UsersModel UserMaster { get; set; }

    }
}
