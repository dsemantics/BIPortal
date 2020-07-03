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
        public int RequestID { get; set; }
        public string ReportID { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime ProcessedDate { get; set; }
        public string Status { get; set; }
    }
}
