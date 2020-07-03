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
        public int OwnerID { get; set; }
        public int RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime ProcessedDate { get; set; }
        public string Status { get; set; }

    }
}
