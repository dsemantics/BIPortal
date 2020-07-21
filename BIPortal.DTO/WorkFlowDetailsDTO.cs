using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.DTO
{
    public class WorkFlowDetailsDTO
    {
        public int RequestDetailID { get; set; }
        public Nullable<int> RequestID { get; set; }
        public string ReportID { get; set; }
        public string ReportName { get; set; }
        public Nullable<System.DateTime> RequestedDate { get; set; }
        public Nullable<System.DateTime> ProcessedDate { get; set; }
        public string Status { get; set; }

        [System.Runtime.Serialization.IgnoreDataMember]
        public WorkFlowMasterDTO WorkFlowMaster { get; set; }
    }
}
